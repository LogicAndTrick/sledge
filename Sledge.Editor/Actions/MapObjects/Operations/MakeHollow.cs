using System.Collections.Generic;
using System.Linq;
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
    public class MakeHollow : CreateEditDelete
    {
        private readonly decimal _width;
        private List<Solid> _objects;
        private bool _firstRun;

        public MakeHollow(IEnumerable<Solid> objects, decimal width)
        {
            _objects = objects.ToList();
            _width = width;
            _firstRun = true;
        }

        public override void Dispose()
        {
            _objects = null;
            base.Dispose();
        }

        public override void Perform(Document document)
        {
            if (_firstRun)
            {
                _firstRun = false;
                foreach (var obj in _objects)
                {
                    var split = false;
                    var solid = obj;

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

                        if (front != null)
                        {
                            // Retain the front solid
                            if (obj.IsSelected) front.IsSelected = true;
                            Create(obj.Parent.ID, front);
                        }

                        if (back == null || !back.IsValid()) break;

                        // Use the back solid as the new clipping target
                        solid = back;
                    }
                    if (!split) continue;

                    Delete(obj.ID);
                }
            }
            base.Perform(document);
        }
    }
}