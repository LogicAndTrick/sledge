using System;
using System.Text;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public interface IHistoryItem : IDisposable
    {
        string Name { get; }
        void Undo(Map map);
        void Redo(Map map);
    }
}
