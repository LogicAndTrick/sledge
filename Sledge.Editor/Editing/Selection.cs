using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Geometric;

namespace Sledge.Editor.Editing
{
    public static class Selection
    {
        private static List<MapObject> SelectedObjects { get; set; }
        private static List<Face> SelectedFaces { get; set; }
        public static bool InFaceSelection { get; private set; }
        private static Box _boundingBox;

        private static bool _changed;

        static Selection()
        {
            SelectedObjects = new List<MapObject>();
            SelectedFaces = new List<Face>();
            InFaceSelection = false;
            _changed = false;
        }

        public static IEnumerable<MapObject> GetSelectedObjects()
        {
            if (_changed)
            {
                var distinct = SelectedObjects.Distinct().ToList();
                SelectedObjects.Clear();
                SelectedObjects.AddRange(distinct);
                _changed = false;
            }
            return SelectedObjects;
        }

        public static IEnumerable<Face> GetSelectedFaces()
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

        public static void SwitchToFaceSelection()
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
        }

        public static void SwitchToObjectSelection()
        {
            if (!InFaceSelection) return;
            InFaceSelection = false;
            Clear();
        }

        public static void Clear()
        {
            SelectedObjects.ForEach(x => x.IsSelected = false);
            SelectedObjects.Clear();
            SelectedFaces.ForEach(x => x.IsSelected = false);
            SelectedFaces.Clear();
            _changed = false;
        }

        public static void Select(MapObject obj)
        {
            obj.IsSelected = true;
            SelectedObjects.Add(obj);
            _changed = true;
        }

        public static void Select(IEnumerable<MapObject> objs)
        {
            foreach (var o in objs)
            {
                Select(o);
            }
        }

        public static void Select(Face face)
        {
            face.IsSelected = true;
            SelectedFaces.Add(face);
            _changed = true;
        }

        public static void Select(IEnumerable<Face> faces)
        {
            foreach (var o in faces)
            {
                Select(o);
            }
        }

        public static void Deselect(MapObject obj)
        {
            SelectedObjects.RemoveAll(x => x == obj);
            obj.IsSelected = false;
        }

        public static void Deselect(Face face)
        {
            SelectedFaces.RemoveAll(x => x == face);
            face.IsSelected = false;
        }

        public static bool IsEmpty()
        {
            return InFaceSelection ? SelectedFaces.Count == 0 : SelectedObjects.Count == 0;
        }
    }
}
