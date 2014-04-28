using System.Collections.Generic;
using System.Linq;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions
{
    public class ActionCollection : IAction
    {
        private List<IAction> Actions { get; set; }

        public bool SkipInStack { get { return Actions.All(x => x.SkipInStack); } }
        public bool ModifiesState { get { return Actions.Any(x => x.ModifiesState); } }

        public ActionCollection(params IAction[] actions)
        {
            Actions = actions.ToList();
        }

        public void Add(params IAction[] actions)
        {
            Actions.AddRange(actions);
        }

        public bool IsEmpty()
        {
            return Actions.Count == 0;
        }

        public void Dispose()
        {
            Actions.ForEach(x => x.Dispose());
            Actions.Clear();
            Actions = null;
        }

        public void Reverse(Document document)
        {
            for (var i = Actions.Count - 1; i >= 0; i--)
            {
                Actions[i].Reverse(document);
            }
        }

        public void Perform(Document document)
        {
            Actions.ForEach(x => x.Perform(document));
        }
    }
}
