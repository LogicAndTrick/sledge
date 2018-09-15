using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.Common;
using Sledge.Common.Translations;

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
        public override BatchStepType StepType { get; }

        public BatchProcess(BatchStepType stepType, string process, string arguments)
        {
            Process = process;
            Arguments = arguments;
            StepType = stepType;
        }
        
        public override async Task Run(Batch batch, MapDocument document)
        {
            var pcs = Process;
            var args = Arguments;
            var wd = WorkingDirectory;

            if (!File.Exists(pcs))
            {
                // Only show this notice once at most
                if (batch.Successful)
                {
                    var tlate = Container.Get<ITranslationStringProvider>();
                    var prefix = GetType().FullName;
                    MessageBox.Show(tlate.GetString(prefix, "ProgramNotFoundMessage"), tlate.GetString(prefix, "ProgramNotFoundTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                batch.Successful = false;
                await Oy.Publish("Compile:Error", "Process not found: " + pcs + "\n");
                return;
            }

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