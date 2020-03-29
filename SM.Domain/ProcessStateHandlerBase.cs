using System;
using SM.Model;

namespace SM.Domain
{
    public abstract class ProcessStateHandlerBase
    {
        public virtual ProcessStateHandlerBase Initialize(int id, string processName)
        {
            ProcessState = new ProcessState(id, processName);

            return this;
        } 

        /// <summary>
        /// Событие при обновлении состояния
        /// </summary>
        public abstract event EventHandler OnStateRefreshed;

        /// <summary>
        /// Событие при завершении наблюдения за процессом
        /// </summary>
        public abstract event EventHandler OnClosed;

        /// <summary>
        /// Состояние процесса
        /// </summary>
        public ProcessState ProcessState { get; set; }

        /// <summary>
        /// Обновить состояние процесса
        /// </summary>
        public abstract void RefreshState();
    }
}