using System;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    public interface IHistoryItem : IDisposable
    {
        string Name { get; }
        bool SkipInStack { get; }
        bool ModifiesState { get; }
        bool DiscardInStack { get; }
        void Undo(Document document);
        void Redo(Document document);
    }
}
