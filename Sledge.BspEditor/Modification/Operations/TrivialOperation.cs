using System;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Modification.Operations
{
    /// <summary>
    /// An irreversable operation that doesn't modify meaningful persisted state.
    /// </summary>
    public class TrivialOperation : IOperation
    {
        public bool Trivial => true;

        private Func<MapDocument, Task> _action;
        private Action<Change> _change;

        public TrivialOperation(Action<MapDocument> action, Action<Change> change)
        {
            _action = x =>
            {
                action(x);
                return Task.FromResult(0);
            };
            _change = change;
        }

        public TrivialOperation(Func<MapDocument, Task> action, Action<Change> change)
        {
            _action = action;
            _change = change;
        }

        public TrivialOperation(IOperation operation)
        {
            _action = async x =>
            {
                var ch = await operation.Perform(x);
                _change = c => c.Merge(ch);
            };
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var change = new Change(document);
            await _action(document);
            _change(change);
            return change;
        }

        public Task<Change> Reverse(MapDocument document)
        {
            throw new NotSupportedException("A trivial operation is irreversable.");
        }
    }
}
