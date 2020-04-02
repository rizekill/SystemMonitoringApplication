using System;
using System.Threading;

namespace HighLoadTest
{
    class Program
    {
        static void Main()
        {
            var source = new CancellationTokenSource();
            var proc = Environment.ProcessorCount * 90 / 100;
            for (var i = 0; i < proc; i++)
                new Thread(Function).Start(source.Token);

            Console.ReadLine();
            source.Cancel();
        }

        static void Function(object obj)
        {
            var token = (CancellationToken)obj;
            while (!token.IsCancellationRequested) { }
        }
    }
}
