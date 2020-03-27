using System.Threading;
using SM.Domain;

namespace SystemMonitoringApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            new MonitorService().Run(cancellationTokenSource.Token);
        }
    }
}
