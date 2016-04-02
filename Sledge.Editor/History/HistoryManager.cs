using System.Collections.Generic;
using System.Linq;
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
            _stacks.Push(new HistoryStack("base", Sledge.Settings.Select.UndoStackSize));
        }

        public void AddHistoryItem(IHistoryItem item)
        {
            var stack = _stacks.Peek();
            stack.Add(item);

            if (_stacks.Count == 1 && item.ModifiesState)
            {
                TotalActionsSinceLastSave++;
                TotalActionsSinceLastAutoSave++;
            }
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public void PushStack(string name)
        {
            _stacks.Push(new HistoryStack(name, 100));
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public void PopStack()
        {
            var popped = _stacks.Pop();
            var stack = _stacks.Peek();

            foreach (var item in popped.GetHistoryItems().Where(x => !x.DiscardInStack))
            {
                stack.Add(item);

                if (_stacks.Count == 1 && item.ModifiesState)
                {
                    TotalActionsSinceLastSave++;
                    TotalActionsSinceLastAutoSave++;
                }
            }
            
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public void Undo()
        {
            if (!CanUndo()) return;

            var modifiesCount = 0;

            while (CanUndo())
            {
                var u = _stacks.Peek().NextUndo();
                if (u.ModifiesState) modifiesCount++;
                _stacks.Peek().Undo(_document);
                if (!u.SkipInStack) break;
            }

            if (_stacks.Count == 1)
            {
                TotalActionsSinceLastSave -= modifiesCount;
                TotalActionsSinceLastAutoSave -= modifiesCount;
            }
            Mediator.Publish(EditorMediator.HistoryChanged);
        }

        public void Redo()
        {
            if (!CanRedo()) return;

            var modifiesCount = _stacks.Peek().NextRedo().ModifiesState ? 1 : 0;
            _stacks.Peek().Redo(_document);

            while (CanRedo())
            {
                var r = _stacks.Peek().NextRedo();
                if (!r.SkipInStack) break;
                if (r.ModifiesState) modifiesCount++;
                _stacks.Peek().Redo(_document);
            }

            if (_stacks.Count == 1)
            {
                TotalActionsSinceLastSave += modifiesCount;
                TotalActionsSinceLastAutoSave += modifiesCount;
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

        public IEnumerable<HistoryStack> GetHistoryStacks()
        {
            return new List<HistoryStack>(_stacks);
        }
    }
}
