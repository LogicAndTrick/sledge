using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Carves the given objects by the given planes, reselecting the new solids if needed.
    /// Reverse: Removes the carved objects and restores the originals, reselecting if needed.
    /// Compare to the clip operation, these two are enormously similar.
    /// </summary>
    public class Carve : CreateEditDelete
    {
        private Solid _carver;
        private List<Solid> _objects;
        private bool _firstRun;

        public Carve(IEnumerable<Solid> objects, Solid carver)
        {
            _objects = objects.ToList();
            _carver = carver;
            _firstRun = true;
        }

        public override void Dispose()
        {
            _carver = null;
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

                        if (front != null)
                        {
                            // Retain the front solid
                            if (solid.IsSelected) front.IsSelected = true;
                            Create(solid.Parent.ID, front);
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