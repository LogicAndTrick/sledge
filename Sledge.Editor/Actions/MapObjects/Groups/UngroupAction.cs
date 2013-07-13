using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Groups
{
    public class UngroupAction : IAction
    {
        private Dictionary<long, long> _groupsAndParents;
        private Dictionary<long, long> _childrenAndParents;

        public UngroupAction(IEnumerable<MapObject> objects)
        {
            var objs = objects.Where(x => x != null && x.Parent != null).OfType<Group>().ToList();
            _groupsAndParents = objs.ToDictionary(x => x.ID, x => x.Parent.ID);
            _childrenAndParents = objs.SelectMany(x => x.Children).ToDictionary(x => x.ID, x => x.Parent.ID);
        }

        public void Perform(Document document)
        {
            foreach (var child in _childrenAndParents.Keys.Select(x => document.Map.WorldSpawn.FindByID(x)))
            {
                child.SetParent(document.Map.WorldSpawn);
                child.UpdateBoundingBox();
            }

            foreach (var group in _groupsAndParents.Keys.Select(x => document.Map.WorldSpawn.FindByID(x)))
            {
                document.Selection.Deselect(group);
                group.SetParent(null);
            }

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }

        public void Reverse(Document document)
        {
            foreach (var gp in _groupsAndParents)
            {
                var group = document.Map.WorldSpawn.FindByID(gp.Key);
                var parent = document.Map.WorldSpawn.FindByID(gp.Value);
                group.SetParent(parent);
            }
            foreach (var cp in _childrenAndParents)
            {
                var child = document.Map.WorldSpawn.FindByID(cp.Key);
                var parent = document.Map.WorldSpawn.FindByID(cp.Value);
                child.SetParent(parent);
                child.UpdateBoundingBox();
            }

            Dispose();

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }

        public void Dispose()
        {
            _groupsAndParents = null;
            _childrenAndParents = null;
        }
    }
}