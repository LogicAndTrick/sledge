using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Changes the given faces (by ID) to the "after" state.
    /// Reverse: Changes the given faces (by ID) to the "before" state.
    /// </summary>
    public class EditFace : IAction
    {
        private class EditFaceReference
        {
            public long ParentID { get; set; }
            public long ID { get; set; }
            public Face Before { get; set; }
            public Face After { get; set; }
            public Action<Face> Action { get; set; }

            public EditFaceReference(long id, Face before, Face after)
            {
                ParentID = before.Parent.ID;
                ID = id;
                Before = before.Clone();
                After = after.Clone();
                Action = null;
            }

            public EditFaceReference(Face obj, Action<Face> action)
            {
                ID = obj.ID;
                Before = obj.Clone();
                After = null;
                Action = action;
            }

            public void Perform(MapObject root)
            {
                var obj = root.FindByID(ParentID) as Solid;
                if (obj == null) return;
                var face = obj.Faces.FirstOrDefault(x => x.ID == ID);
                if (face == null) return;
                if (Action != null) Action(face);
                else face.Unclone(After);
            }

            public void Reverse(MapObject root)
            {
                var obj = root.FindByID(ID) as Solid;
                if (obj == null) return;
                var face = obj.Faces.FirstOrDefault(x => x.ID == ID);
                if (face == null) return;
                face.Unclone(Before);
            }
        }

        private List<EditFaceReference> _objects;

        public EditFace(IEnumerable<Face> before, IEnumerable<Face> after)
        {
            var b = before.ToList();
            var a = after.ToList();
            var ids = b.Select(x => x.ID).Where(x => a.Any(y => x == y.ID));
            _objects = ids.Select(x => new EditFaceReference(x, b.First(y => y.ID == x), a.First(y => y.ID == x))).ToList();
        }

        public EditFace(IEnumerable<Face> objects, Action<Face> action)
        {
            _objects = objects.Select(x => new EditFaceReference(x, action)).ToList();
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            Parallel.ForEach(_objects, x => x.Reverse(document.Map.WorldSpawn));
        }

        public void Perform(Document document)
        {
            Parallel.ForEach(_objects, x => x.Perform(document.Map.WorldSpawn));
        }
    }
}