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
    public class Clip : CreateEditDelete
    {
        private Plane _plane;
        private List<Solid> _objects;
        private bool _firstRun;
        private bool _keepFront;
        private bool _keepBack;

        public Clip(IEnumerable<Solid> objects, Plane plane, bool keepFront, bool keepBack)
        {
            _objects = objects.Where(x => x.IsValid()).ToList();
            _plane = plane;
            _keepFront = keepFront;
            _keepBack = keepBack;
            _firstRun = true;
        }

        public override void Dispose()
        {
            _plane = null;
            _objects = null;
            base.Dispose();
        }

        public override void Perform(Document document)
        {
            if (_firstRun)
            {
                _firstRun = false;
                foreach (var solid in _objects)
                {
                    // Split solid by plane
                    Solid back, front;
                    if (!solid.Split(_plane, out back, out front, document.Map.IDGenerator)) continue;

                    if (solid.IsSelected)
                    {
                        back.IsSelected = front.IsSelected = true;
                    }

                    if (_keepBack) Create(solid.Parent.ID, back);
                    if (_keepFront) Create(solid.Parent.ID, front);

                    Delete(solid.ID);
                }
            }
            base.Perform(document);
        }
    }
}