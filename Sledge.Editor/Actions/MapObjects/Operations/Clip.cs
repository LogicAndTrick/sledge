using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
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
        private Dictionary<long, long> _parents;
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
            _parents = null;
            base.Dispose();
        }

        public override void Perform(Document document)
        {
            if (_firstRun)
            {
                _firstRun = false;
                _parents = new Dictionary<long, long>();
                foreach (var solid in _objects)
                {
                    // Split solid by plane
                    Solid back, front;
                    if (!solid.Split(_plane, out back, out front, document.Map.IDGenerator)) continue;

                    if (solid.IsSelected)
                    {
                        back.IsSelected = front.IsSelected = true;
                    }

                    if (_keepBack) Create(back);
                    if (_keepFront) Create(front);

                    Delete(solid.ID);

                    if (_keepBack) _parents.Add(back.ID, solid.Parent.ID);
                    if (_keepFront) _parents.Add(front.ID, solid.Parent.ID);
                }
            }
            base.Perform(document);
            var objs = new List<MapObject>();
            foreach (var kv in _parents)
            {
                var obj = document.Map.WorldSpawn.FindByID(kv.Key);
                var parent = document.Map.WorldSpawn.FindByID(kv.Value);
                obj.SetParent(parent);

                if (parent is World) objs.Add(obj);
                else if (!objs.Contains(parent)) objs.Add(parent);
            }
            document.Map.UpdateAutoVisgroups(objs, true);
        }
    }
}