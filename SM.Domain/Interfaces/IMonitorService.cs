using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Domain.Interfaces
{
    public interface IMonitorService
    {
        /// <summary>
        /// Срабатывает при появлении высокой нагрузки 
        /// </summary>
        event EventHandler OnHighLoaded;

        /// <summary>
        /// Срабатывает при старте наблюдения за процессом
        /// </summary>
        event EventHandler OnProcessObserveOpened;

        /// <summary>
        /// Срабатывает при обновлении состояния процесса
        /// </summary>
        event EventHandler OnProcessStateUpdated;

        /// <summary>
        /// Срабатывает при завершении наблюдения за процессом
        /// </summary>
        event EventHandler OnProcessObserveClosed;

        /// <summary>
        /// Запуск мониторинга
        /// </summary>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Остановить мониторинг
        /// </summary>
        void Stop();
    }
}