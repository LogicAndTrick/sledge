using System;
using System.Text;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public interface IHistoryItem : IDisposable
    {
        void Undo(Map map);
        void Redo(Map map);
    }
}
