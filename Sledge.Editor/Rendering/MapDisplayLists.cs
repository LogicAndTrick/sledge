using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Editing;
using Sledge.Graphics.Helpers;

namespace Sledge.Editor.Rendering
{
    public static class MapDisplayLists
    {
        public const string DeselectedObjects3DName = "Deselected Objects 3D Display List";
        public const string DeselectedObjects2DName = "Deselected Objects 2D Display List";

        public const string SelectedObjects3DName = "Selected Objects 3D Display List";
        public const string SelectedObjects2DName = "Selected Objects 2D Display List";

        public const string SelectedObjectWireframesName = "Selected Object Wireframes Display List";
        public const string SelectedObjectTintsName = "Selected Object Tints Display List";

        private static readonly object Lock = new object();

        private static void CollectFaces(List<Face> faces, IEnumerable<MapObject> list, bool excludeSelected)
        {
            foreach (var mo in list.Where(mo => !excludeSelected || !mo.IsSelected))
            {
                if (mo is Solid && !mo.IsCodeHidden && !mo.IsVisgroupHidden)
                {
                    faces.AddRange(((Solid)mo).Faces.Where(x => !x.IsHidden && (!excludeSelected || !x.IsSelected)));
                }
                else if (mo is Entity || mo is Group)
                {
                    if (mo is Entity && !mo.IsCodeHidden && !mo.IsVisgroupHidden) faces.AddRange(((Entity)mo).GetFaces());
                    CollectFaces(faces, mo.Children, excludeSelected);
                }
            }
        }

        public static void RegenerateSelectLists(SelectionManager selection)
        {
            lock (Lock)
            {
                var faces = new List<Face>();
                if (selection.InFaceSelection) faces.AddRange(selection.GetSelectedFaces());
                else CollectFaces(faces, selection.GetSelectedObjects(), false);

                DataStructures.Rendering.Rendering.CreateFilledList(SelectedObjects3DName, faces, Color.Empty);
                DataStructures.Rendering.Rendering.CreateWireframeList(SelectedObjectWireframesName, faces, true);
                DataStructures.Rendering.Rendering.CreateFilledList(SelectedObjectTintsName, faces, Color.FromArgb(64, Color.Red));
                DataStructures.Rendering.Rendering.CreateWireframeList(SelectedObjects2DName, faces, false);
            }
        }

        public static void RegenerateDisplayLists(IEnumerable<MapObject> objects, bool excludeSelected)
        {
            lock (Lock)
            {
                var faces = new List<Face>();
                CollectFaces(faces, objects, excludeSelected);

                DataStructures.Rendering.Rendering.CreateFilledList(DeselectedObjects3DName, faces, Color.Empty);
                DataStructures.Rendering.Rendering.CreateWireframeList(DeselectedObjects2DName, faces, false);
            }
        }

        public static void DeleteLists()
        {
            lock (Lock)
            {
                DisplayList.Delete(DeselectedObjects3DName);
                DisplayList.Delete(DeselectedObjects2DName);
                DisplayList.Delete(SelectedObjects3DName);
                DisplayList.Delete(SelectedObjects2DName);
                DisplayList.Delete(SelectedObjectWireframesName);
                DisplayList.Delete(SelectedObjectTintsName);
            }
        }
    }
}
