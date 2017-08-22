using System.Diagnostics;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Compile
{
    /// <summary>
    /// A process to run as part of a batch
    /// </summary>
    public class BatchProcess : BatchStep
    {
        public string Process { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; } = "{WorkingDirectory}";
        public bool InterceptOutput { get; set; } = true;

        public BatchProcess(string process, string arguments)
        {
            Process = process;
            Arguments = arguments;
        }

        public override async Task Run(Batch batch, MapDocument document)
        {
            var pcs = Process;
            var args = Arguments;
            var wd = WorkingDirectory;

            // Replace {Variables} in the strings
            foreach (var kv in batch.Variables)
            {
                var s = '{' + kv.Key + '}';
                pcs = pcs.Replace(s, kv.Value);
                args = args.Replace(s, kv.Value);
                wd = wd.Replace(s, kv.Value);
            }

            await Oy.Publish("Compile:Information", $"{pcs} {args}\r\n");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(pcs, args)
                {
                    CreateNoWindow = InterceptOutput,
                    UseShellExecute = !InterceptOutput,
                    RedirectStandardOutput = InterceptOutput,
                    RedirectStandardError = InterceptOutput,
                    WorkingDirectory = wd
                },
                EnableRaisingEvents = true
            };

            // Use a task to signal process completion
            var tcs = new TaskCompletionSource<bool>();
            process.Exited += (s, e) =>
            {
                tcs.SetResult(true);
                process.Dispose();
            };

            // Listen to events if we're intercepting 
            if (InterceptOutput)
            {
                process.OutputDataReceived += (sender, a) => Oy.Publish("Compile:Output", a.Data + "\r\n");
                process.ErrorDataReceived += (sender, a) => Oy.Publish("Compile:Error", a.Data + "\r\n");
            }

            process.Start();

            if (InterceptOutput)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            await tcs.Task;
        }
    }
}