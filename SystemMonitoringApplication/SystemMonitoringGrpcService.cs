using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using SM.Domain.Interfaces;

namespace SystemMonitoringApplication
{
    public class SystemMonitoringGrpcService : ProcessMonitor.ProcessMonitorBase
    {
        private readonly IMonitorService _monitorService;

        public SystemMonitoringGrpcService(IMonitorService monitorService)
        {
            _monitorService = monitorService;
        }

        public override Task<ProcessStates> GetProcessStates(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new ProcessStates
            {
                ProcessStates_ =
                {
                    _monitorService.GetProcessStates().Select(x => new ProcessStates.Types.ProcessState
                    {
                        ProcessId = x.ProcessId,
                        ProcessName = x.ProcessName,
                        Cpu = x.Cpu,
                        Memory = x.Memory
                    })
                }
            });
        }
    }
}