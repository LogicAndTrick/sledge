using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Modification
{
    public class Transaction : IOperation
    {
        private readonly List<IOperation> _operations;
        public bool Trivial => _operations.All(x => x.Trivial);
        public bool IsEmpty => !_operations.Any();

        public Transaction(params IOperation[] operations) : this(operations.ToList())
        {
        }

        public Transaction(IEnumerable<IOperation> operations)
        {
            _operations = operations.ToList();
        }

        public void Add(IOperation operation)
        {
            _operations.Add(operation);
        }

        public void AddRange(IEnumerable<IOperation> operations)
        {
            _operations.AddRange(operations);
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);
            foreach (var operation in _operations)
            {
                ch.Merge(await operation.Perform(document));
            }
            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);
            for (var i = _operations.Count - 1; i >= 0; i--)
            {
                var operation = _operations[i];
                ch.Merge(await operation.Reverse(document));
            }
            return ch;
        }
    }
}
