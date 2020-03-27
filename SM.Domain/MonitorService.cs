using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Domain
{
    /// <summary>
    /// Сервис мониторинга процессов и состояния системы
    /// </summary>
    public class MonitorService
    {
        public MonitorService()
        {
            ObservedProcesses = new ConcurrentDictionary<string, ProcessStateHandler>();
            MonitorSystemInfo = new MonitorSystemInfo();
            MonitorSystemInfo.OnHighLoaded += OnHighLoaded;
        }

        /// <summary>
        ///  Список отслеживаемых процессов
        /// </summary>
        public readonly ConcurrentDictionary<string, ProcessStateHandler> ObservedProcesses;

        /// <summary>
        /// Системная информация
        /// </summary>
        public MonitorSystemInfo MonitorSystemInfo;

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

        public void Run(CancellationToken cancellationToken)
        {   //запускаем постоянное обновление состояний в фоновой задаче
            Task.Run(() => RunStateRefresher(cancellationToken));

            while (!cancellationToken.IsCancellationRequested)
            {   // получаем список имен активных процессов 
                var activeProcessNames = ObservedProcesses.Values.Select(s => s.ProcessState.ProcessName);
                // берем только новые
                var newProcesses = Process.GetProcesses()
                    .Where(x => !activeProcessNames.Contains(x.ProcessName));

                foreach (var currentProcess in newProcesses)
                {
                    // создаем обаботчик процесса
                    var processHandler = new ProcessStateHandler(currentProcess);

                    if (!ObservedProcesses.TryAdd(currentProcess.ProcessName, processHandler))
                        continue;

                    // оповещаем о старте наблюдения за процессом
                    OnProcessObserveOpened?.Invoke(processHandler.ProcessState, EventArgs.Empty);

                    // подписываемся на событие обновления состояния процесса
                    processHandler.OnStateRefreshed += OnProcessStateUpdated;

                    // подписываемся на события для завершение наблюдения 
                    processHandler.OnClosed += CloseProcessObserver;
                    currentProcess.Disposed += CloseProcessObserver;
                }
            }
        }

        /// <summary>
        /// Завершить наблюдение за процессом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseProcessObserver(object sender, EventArgs e)
        {
            var processName = sender switch
            {
                ProcessStateHandler handler => handler.ProcessState.ProcessName,
                Process process => process.ProcessName,
                _ => throw new ArgumentException($"Не предусмотрена обработка типа: {sender.GetType().Name} при завершении наблюдения за процессом")
            };

            // Убираем процесс из наблюдения
            if (!ObservedProcesses.TryRemove(processName, out var processHandler))
                return;

            processHandler.OnStateRefreshed -= OnProcessStateUpdated;
            processHandler.OnClosed -= CloseProcessObserver;
            //оповещаем о завершении наблюдения за процессом
            OnProcessObserveClosed?.Invoke(processHandler.ProcessState.ProcessName, EventArgs.Empty);
        }

        /// <summary>
        /// Запустить процесс обновления состояния процессов
        /// </summary>
        /// <param name="cancellationToken"></param>
        private void RunStateRefresher(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
                foreach (var process in ObservedProcesses)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    process.Value.RefreshState();
                    MonitorSystemInfo.Refresh();
                }
        }
    }
}