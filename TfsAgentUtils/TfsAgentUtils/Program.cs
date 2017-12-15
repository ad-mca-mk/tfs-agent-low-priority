using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TfsAgentUtils
{
    public class Program
    {
        static void Main()
        {
            Process[] agentProcesses = GetTfsAgentProcesses();

            if (agentProcesses.Length == 0)
            {
                Console.WriteLine("TFS Agent is not found");
                return;
            }

            ReduceProcessTreePriority(agentProcesses);
        }

        private static Process[] GetTfsAgentProcesses()
        {
            Process[] agentProcess = null;
            DateTime timeout = DateTime.Now.AddMinutes(1);

            do
            {
                agentProcess = Process.GetProcessesByName("AgentService");

            } while (agentProcess.Length == 0 || timeout <= DateTime.Now);
            return agentProcess;
        }

        private static void ReduceProcessTreePriority(Process[] agentProcesses)
        {
            foreach (var thisProcess in agentProcesses)
            {
                Console.WriteLine($"Success: {thisProcess.ProcessName} ({thisProcess.Id}) set to Lower Priority");
                try
                {
                    thisProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                }
                catch
                {
                    Console.WriteLine($"Fail: {thisProcess.ProcessName} ({thisProcess.Id.ToString()}) set to Lower Priority");
                }

                List<Process> childProcesses = new List<Process>();

                foreach (var childProcess in Process.GetProcesses())
                {
                    try
                    {
                        if (childProcess.Parent().Id == thisProcess.Id)
                        {
                            childProcesses.Add(childProcess);
                        }
                    }
                    catch
                    {
                        // No need to notify for system processes
                    }
                }

                ReduceProcessTreePriority(childProcesses.ToArray());

            }
        }

    }
}
