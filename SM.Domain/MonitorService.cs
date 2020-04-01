using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SM.Contracts.Events;
using SM.Core;
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
        public MonitorService(IMessageProcessor messageProcessor)
        {
            _messageProcessor = messageProcessor;
            _observedProcesses = new ConcurrentDictionary<string, ProcessStateHandlerBase>();
            MonitorSystemInfo = new TMonitorSystemInfo();

        }

        /// <summary>
        /// Системная информация
        /// </summary>
        public TMonitorSystemInfo MonitorSystemInfo;

        /// <summary>
        /// Обработчик сообщений
        /// </summary>
        private readonly IMessageProcessor _messageProcessor;

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
            MonitorSystemInfo.OnHighLoaded += OnHighLoad;

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
            MonitorSystemInfo.OnHighLoaded -= OnHighLoad;

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
                        _messageProcessor.Publish(new ProcessObserveOpenEvent(processHandler.ProcessState));

                        // подписываемся на событие обновления состояния процесса
                        processHandler.OnStateRefreshed += OnProcessStateUpdate;

                        // подписываемся на события для завершение наблюдения 
                        processHandler.OnClosed += OnCloseProcess;
                        currentProcess.Disposed += OnCloseProcess;
                    }
                }
            });

        ///// <summary>
        ///// Срабатывает при появлении высокой нагрузки 
        ///// </summary>
        private void OnHighLoad(object sender, EventArgs e)
        {
            if (sender is TMonitorSystemInfo monitorSystemInfo)
                _messageProcessor.Publish(new HighLoadEvent(monitorSystemInfo.SystemInfo));
        }

        ///// <summary>
        ///// Срабатывает при обновлении состояния процесса
        ///// </summary>
        private void OnProcessStateUpdate(object sender, EventArgs e)
        {
            if (sender is TProcessStateHandler processStateHandler)
                _messageProcessor.Publish(new ProcessStateUpdateEvent(processStateHandler.ProcessState));
        }

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

            processHandler.OnStateRefreshed -= OnProcessStateUpdate;
            processHandler.OnClosed -= OnCloseProcess;
            //оповещаем о завершении наблюдения за процессом
            _messageProcessor.Publish(new ProcessObserveCompleteEvent(processHandler.ProcessState));
        }
    }
}