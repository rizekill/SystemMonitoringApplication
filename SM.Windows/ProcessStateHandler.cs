using System;
using System.Diagnostics;
using System.IO;
using SM.Domain;

namespace SM.Windows
{
    /// <summary>
    /// Обработчик состояния процесса
    /// </summary>
    public class ProcessStateHandler : ProcessStateHandlerBase
    {
        public override ProcessStateHandlerBase Initialize(int id, string processName)
        {
            _cpuPerformanceCounter = new PerformanceCounter("Process", "% Processor Time", processName, true);
            _ramPerformanceCounter = new PerformanceCounter("Process", "Private Bytes", processName, true);

            base.Initialize(id, processName);

            return this;
        }

        /// <summary>
        /// Событие при обновлении состояния
        /// </summary>
        public override event EventHandler OnStateRefreshed;

        /// <summary>
        /// Событие при завершении наблюдения за процессом
        /// </summary>
        public override event EventHandler OnClosed;

        /// <summary>
        /// Обновить состояние процесса
        /// </summary>
        public override void RefreshState()
        {
            try
            {
                ProcessState.Cpu = Math.Round((double)(_cpuPerformanceCounter.NextValue() / Environment.ProcessorCount), 2);
                //приводим к Mb
                ProcessState.Memory = Math.Round((double)(_ramPerformanceCounter.NextValue() / 1024 / 1024), 2);

                OnStateRefreshed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                OnClosed?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        private PerformanceCounter _cpuPerformanceCounter;
        private PerformanceCounter _ramPerformanceCounter;
    }
}