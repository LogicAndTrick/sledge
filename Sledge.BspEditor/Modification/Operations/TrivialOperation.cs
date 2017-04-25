using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private Action<MapDocument> _action;
        private Action<Change> _change;

        public TrivialOperation(Action<MapDocument> action, Action<Change> change)
        {
            _action = action;
            _change = change;
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var change = new Change(document);
            _action(document);
            _change(change);
            return change;
        }

        public Task<Change> Reverse(MapDocument document)
        {
            throw new NotSupportedException("A trivial operation is irreversable.");
        }
    }
}
