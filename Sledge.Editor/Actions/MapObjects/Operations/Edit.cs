using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Changes the given objects (by ID) to the "after" state.
    /// Reverse: Changes the given objects (by ID) to the "before" state.
    /// </summary>
    public class Edit : IAction
    {
        private class EditReference
        {
            public long ID { get; set; }
            public MapObject Before { get; set; }
            public MapObject After { get; set; }
            public Action<MapObject> Action { get; set; }

            public EditReference(long id, MapObject before, MapObject after)
            {
                ID = id;
                Before = before.Clone();
                After = after.Clone();
                Action = null;
            }

            public EditReference(MapObject obj, Action<MapObject> action)
            {
                ID = obj.ID;
                Before = obj.Clone();
                After = null;
                Action = action;
            }

            public void Perform(MapObject root)
            {
                var obj = root.FindByID(ID);
                if (obj == null) return;
                if (Action != null) Action(obj);
                else obj.Unclone(After);
            }

            public void Reverse(MapObject root)
            {
                var obj = root.FindByID(ID);
                if (obj == null) return;
                obj.Unclone(Before);
            }
        }

        private List<EditReference> _objects;

        public Edit(IEnumerable<MapObject> before, IEnumerable<MapObject> after)
        {
            var b = before.ToList();
            var a = after.ToList();
            var ids = b.Select(x => x.ID).Where(x => a.Any(y => x == y.ID));
            _objects = ids.Select(x => new EditReference(x, b.First(y => y.ID == x), a.First(y => y.ID == x))).ToList();
        }

        public Edit(IEnumerable<MapObject> objects, Action<MapObject> action)
        {
            _objects = objects.Select(x => new EditReference(x, action)).ToList();
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            Parallel.ForEach(_objects, x => x.Reverse(document.Map.WorldSpawn));

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged, _objects.Select(x => document.Map.WorldSpawn.FindByID(x.ID)));
        }

        public void Perform(Document document)
        {
            Parallel.ForEach(_objects, x => x.Perform(document.Map.WorldSpawn));

            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged, _objects.Select(x => document.Map.WorldSpawn.FindByID(x.ID)));
        }
    }
}
