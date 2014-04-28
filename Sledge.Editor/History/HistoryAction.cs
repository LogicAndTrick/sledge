using System.Collections.Generic;
using System.Linq;
using Sledge.Editor.Actions;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    public class HistoryAction : IHistoryItem
    {
        private readonly List<IAction> _actions;

        public string Name { get; private set; }
        public bool SkipInStack { get; private set; }
        public bool ModifiesState { get; private set; }

        public HistoryAction(string name, params IAction[] actions)
        {
            Name = name;
            _actions = actions.ToList();
            SkipInStack = actions.All(x => x.SkipInStack);
            ModifiesState = actions.Any(x => x.ModifiesState);
        }

        public void Undo(Document document)
        {
            for (var i = _actions.Count - 1; i >= 0; i--)
            {
                _actions[i].Reverse(document);
            }
        }

        public void Redo(Document document)
        {
            _actions.ForEach(x => x.Perform(document));
        }

        public void Dispose()
        {
            _actions.ForEach(x => x.Dispose());
            _actions.Clear();
        }
    }
}