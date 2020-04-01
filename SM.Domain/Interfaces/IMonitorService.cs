using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Model;

namespace SM.Domain.Interfaces
{
    public interface IMonitorService
    {
        /// <summary>
        /// ������ �����������
        /// </summary>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// ���������� ����������
        /// </summary>
        void Stop();

        /// <summary>
        /// �������� ��������� ���������
        /// </summary>
        IReadOnlyCollection<ProcessState> GetProcessStates();
    }
}