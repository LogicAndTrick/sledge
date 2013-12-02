using System.Collections.Generic;
using Sledge.Common.Mediator;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    public class HistoryManager
    {
        private readonly Document _document;
        private readonly Stack<HistoryStack> _stacks; 

        public long TotalActionsSinceLastSave { get; set; }
        public long TotalActionsSinceLastAutoSave { get; set; }

        public HistoryManager(Document doc)
        {
            _document = doc;
            _stacks = new Stack<HistoryStack>();
            _stacks.Push(new HistoryStack("base", 100));
        }

        public void AddHistoryItem(IHistoryItem item)
        {
            var stack = _stacks.Peek();
            stack.Add(item);

            if (_stacks.Count == 1)
            {
                TotalActionsSinceLastSave++;
                TotalActionsSinceLastAutoSave++;
            }
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public void PushStack(string name)
        {
            _stacks.Push(new HistoryStack(name, 100));
        }

        public void PopStack(IHistoryItem action = null)
        {
            _stacks.Pop();
            if (action != null)
            {
                AddHistoryItem(action);
            }
        }

        public void Undo()
        {
            if (!CanUndo()) return;
            _stacks.Peek().Undo(_document);
            if (_stacks.Count == 1)
            {
                TotalActionsSinceLastSave--;
                TotalActionsSinceLastAutoSave--;
            }
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public void Redo()
        {
            if (!CanRedo()) return;
            _stacks.Peek().Redo(_document);
            if (_stacks.Count == 1)
            {
                TotalActionsSinceLastSave++;
                TotalActionsSinceLastAutoSave++;
            }
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public string GetUndoString()
        {
            return _stacks.Peek().GetUndoString();
        }

        public string GetRedoString()
        {
            return _stacks.Peek().GetRedoString();
        }

        public bool CanUndo()
        {
            return _stacks.Peek().CanUndo();
        }

        public bool CanRedo()
        {
            return _stacks.Peek().CanRedo();
        }
    }
}
