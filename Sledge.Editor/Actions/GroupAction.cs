using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions
{
    public class GroupAction : IAction
    {
        private Dictionary<MapObject, MapObject> _originalParents;
        private Group _createdGroup;

        public void Perform(Document document)
        {
            _originalParents = new Dictionary<MapObject, MapObject>();
            var objs = document.Selection.GetSelectedParents().ToList();
            objs.ForEach(x => _originalParents.Add(x, x.Parent));

            _createdGroup = new Group(document.Map.IDGenerator.GetNextObjectID());
            objs.ForEach(x => x.SetParent(_createdGroup));
            _createdGroup.SetParent(document.Map.WorldSpawn);
            _createdGroup.UpdateBoundingBox();

            document.Selection.Clear();
            document.Selection.Select(_createdGroup.FindAll());
        }

        public void Reverse(Document document)
        {
            var children = _createdGroup.Children.ToList();
            children.ForEach(x => x.SetParent(_originalParents[x]));
            children.ForEach(x => x.UpdateBoundingBox());

            document.Selection.Clear();
            document.Selection.Select(children.SelectMany(x => x.FindAll()));

            Dispose();
        }

        public void Dispose()
        {
            _createdGroup = null;
            _originalParents = null;
        }
    }
}