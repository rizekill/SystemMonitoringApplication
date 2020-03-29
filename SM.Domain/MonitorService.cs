using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SM.Domain.Interfaces;
using SM.Model;

namespace SM.Domain
{
    /// <summary>
    /// Сервис мониторинга процессов и состояния системы
    /// </summary>
    public class MonitorService<TMonitorSystemInfo, TProcessStateHandler> : IMonitorService
        where TMonitorSystemInfo : MonitorSystemInfoBase, new()
        where TProcessStateHandler : ProcessStateHandlerBase, new()
    {
        public MonitorService()
        {
            _observedProcesses = new ConcurrentDictionary<string, ProcessStateHandlerBase>();
            MonitorSystemInfo = new TMonitorSystemInfo();
            MonitorSystemInfo.OnHighLoaded += OnHighLoaded;
        }

        /// <summary>
        /// Системная информация
        /// </summary>
        public TMonitorSystemInfo MonitorSystemInfo;

        /// <summary>
        /// Срабатывает при появлении высокой нагрузки 
        /// </summary>
        public event EventHandler OnHighLoaded;

        /// <summary>
        /// Срабатывает при старте наблюдения за процессом
        /// </summary>
        public event EventHandler OnProcessObserveOpened;

        /// <summary>
        /// Срабатывает при обновлении состояния процесса
        /// </summary>
        public event EventHandler OnProcessStateUpdated;

        /// <summary>
        /// Срабатывает при завершении наблюдения за процессом
        /// </summary>
        public event EventHandler OnProcessObserveClosed;

        /// <summary>
        ///  Список отслеживаемых процессов
        /// </summary>
        private readonly ConcurrentDictionary<string, ProcessStateHandlerBase> _observedProcesses;

        /// <summary>
        /// Запуск мониторинга
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Start(CancellationToken cancellationToken)
        {
            //запускаем постоянное обновление состояний
            RunStateRefresher(cancellationToken);

            //запускаем добавление новых процессов для наблюдения
            return RunAddingNewObservedProcesses(cancellationToken);
        }

        /// <summary>
        /// Остановить мониторинг
        /// </summary>
        public void Stop()
        {
            MonitorSystemInfo.OnHighLoaded -= OnHighLoaded;

            foreach (var observedProcess in _observedProcesses)
                CloseProcessObserver(processName: observedProcess.Key);
        }

        /// <summary>
        /// Получить состояния процессов
        /// </summary>
        public IReadOnlyCollection<ProcessState> GetProcessStates()
            => _observedProcesses.Values
                .Select(x => x.ProcessState)
                .ToList()
                .AsReadOnly();

        /// <summary>
        /// Завершение процесса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseProcess(object sender, EventArgs e)
        {
            var processName = sender switch
            {
                TProcessStateHandler handler => handler.ProcessState.ProcessName,
                Process process => process.ProcessName,
                _ => throw new ArgumentException(
                    $"Не предусмотрена обработка типа: {sender.GetType().Name} при завершении наблюдения за процессом")
            };

            // Убираем процесс из наблюдения
            CloseProcessObserver(processName);
        }

        /// <summary>
        /// Завершить наблюдение за процессом
        /// </summary>
        /// <param name="processName">Наименование процесса</param>
        private void CloseProcessObserver(string processName)
        {
            // Убираем процесс из наблюдения
            if (!_observedProcesses.TryRemove(processName, out var processHandler))
                return;

            processHandler.OnStateRefreshed -= OnProcessStateUpdated;
            processHandler.OnClosed -= OnCloseProcess;
            //оповещаем о завершении наблюдения за процессом
            OnProcessObserveClosed?.Invoke(processHandler.ProcessState.ProcessName, EventArgs.Empty);
        }

        /// <summary>
        /// Запустить процесс обновления состояния процессов
        /// </summary>
        /// <param name="cancellationToken"></param>
        private void RunStateRefresher(CancellationToken cancellationToken)
            => Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                    foreach (var process in _observedProcesses)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        process.Value.RefreshState();
                        MonitorSystemInfo.Refresh();
                    }
            });

        /// <summary>
        /// Запустить добавление новых процессов для мониторинга
        /// </summary>
        /// <param name="cancellationToken"></param>
        private Task RunAddingNewObservedProcesses(CancellationToken cancellationToken)
            => Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // получаем список имен активных процессов 
                    var activeProcessNames = _observedProcesses.Values.Select(s => s.ProcessState.ProcessName);
                    // берем только новые
                    var newProcesses = Process.GetProcesses()
                        .Where(x => !activeProcessNames.Contains(x.ProcessName));

                    foreach (var currentProcess in newProcesses)
                    {
                        // создаем обработчик процесса
                        var processHandler = new TProcessStateHandler()
                            .Initialize(currentProcess.Id, currentProcess.ProcessName);

                        if (!_observedProcesses.TryAdd(currentProcess.ProcessName, processHandler))
                            continue;

                        // оповещаем о старте наблюдения за процессом
                        OnProcessObserveOpened?.Invoke(processHandler.ProcessState, EventArgs.Empty);

                        // подписываемся на событие обновления состояния процесса
                        processHandler.OnStateRefreshed += OnProcessStateUpdated;

                        // подписываемся на события для завершение наблюдения 
                        processHandler.OnClosed += OnCloseProcess;
                        currentProcess.Disposed += OnCloseProcess;
                    }
                }
            });
    }
}