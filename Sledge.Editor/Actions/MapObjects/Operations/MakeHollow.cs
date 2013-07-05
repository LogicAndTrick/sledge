using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Makes the given objects hollow, reselecting the new solids if needed.
    /// Reverse: Removes the hollowed objects and restores the originals, reselecting if needed.
    /// Compare to the carve and clip operations, these are enormously similar.
    /// </summary>
    public class MakeHollow : IAction
    {
        private class MakeHollowReference
        {
            public long ID { get; set; }
            public long ParentID { get; set; }
            public bool IsSelected { get; set; }

            public MakeHollowReference(long id, long parentID, bool isSelected)
            {
                ID = id;
                ParentID = parentID;
                IsSelected = isSelected;
            }
        }

        private readonly decimal _width;
        private List<Solid> _objects;

        private List<MakeHollowReference> _clipped;
        private List<long> _results;

        public MakeHollow(IEnumerable<Solid> objects, decimal width)
        {
            _objects = objects.ToList();
            _width = width;
        }

        public void Dispose()
        {
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
            _clipped = new List<MakeHollowReference>();
            _results = new List<long>();
            var deselect = new List<MapObject>();
            var reselect = new List<MapObject>();
            foreach (var obj in _objects)
            {
                var split = false;
                var solid = obj;
                var parent = obj.Parent;

                // Make a scaled version of the solid for the "inside" of the hollowed solid
                var origin = solid.GetOrigin();
                var current = obj.BoundingBox.Dimensions;
                var target = current - (new Coordinate(_width, _width, _width) * 2); // Double the width to take from both sides
                // Ensure we don't have any invalid target sizes
                if (target.X < 1) target.X = 1;
                if (target.Y < 1) target.Y = 1;
                if (target.Z < 1) target.Z = 1;

                // Clone and scale the solid
                var scale = target.ComponentDivide(current);
                var transform = new UnitScale(scale, origin);
                var carver = (Solid) solid.Clone();
                carver.Transform(transform, document.Map.GetTransformFlags());

                solid.SetParent(null);
                carver.SetParent(null);

                // For a negative width, we want the original solid to be the inside instead
                if (_width < 0)
                {
                    var temp = carver;
                    carver = solid;
                    solid = temp;
                }

                // Carve the outside solid with the inside solid
                foreach (var plane in carver.Faces.Select(x => x.Plane))
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

                _clipped.Add(new MakeHollowReference(obj.ID, parent.ID, obj.IsSelected));
                obj.SetParent(null);
                if (obj.IsSelected) deselect.Add(obj);
            }
            document.Selection.Deselect(deselect);
            document.Selection.Select(reselect);

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }
    }
}