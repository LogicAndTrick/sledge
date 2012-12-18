using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.History
{
    public class HistoryEdit : IHistoryItem
    {
        private readonly List<Tuple<MapObject, MapObject, MapObject>> _changedObjects;

        public HistoryEdit(IEnumerable<MapObject> beforeEdit, IEnumerable<MapObject> current)
        {
            _changedObjects = new List<Tuple<MapObject, MapObject, MapObject>>();
            var be = beforeEdit.ToList();
            var cu = current.ToList();
            if (be.Count != cu.Count) throw new Exception("Both lists must be the same length.");
            for (var i = 0; i < be.Count; i++)
            {
                _changedObjects.Add(Tuple.Create(cu[i], be[i].Clone(), cu[i].Clone()));
            }
        }

        public void Undo(Map map)
        {
            _changedObjects.ForEach(x => x.Item1.Unclone(x.Item2));
        }

        public void Redo(Map map)
        {
            _changedObjects.ForEach(x => x.Item1.Unclone(x.Item3));
        }

        public void Dispose()
        {
            _changedObjects.Clear();
        }
    }
}