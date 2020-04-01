using SM.Core;
using SM.Model;

namespace SM.Contracts.Events
{
    /// <summary>
    /// Событие об обновлении состояния процесса
    /// </summary>
    public class ProcessStateUpdateEvent : EventMessage
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ProcessStateUpdateEvent(ProcessState processState) => ProcessState = processState;

        /// <summary>
        /// Состояние процесса
        /// </summary>
        public ProcessState ProcessState { get; set; }
    }
}
