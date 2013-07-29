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
        private List<MapObject> SelectedObjects { get; set; }
        private List<Face> SelectedFaces { get; set; }
        public bool InFaceSelection { get; private set; }

        private bool _changed;

        public SelectionManager(Documents.Document doc)
        {
            Document = doc;
            SelectedObjects = new List<MapObject>();
            SelectedFaces = new List<Face>();
            InFaceSelection = false;
            _changed = false;
        }

        public Box GetSelectionBoundingBox()
        {
            return IsEmpty() ? null : new Box(GetSelectedObjects().Select(x => x.BoundingBox));
        }

        public IEnumerable<MapObject> GetSelectedObjects()
        {
            if (_changed)
            {
                var distinct = SelectedObjects.Distinct().ToList();
                SelectedObjects.Clear();
                SelectedObjects.AddRange(distinct);
                _changed = false;
            }
            return new List<MapObject>(SelectedObjects);
        }

        public IEnumerable<Face> GetSelectedFaces()
        {
            if (_changed)
            {
                var distinct = SelectedFaces.Distinct().ToList();
                SelectedFaces.Clear();
                SelectedFaces.AddRange(distinct);
                _changed = false;
            }
            return SelectedFaces;
        }

        public void SwitchToFaceSelection()
        {
            if (InFaceSelection) return;
            SelectedFaces.ForEach(x => x.IsSelected = false);
            SelectedFaces.Clear();

            SelectedFaces.AddRange(GetSelectedObjects()
                                       .OfType<Solid>()
                                       .SelectMany(x =>
                                                       {
                                                           var disps = x.Faces.Where(y => y is Displacement);
                                                           return disps.Any() ? disps : x.Faces;
                                                       }));
            SelectedFaces.ForEach(x => x.IsSelected = true);

            SelectedObjects.ForEach(x => x.IsSelected = false);
            SelectedObjects.Clear();

            InFaceSelection = true;
            _changed = false;

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
            SelectedObjects.ForEach(x => x.IsSelected = false);
            SelectedObjects.Clear();
            SelectedFaces.ForEach(x => x.IsSelected = false);
            SelectedFaces.Clear();
            _changed = false;
        }

        public void Select(MapObject obj)
        {
            Select(new[] { obj });
        }

        public void Select(IEnumerable<MapObject> objs)
        {
            foreach (var obj in objs.SelectMany(x => x.FindAll()).Distinct())
            {
                obj.IsSelected = true;
                SelectedObjects.Add(obj);
            }
            _changed = true;
        }

        public void Select(Face face)
        {
            face.IsSelected = true;
            SelectedFaces.Add(face);
            _changed = true;
        }

        public void Select(IEnumerable<Face> faces)
        {
            foreach (var face in faces)
            {
                face.IsSelected = true;
                SelectedFaces.Add(face);
            }
            _changed = true;
        }

        public void Deselect(MapObject obj)
        {
            Deselect(new[] {obj});
        }

        public void Deselect(IEnumerable<MapObject> objs)
        {
            foreach (var obj in objs.SelectMany(x => x.FindAll()).Distinct())
            {
                SelectedObjects.RemoveAll(x => x == obj);
                obj.IsSelected = false;
            }
            _changed = true;
        }

        public void Deselect(Face face)
        {
            SelectedFaces.RemoveAll(x => x == face);
            face.IsSelected = false;
        }

        public void Deselect(IEnumerable<Face> faces)
        {
            foreach (var face in faces)
            {
                SelectedFaces.RemoveAll(x => x == face);
                face.IsSelected = false;
            }
            _changed = true;
        }

        public bool IsEmpty()
        {
            return InFaceSelection ? SelectedFaces.Count == 0 : SelectedObjects.Count == 0;
        }

        public IEnumerable<MapObject> GetSelectedParents()
        {
            var sel = GetSelectedObjects().ToList();
            sel.SelectMany(x => x.Children).ToList().ForEach(x => sel.Remove(x));
            return sel;
        }
    }
}
