using System.Collections.Generic;
using System.Linq;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Groups
{
    public class GroupAction : IAction
    {
        private readonly List<long> _groupedObjects;
        private long _groupId;
        private Dictionary<long, long> _originalChildParents;

        public bool SkipInStack { get { return false; } }
        public bool ModifiesState { get { return true; } }

        public GroupAction(IEnumerable<MapObject> groupedObjects)
        {
            _groupedObjects = groupedObjects.Select(x => x.ID).ToList();
        }

        public void Perform(Document document)
        {
            var objects = _groupedObjects
                .Select(x => document.Map.WorldSpawn.FindByID(x))
                .Where(x => x != null && x.Parent != null)
                .ToList();
            _originalChildParents = objects.ToDictionary(x => x.ID, x => x.Parent.ID);

            if (_groupId == 0) _groupId = document.Map.IDGenerator.GetNextObjectID();
            var group = new Group(_groupId) {Colour = Colour.GetRandomGroupColour()};

            objects.ForEach(x => x.SetParent(group));
            objects.ForEach(x => x.Colour = group.Colour.Vary());
            group.SetParent(document.Map.WorldSpawn);
            group.UpdateBoundingBox();

            if (group.GetChildren().All(x => x.IsSelected))
            {
                document.Selection.Select(group);
                Mediator.Publish(EditorMediator.SelectionChanged);
            }

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }

        public void Reverse(Document document)
        {
            var group = document.Map.WorldSpawn.FindByID(_groupId);
            var children = group.GetChildren().ToList();
            children.ForEach(x => x.SetParent(document.Map.WorldSpawn.FindByID(_originalChildParents[x.ID])));
            children.ForEach(x => x.Colour = Colour.GetRandomBrushColour());
            children.ForEach(x => x.UpdateBoundingBox());
            group.SetParent(null);

            if (group.IsSelected)
            {
                document.Selection.Deselect(group);
                Mediator.Publish(EditorMediator.SelectionChanged);
            }

            _originalChildParents.Clear();

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }

        public void Dispose()
        {
            _originalChildParents = null;
        }
    }
}