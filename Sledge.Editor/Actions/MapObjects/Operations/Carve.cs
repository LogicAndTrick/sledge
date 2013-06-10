using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Providers.Map;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Carves the given objects by the given planes, reselecting the new solids if needed.
    /// Reverse: Removes the carved objects and restores the originals, reselecting if needed.
    /// Compare to the clip operation, these two are enormously similar.
    /// </summary>
    public class Carve : IAction
    {
        private class CarveReference
        {
            public long ID { get; set; }
            public long ParentID { get; set; }
            public bool IsSelected { get; set; }

            public CarveReference(long id, long parentID, bool isSelected)
            {
                ID = id;
                ParentID = parentID;
                IsSelected = isSelected;
            }
        }

        private Solid _carver;
        private List<Solid> _objects;

        private List<CarveReference> _clipped;
        private List<long> _results;

        public Carve(IEnumerable<Solid> objects, Solid carver)
        {
            _objects = objects.ToList();
            _carver = carver;
        }

        public void Dispose()
        {
            _carver = null;
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

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }

        public void Perform(Document document)
        {
            _clipped = new List<CarveReference>();
            _results = new List<long>();
            var deselect = new List<MapObject>();
            var reselect = new List<MapObject>();
            foreach (var obj in _objects)
            {
                var split = false;
                var solid = obj;
                var parent = obj.Parent;

                foreach (var plane in _carver.Faces.Select(x => x.Plane))
                {
                    // Split solid by plane
                    Solid back, front;
                    try
                    {
                        if (!solid.Split(plane, out back, out front, document.Map.IDGenerator)) continue;
                    }
                    catch
                    {
                        // We're not too fussy about over-complicated carving, just get out if we've broken it.
                        break;
                    }
                    split = true;

                    if (back == null || !back.IsValid()) break;

                    // Retain the front solid
                    _results.Add(front.ID);
                    if (obj.IsSelected) reselect.Add(front);
                    front.SetParent(parent);

                    // Use the back solid as the new clipping target
                    solid = back;
                }
                if (!split) continue;

                _clipped.Add(new CarveReference(obj.ID, parent.ID, obj.IsSelected));
                obj.SetParent(null);
                if (obj.IsSelected) deselect.Add(obj);
            }
            document.Selection.Deselect(deselect);
            document.Selection.Select(reselect);

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }
    }
}