using System;

namespace Ap.CorrAnalysis.ConsoleHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var host = new Host();
            host.Bootstraper.Start();
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            host.Bootstraper.Stop();
        }
    }
}