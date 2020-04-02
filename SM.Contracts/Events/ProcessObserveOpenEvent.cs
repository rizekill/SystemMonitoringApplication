using SM.Core;
using SM.Model;

namespace SM.Contracts.Events
{
    /// <summary>
    /// Событие о начале наблюдения за процессом
    /// </summary>
    public class ProcessObserveOpenEvent : EventMessage
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ProcessObserveOpenEvent(ProcessState processState) => ProcessState = processState;

        /// <summary>
        /// Состояние процесса
        /// </summary>
        public ProcessState ProcessState { get; set; }
    }
}
