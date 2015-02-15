using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    public class Reparent : IAction
    {
        private class ReparentReference
        {
            public long ID { get; set; }
            public long OriginalParentID { get; set; }
        }

        public bool SkipInStack { get { return false; } }
        public bool ModifiesState { get { return true; } }

        private readonly long _parentId;
        private List<ReparentReference> _objects; 

        public Reparent(long parentId, IEnumerable<MapObject> objects)
        {
            _parentId = parentId;
            _objects = objects.Select(x => new ReparentReference
                                               {
                                                   ID = x.ID,
                                                   OriginalParentID = x.Parent.ID
                                               }).ToList();
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            var parents = _objects.Select(x => x.OriginalParentID)
                .Distinct()
                .ToDictionary(x => x, x => document.Map.WorldSpawn.FindByID(x));
            foreach (var o in _objects)
            {
                var obj = document.Map.WorldSpawn.FindByID(o.ID);
                if (obj == null) continue;

                obj.SetParent(parents[o.OriginalParentID]);
                document.Map.UpdateAutoVisgroups(obj, true);
            }

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }

        public void Perform(Document document)
        {
            var parent = document.Map.WorldSpawn.FindByID(_parentId);
            foreach (var o in _objects)
            {
                var obj = document.Map.WorldSpawn.FindByID(o.ID);
                if (obj == null) continue;
                obj.SetParent(parent);
                document.Map.UpdateAutoVisgroups(obj, true);
            }

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }
    }
}