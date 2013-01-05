using System;
using System.Text;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    public interface IHistoryItem : IDisposable
    {
        string Name { get; }
        void Undo(Document document);
        void Redo(Document document);
    }
}
