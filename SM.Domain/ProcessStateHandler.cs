using System;
using System.Diagnostics;
using SM.Model;

namespace SM.Domain
{
    /// <summary>
    /// Обработчик состояния процесса
    /// </summary>
    public class ProcessStateHandler
    {
        public ProcessStateHandler(Process process)
        {
            ProcessState = new ProcessState(process.Id, process.ProcessName);
            _cpuPerformanceCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
            _ramPerformanceCounter = new PerformanceCounter("Process", "Private Bytes", process.ProcessName, true);
        }

        /// <summary>
        /// Событие при обновлении состояния
        /// </summary>
        public event EventHandler OnStateRefreshed;

        /// <summary>
        /// Событие при завершении наблюдения за процессом
        /// </summary>
        public event EventHandler OnClosed;

        /// <summary>
        /// Состояние процесса
        /// </summary>
        public ProcessState ProcessState { get; set; }

        /// <summary>
        /// Обновить состояние процесса
        /// </summary>
        public void RefreshState()
        {
            try
            {
                ProcessState.Cpu = Math.Round(_cpuPerformanceCounter.NextValue() / Environment.ProcessorCount, 2);
                //приводим к Mb
                ProcessState.Memory = Math.Round(_ramPerformanceCounter.NextValue() / 1024 / 1024, 2);

                OnStateRefreshed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                OnClosed?.Invoke(this, EventArgs.Empty);
            }
        }

        private readonly PerformanceCounter _cpuPerformanceCounter;
        private readonly PerformanceCounter _ramPerformanceCounter;
    }
}