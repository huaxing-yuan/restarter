using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace restarter
{
    static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            /* Arguments:
             * restart
             * >    restarter.exe -i <ProcessName> <program.exe> [timeout]
             * >    restarter.exe -p <processId> <program.exe> [timeout]
             * */
            try
            {
                string st = args[0];
                string timeout = args.Length >= 4 ? args[3] : "10";
                switch (st)
                {
                    case "-i":
                        RestartByImage(args[1], args[2], timeout);
                        break;
                    case "-p":
                        RestartByProcessId(args[1], args[2], timeout);
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("There is an error: " + ex.Message);
                printUsage();
            }

        }

        private static void RestartByProcessId(string pid, string program, string timeout)
        {
            DateTime waitUntil = DateTime.Now.AddSeconds(int.Parse(timeout));
            var p = Process.GetProcessById(int.Parse(pid));
            if(! p.HasExited && DateTime.Now <= waitUntil)
            {
                Thread.Sleep(1000);
            }
            Process.Start(program);
        }

        private static void RestartByImage(string imageName, string program, string timeout)
        {
            DateTime waitUntil = DateTime.Now.AddSeconds(int.Parse(timeout));
            while(System.Diagnostics.Process.GetProcessesByName(imageName).Length > 0 && DateTime.Now <= waitUntil)
            {
                Console.WriteLine("Waiting for the application to exit...");
                Thread.Sleep(1000);
            }
            foreach(var p in System.Diagnostics.Process.GetProcessesByName(imageName))
            {
                Console.WriteLine("Force close the application to exit");
                p.Kill();
            }
            Process.Start(program);

        }

        static void printUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\t> restarter -i <ImageName> [timeout]");
            Console.WriteLine("\t> restarter -p <ProcessId> [timeout]");

            Console.WriteLine("ImageName:   The image name (process name) of the application.");
            Console.WriteLine("ProcessId:   The PID of the application.");
            Console.WriteLine("timeout:     The time in second to wait until the program will force close the given process");
        }
    }
}
