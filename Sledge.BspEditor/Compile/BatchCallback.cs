using System;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Compile
{
    /// <summary>
    /// A callback to run as part of a batch
    /// </summary>
    public class BatchCallback : BatchStep
    {
        public override BatchStepType StepType { get; }

        private readonly Func<Batch, MapDocument, Task> _callback;

        public BatchCallback(BatchStepType stepType, Func<Batch, MapDocument, Task> callback)
        {
            _callback = callback;
            StepType = stepType;
        }

        public override Task Run(Batch batch, MapDocument document)
        {
            return _callback(batch, document);
        }
    }
}