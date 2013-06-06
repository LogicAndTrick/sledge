using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Clips the given objects by the given plane, reselecting the new solids if needed.
    /// Reverse: Removes the clipped objects and restores the originals, reselecting if needed.
    /// </summary>
    public class Clip : IAction
    {
        private class ClipReference
        {
            public long ID { get; set; }
            public long ParentID { get; set; }
            public bool IsSelected { get; set; }

            public ClipReference(long id, long parentID, bool isSelected)
            {
                ID = id;
                ParentID = parentID;
                IsSelected = isSelected;
            }
        }

        private Plane _plane;
        private List<Solid> _objects;

        private List<ClipReference> _clipped;
        private List<long> _results;

        public Clip(List<Solid> objects, Plane plane)
        {
            _objects = objects;
            _plane = plane;
        }

        public void Dispose()
        {
            _plane = null;
            _objects = null;
            _clipped = null;
            _results = null;
        }

        public void Reverse(Document document)
        {
            // Remove and deselect the new objects
            var remove = document.Map.WorldSpawn.Find(x => _results.Contains(x.ID));
            if (remove.Any(x => x.IsSelected))
            {
                document.Selection.Deselect(remove.Where(x => x.IsSelected));
            }
            remove.ForEach(x => x.SetParent(null));

            // Add and reselect the old ones
            var reselect = new List<MapObject>();
            foreach (var cr in _clipped)
            {
                var obj = _objects.First(x => x.ID == cr.ID);
                var parent = document.Map.WorldSpawn.FindByID(cr.ParentID);
                obj.SetParent(parent);
                if (cr.IsSelected) reselect.Add(obj);
            }
            document.Selection.Select(reselect);
        }

        public void Perform(Document document)
        {
            _clipped = new List<ClipReference>();
            _results = new List<long>();
            var deselect = new List<MapObject>();
            var reselect = new List<MapObject>();
            foreach (var solid in _objects)
            {
                // Split solid by plane
                Solid back, front;
                if (!solid.Split(_plane, out back, out front, document.Map.IDGenerator)) continue;

                // Record the changes
                _clipped.Add(new ClipReference(solid.ID, solid.Parent.ID, solid.IsSelected));
                _results.Add(back.ID);
                _results.Add(front.ID);

                // Remember selections
                if (solid.IsSelected)
                {
                    deselect.Add(solid);
                    reselect.Add(back);
                    reselect.Add(front);
                }

                // Update parents
                var parent = solid.Parent;
                solid.SetParent(null);
                back.SetParent(parent);
                front.SetParent(parent);
            }
            document.Selection.Deselect(deselect);
            document.Selection.Select(reselect);
        }
    }
}