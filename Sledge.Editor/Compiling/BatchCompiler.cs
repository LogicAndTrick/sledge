using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Sledge.Editor.Compiling
{
    public static class BatchCompiler
    {
        public static void Compile(Batch batch)
        {
            /*
            var script = new StringBuilder();
            script.AppendLine(batch.BeforeExecute);
            foreach (var step in batch.Steps)
            {
                script.AppendLine(batch.BeforeExecuteStep);
                script.AppendLine(step.BeforeExecute);
                var cq = step.SystemCommand ? "" : "\"";
                script.Append(cq).Append(step.Operation).Append(cq).Append(' ').AppendLine(step.Flags);
                script.AppendLine(step.AfterExecute);
                script.AppendLine(batch.AfterExecuteStep);
            }
            script.AppendLine(batch.AfterExecute);
            var batchFile = Path.ChangeExtension(batch.TargetFile, "bat");
            File.WriteAllText(batchFile, script.ToString());
            Process.Start(batchFile);*/

            Task.Factory.StartNew(() => DoCompile(batch));
        }

        private static void DoCompile(Batch batch)
        {
            foreach (var step in batch.Steps)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(step.SystemCommand ? "cmd.exe" : step.Operation, step.SystemCommand ? step.Operation + " " + step.Flags : step.Flags)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        WorkingDirectory = Path.GetDirectoryName(batch.TargetFile)
                    }
                };
                var logger = new CompileLogTracer();
                process.OutputDataReceived += (sender, args) => logger.AddLine(args.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }
    }
}
