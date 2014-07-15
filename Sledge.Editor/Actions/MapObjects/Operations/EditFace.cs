using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common.Mediator;
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
            public Action<Document, Face> Action { get; set; }

            public EditFaceReference(long id, Face before, Face after)
            {
                ParentID = before.Parent.ID;
                ID = id;
                Before = before.Clone();
                After = after.Clone();
                Action = null;
            }

            public EditFaceReference(Face face, Action<Document, Face> action)
            {
                ParentID = face.Parent.ID;
                ID = face.ID;
                Before = face.Clone();
                After = null;
                Action = action;
            }

            public void Perform(Document document)
            {
                var root = document.Map.WorldSpawn;
                var face = GetFace(root);
                if (face == null) return;
                if (Action != null) Action(document, face);
                else face.Unclone(After);
            }

            public void Reverse(Document document)
            {
                var root = document.Map.WorldSpawn;
                var face = GetFace(root);
                if (face == null) return;
                face.Unclone(Before);
            }

            public Face GetFace(MapObject root)
            {
                var obj = root.FindByID(ParentID) as Solid;
                return obj == null ? null : obj.Faces.FirstOrDefault(x => x.ID == ID);
            }
        }

        public bool SkipInStack { get { return false; } }
        public bool ModifiesState { get { return true; } }

        private List<EditFaceReference> _objects;
        private readonly bool _textureOrIdChange;

        public EditFace(IEnumerable<Face> before, IEnumerable<Face> after, bool textureOrIdChange)
        {
            var b = before.ToList();
            var a = after.ToList();
            var ids = b.Select(x => x.ID).Where(x => a.Any(y => x == y.ID));
            _objects = ids.Select(x => new EditFaceReference(x, b.First(y => y.ID == x), a.First(y => y.ID == x))).ToList();
            _textureOrIdChange = textureOrIdChange;
        }

        public EditFace(IEnumerable<Face> objects, Action<Document, Face> action, bool textureOrIdChange)
        {
            _objects = objects.Select(x => new EditFaceReference(x, action)).ToList();
            _textureOrIdChange = textureOrIdChange;
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            _objects.ForEach(x => x.Reverse(document));

            var faces = _objects.Select(x => x.GetFace(document.Map.WorldSpawn));
            if (_textureOrIdChange)
            {
                document.Map.UpdateAutoVisgroups(faces.Where(x => x != null).Select(x => x.Parent).Distinct(), false);
                Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);  
                Mediator.Publish(EditorMediator.VisgroupsChanged);
            }
            else
            {
                Mediator.Publish(EditorMediator.DocumentTreeFacesChanged, faces);
            }
        }

        public void Perform(Document document)
        {
            _objects.ForEach(x => x.Perform(document));
            
            var faces = _objects.Select(x => x.GetFace(document.Map.WorldSpawn));
            if (_textureOrIdChange)
            {
                document.Map.UpdateAutoVisgroups(faces.Where(x => x != null).Select(x => x.Parent).Distinct(), false);
                Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
                Mediator.Publish(EditorMediator.VisgroupsChanged);
            }
            else
            {
                Mediator.Publish(EditorMediator.DocumentTreeFacesChanged, faces);
            }
        }
    }
}