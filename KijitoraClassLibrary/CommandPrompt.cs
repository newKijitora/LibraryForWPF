using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijitoraClassLibrary
{
    public sealed class CommandPrompt
    {
        public void Execute(CommandLine commandLine)
        {
            Execute(commandLine as IEnumerable<string>);
        }

        public void Execute(IEnumerable<string> commandLine)
        {
            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
                process.StartInfo.CreateNoWindow = true;
                process.OutputDataReceived += OutputDataReceived;
                process.Start();
                process.BeginOutputReadLine();
                using (var writer = process.StandardInput)
                {
                    if (writer.BaseStream.CanWrite)
                    {
                        foreach (var command in commandLine)
                        {
                            writer.WriteLine(command);
                        }
                        writer.WriteLine("exit");
                    }
                    process.WaitForExit();
                }
            }
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
