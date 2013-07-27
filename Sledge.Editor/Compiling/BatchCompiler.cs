using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Sledge.Editor.Compiling
{
    public static class BatchCompiler
    {
        public static void Compile(Batch batch)
        {
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
            Process.Start(batchFile);
        }
    }
}
