using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Model;

namespace SM.Domain.Interfaces
{
    public interface IMonitorService
    {
        /// <summary>
        /// Запуск мониторинга
        /// </summary>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Остановить мониторинг
        /// </summary>
        void Stop();

        /// <summary>
        /// Получить состояния процессов
        /// </summary>
        IReadOnlyCollection<ProcessState> GetProcessStates();
    }
}