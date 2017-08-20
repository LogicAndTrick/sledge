using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Compile
{
    public class Batch
    {
        public List<BatchStep> Steps { get; set; }
        public Dictionary<string, string> Variables { get; set; }

        public Batch()
        {
            Steps = new List<BatchStep>();
            Variables = new Dictionary<string, string>();
        }

        public async Task Run(MapDocument document)
        {
            foreach (var step in Steps)
            {
                await step.Run(this, document);
            }
        }
    }

    public class BatchArgument
    {
        public string Name { get; set; }
        public string Arguments { get; set; }
    }

    /// <summary>
    /// A step to run as part of a batch
    /// </summary>
    public abstract class BatchStep
    {
        public abstract Task Run(Batch batch, MapDocument document);
    }

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

    /// <summary>
    /// A callback to run as part of a batch
    /// </summary>
    public class BatchCallback : BatchStep
    {
        private readonly Func<Batch, MapDocument, Task> _callback;

        public BatchCallback(Func<Batch, MapDocument, Task> callback)
        {
            _callback = callback;
        }

        public override Task Run(Batch batch, MapDocument document)
        {
            return _callback(batch, document);
        }
    }
}
