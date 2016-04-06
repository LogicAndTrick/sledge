using Sledge.Editor.Documents;
using Sledge.Editor.History;

namespace Sledge.Editor.Tools.VMTool.Actions
{
    public abstract class VMAction : IHistoryItem
    {
        private readonly VMTool _tool;

        public abstract string Name { get; }
        public abstract bool SkipInStack { get; }
        public abstract bool ModifiesState { get; }
        public bool DiscardInStack { get { return true; } }

        protected VMAction(VMTool tool)
        {
            _tool = tool;
        }

        public void Undo(Document document)
        {
            Reverse(_tool);
        }

        public void Redo(Document document)
        {
            Perform(_tool);
        }

        protected abstract void Reverse(VMTool tool);
        protected abstract void Perform(VMTool tool);
        public abstract void Dispose();
    }
}