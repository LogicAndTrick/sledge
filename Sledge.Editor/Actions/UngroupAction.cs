using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions
{
    public class UngroupAction : IAction
    {
        private Dictionary<MapObject, MapObject> _groupsAndParents;
        private Dictionary<MapObject, MapObject> _childrenAndParents;

        public void Perform(Document document)
        {
            _groupsAndParents = new Dictionary<MapObject, MapObject>();
            _childrenAndParents = new Dictionary<MapObject, MapObject>();

            var objs = document.Selection.GetSelectedParents().OfType<Group>().ToList();
            objs.ForEach(x => _groupsAndParents.Add(x, x.Parent));
            objs.ForEach(x => x.SetParent(null));

            var children = objs.SelectMany(x => x.Children).ToList();
            foreach (var o in children)
            {
                _childrenAndParents.Add(o, o.Parent);
                o.SetParent(document.Map.WorldSpawn);
                o.UpdateBoundingBox();
            }

            document.Selection.Clear();
            document.Selection.Select(children);
        }

        public void Reverse(Document document)
        {
            foreach (var gp in _groupsAndParents)
            {
                gp.Key.SetParent(gp.Value);
            }
            foreach (var cp in _childrenAndParents)
            {
                cp.Key.SetParent(cp.Value);
                cp.Key.UpdateBoundingBox();
            }

            document.Selection.Clear();
            document.Selection.Select(_groupsAndParents.Keys.SelectMany(x => x.FindAll()));

            Dispose();
        }

        public void Dispose()
        {
            _groupsAndParents = null;
            _childrenAndParents = null;
        }
    }
}