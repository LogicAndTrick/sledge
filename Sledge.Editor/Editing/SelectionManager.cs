using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Geometric;

namespace Sledge.Editor.Editing
{
    public class SelectionManager
    {
        private Documents.Document Document { get; set; }
        private HashSet<MapObject> SelectedObjects { get; set; }
        private HashSet<Face> SelectedFaces { get; set; }
        public bool InFaceSelection { get; private set; }

        public SelectionManager(Documents.Document doc)
        {
            Document = doc;
            SelectedObjects = new HashSet<MapObject>();
            SelectedFaces = new HashSet<Face>();
            InFaceSelection = false;
        }

        public Box GetSelectionBoundingBox()
        {
            if (IsEmpty()) return null;
            return InFaceSelection
                ? new Box(GetSelectedFaces().Select(x => x.BoundingBox))
                : new Box(GetSelectedObjects().Select(x => x.BoundingBox));
        }

        public IEnumerable<MapObject> GetSelectedObjects()
        {
            return new List<MapObject>(SelectedObjects);
        }

        public IEnumerable<Face> GetSelectedFaces()
        {
            return new List<Face>(SelectedFaces);
        }

        public void SwitchToFaceSelection()
        {
            if (InFaceSelection) return;
            foreach (var face in SelectedFaces) face.IsSelected = false;
            SelectedFaces.Clear();

            SelectedFaces.UnionWith(GetSelectedObjects()
                                       .OfType<Solid>()
                                       .SelectMany(x =>
                                                       {
                                                           var disps = x.Faces.Where(y => y is Displacement);
                                                           return disps.Any() ? disps : x.Faces;
                                                       }));

            foreach (var face in SelectedFaces) face.IsSelected = true;

            foreach (var obj in SelectedObjects) obj.IsSelected = false;
            SelectedObjects.Clear();

            InFaceSelection = true;

            Mediator.Publish(EditorMediator.SelectionTypeChanged, Document);
            Mediator.Publish(EditorMediator.SelectionChanged, Document);
        }

        public void SwitchToObjectSelection()
        {
            if (!InFaceSelection) return;
            InFaceSelection = false;
            Clear();

            Mediator.Publish(EditorMediator.SelectionTypeChanged, Document);

            Mediator.Publish(EditorMediator.SelectionChanged, Document);
        }

        public void Clear()
        {
            foreach (var obj in SelectedObjects) obj.IsSelected = false;
            SelectedObjects.Clear();

            foreach (var face in SelectedFaces) face.IsSelected = false;
            SelectedFaces.Clear();
        }

        public void Select(MapObject obj)
        {
            Select(new[] { obj });
        }

        public void Select(IEnumerable<MapObject> objs)
        {
            var list = objs.ToList();
            foreach (var o in list) o.IsSelected = true;
            SelectedObjects.UnionWith(list);
        }

        public void Select(Face face)
        {
            face.IsSelected = true;
            SelectedFaces.Add(face);
        }

        public void Select(IEnumerable<Face> faces)
        {
            var list = faces.ToList();
            foreach (var face in list) face.IsSelected = true;
            SelectedFaces.UnionWith(list);
        }

        public void Deselect(MapObject obj)
        {
            Deselect(new[] {obj});
        }

        public void Deselect(IEnumerable<MapObject> objs)
        {
            var list = objs.ToList();
            SelectedObjects.ExceptWith(list);
            foreach (var o in list) o.IsSelected = false;
        }

        public void Deselect(Face face)
        {
            SelectedFaces.Remove(face);
            face.IsSelected = false;
        }

        public void Deselect(IEnumerable<Face> faces)
        {
            var list = faces.ToList();
            SelectedFaces.ExceptWith(list);
            foreach (var o in list) o.IsSelected = false;
        }

        public bool IsEmpty()
        {
            return InFaceSelection ? SelectedFaces.Count == 0 : SelectedObjects.Count == 0;
        }

        public IEnumerable<MapObject> GetSelectedParents()
        {
            var sel = GetSelectedObjects().ToList();
            sel.SelectMany(x => x.GetChildren()).ToList().ForEach(x => sel.Remove(x));
            return sel;
        }
    }
}
