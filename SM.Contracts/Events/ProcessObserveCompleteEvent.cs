using SM.Core;
using SM.Model;

namespace SM.Contracts.Events
{
    /// <summary>
    /// Событие о завершении наблюдения за процессом
    /// </summary>
    public class ProcessObserveCompleteEvent : EventMessage
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ProcessObserveCompleteEvent(ProcessState processState) => ProcessState = processState;

        /// <summary>
        /// Состояние процесса
        /// </summary>
        public ProcessState ProcessState { get; set; }
    }
}
