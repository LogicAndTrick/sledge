using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Removes the given objects from the map, and from the selection if required.
    /// Reverse: Adds the given objects back to their original parents, and reselects them if required.
    /// </summary>
    public class Delete : IAction
    {
        private class DeleteReference
        {
            public long ParentID { get; set; }
            public bool IsSelected { get; set; }
            public MapObject Object { get; set; }
            public bool TopMost { get; set; }

            public DeleteReference(MapObject o, long parentID, bool isSelected, bool topMost)
            {
                Object = o;
                ParentID = parentID;
                IsSelected = isSelected;
                TopMost = topMost;
            }
        }

        private List<long> _ids;
        private List<DeleteReference> _objects;

        public Delete(IEnumerable<long> ids)
        {
            _ids = ids.ToList();
        }

        public void Dispose()
        {
            _ids = null;
            _objects = null;
        }

        public void Reverse(Document document)
        {
            _ids = _objects.Select(x => x.Object.ID).ToList();
            foreach (var dr in _objects.Where(x => x.TopMost))
            {
                dr.Object.SetParent(document.Map.WorldSpawn.FindByID(dr.ParentID));
            }
            document.Selection.Select(_objects.Where(x => x.IsSelected).Select(x => x.Object));
            _objects = null;
        }

        public void Perform(Document document)
        {
            var objects = document.Map.WorldSpawn.Find(x => _ids.Contains(x.ID) && x.Parent != null);
            _objects = objects.Select(x => new DeleteReference(x, x.Parent.ID, x.IsSelected, !objects.Contains(x.Parent))).ToList();
            document.Selection.Deselect(objects);
            foreach (var dr in _objects.Where(x => x.TopMost))
            {
                dr.Object.SetParent(null);
            }
            _ids = null;
        }
    }
}
