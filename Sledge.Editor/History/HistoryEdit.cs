using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.History
{
    /// <summary>
    /// The edit is an action that neither creates nor deletes objects, it merely modifies their data.
    /// </summary>
    public class HistoryEdit : IHistoryItem
    {
        public string Name { get; private set; }

        private readonly List<Tuple<MapObject, MapObject, MapObject>> _changedObjects;

        /// <summary>
        /// Create an edit history item. The two object lists must be in the same order.
        /// </summary>
        /// <param name="name">The name of the history item</param>
        /// <param name="beforeEdit">The list of objects before they were edited</param>
        /// <param name="current">The list of objects as they are now, i.e. after they were edited.</param>
        public HistoryEdit(string name, IEnumerable<MapObject> beforeEdit, IEnumerable<MapObject> current)
        {
            Name = name;
            _changedObjects = new List<Tuple<MapObject, MapObject, MapObject>>();
            var be = beforeEdit.ToList();
            var cu = current.ToList();
            if (be.Count != cu.Count) throw new Exception("Both lists must be the same length.");
            var idg = new IDGenerator(); // These clones will never be added into the document tree, don't care about their ids
            for (var i = 0; i < be.Count; i++)
            {
                _changedObjects.Add(Tuple.Create(cu[i], be[i].Clone(idg), cu[i].Clone(idg)));
            }
        }

        public void Undo(Document document)
        {
            _changedObjects.ForEach(x => x.Item1.Unclone(x.Item2, document.Map.IDGenerator));
        }

        public void Redo(Document document)
        {
            _changedObjects.ForEach(x => x.Item1.Unclone(x.Item3, document.Map.IDGenerator));
        }

        public void Dispose()
        {
            _changedObjects.Clear();
        }
    }
}