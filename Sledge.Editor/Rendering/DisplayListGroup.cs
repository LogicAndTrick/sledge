using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.UI;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.UI;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Editing;
using OpenTK;

namespace Sledge.Editor.Rendering
{
    /// <summary>
    /// A display list group holds a number of references to some (shared)
    /// display lists. The selected and deselected object have their own display lists,
    /// in both 2D (wireframe) and 3D (textured). The selected objects also have their own
    /// wireframe in 3D and the red tints in the 3D view.
    /// 
    /// The lists stored in an instance of this class are simply wrappers around these six
    /// global display lists. Each one applies different transformations before drawing the
    /// lists, so that they look correct in their respective views.
    /// </summary>
    public sealed class DisplayListGroup
    {
        public static readonly string DeselectedObjects3DName = "Deselected Objects 3D Display List";
        public static readonly string DeselectedObjects2DName = "Deselected Objects 2D Display List";

        public static readonly string SelectedObjects3DName = "Selected Objects 3D Display List";
        public static readonly string SelectedObjects2DName = "Selected Objects 2D Display List";

        public static readonly string SelectedObjectWireframesName = "Selected Object Wireframes Display List";
        public static readonly string SelectedObjectTintsName = "Selected Object Tints Display List";

        public static readonly Matrix4d TopMatrix = Matrix4d.Identity;
        public static readonly Matrix4d FrontMatrix = new Matrix4d(Vector4d.UnitZ, Vector4d.UnitX, Vector4d.UnitY, Vector4d.UnitW);
        public static readonly Matrix4d SideMatrix = new Matrix4d(Vector4d.UnitX, Vector4d.UnitZ, Vector4d.UnitY, Vector4d.UnitW);

        public static Matrix4d GetMatrixFor(Viewport2D.ViewDirection dir)
        {
            switch (dir)
            {
                case Viewport2D.ViewDirection.Top:
                    return TopMatrix;
                case Viewport2D.ViewDirection.Front:
                    return FrontMatrix;
                case Viewport2D.ViewDirection.Side:
                    return SideMatrix;
                default:
                    throw new ArgumentOutOfRangeException("dir", @"This direction does not exist.");
            }
        }

        private static readonly object Lock = new object();

        private static void CollectFaces(List<Face> faces, IEnumerable<MapObject> list, bool excludeSelected)
        {
            foreach (var mo in list.Where(mo => !excludeSelected || !mo.IsSelected))
            {
                if (mo is Solid)
                {
                    faces.AddRange(((Solid)mo).Faces.Where(x => !x.IsHidden && (!excludeSelected || !x.IsSelected)));
                }
                else if (mo is Entity || mo is Group)
                {
                    CollectFaces(faces, mo.Children, excludeSelected);
                }
            }
        }

        public static void DeleteLists()
        {
            DisplayList.DeleteAll();
        }

        public static void RegenerateSelectLists()
        {
            lock (Lock)
            {
                var faces = new List<Face>();
                if (Selection.InFaceSelection) faces.AddRange(Selection.GetSelectedFaces());
                else CollectFaces(faces, Selection.GetSelectedObjects(), false);

                DataStructures.Rendering.Rendering.CreateFilledList(SelectedObjects3DName, faces, Color.Empty);
                DataStructures.Rendering.Rendering.CreateWireframeList(SelectedObjectWireframesName, faces, true);
                DataStructures.Rendering.Rendering.CreateFilledList(SelectedObjectTintsName, faces, Color.FromArgb(64, Color.Red));
                DataStructures.Rendering.Rendering.CreateWireframeList(SelectedObjects2DName, faces, false);
            }
        }

        public static void RegenerateDisplayLists(bool excludeSelected)
        {
            lock (Lock)
            {
                var faces = new List<Face>();
                CollectFaces(faces, Document.Map.WorldSpawn.Children, excludeSelected);

                DataStructures.Rendering.Rendering.CreateFilledList(DeselectedObjects3DName, faces, Color.Empty);
                DataStructures.Rendering.Rendering.CreateWireframeList(DeselectedObjects2DName, faces, false);
            }
        }

        private readonly bool _3D;
        private readonly Viewport2D.ViewDirection _viewDirection;

        public TransformedDisplayListRenderable DeselectedObjects { get; private set; }
        public TransformedDisplayListRenderable SelectedObjects { get; private set; }
        public TransformedDisplayListRenderable SelectedObjectWireframes { get; private set; }
        public TransformedDisplayListRenderable SelectedObjectTints { get; private set; }

        public DisplayListGroup(Viewport2D.ViewDirection viewDirection)
        {
            _3D = false;
            _viewDirection = viewDirection;
            Init();
        }

        public DisplayListGroup()
        {
            _3D = true;
            Init();
        }

        private void Init()
        {
            var mat = GetMatrix();
            DeselectedObjects = new TransformedDisplayListRenderable(_3D ? DeselectedObjects3DName : DeselectedObjects2DName, mat);
            SelectedObjects = new TransformedDisplayListRenderable(_3D ? SelectedObjects3DName : SelectedObjects2DName, mat);
            SelectedObjectWireframes = new TransformedDisplayListRenderable(SelectedObjectWireframesName, mat);
            SelectedObjectTints = new TransformedDisplayListRenderable(SelectedObjectTintsName, mat);

            SelectedObjectWireframes.Colour = _3D ? Color.Yellow : Color.Red;
        }

        public void Register()
        {
            if (_3D)
            {
                ViewportManager.AddContext3D(DeselectedObjects);
                ViewportManager.AddContext3D(SelectedObjects);
                ViewportManager.AddContext3D(SelectedObjectWireframes);
                ViewportManager.AddContext3D(SelectedObjectTints);
            }
            else
            {
                ViewportManager.AddContext2D(DeselectedObjects, _viewDirection);
                ViewportManager.AddContext2D(SelectedObjects, _viewDirection);
                ViewportManager.AddContext2D(SelectedObjectWireframes, _viewDirection);
                //ViewportManager.AddContext2D(SelectedObjectTints, _viewDirection);
            }
        }

        public void SetSelectListTransform(Matrix4d matrix)
        {
            SelectedObjects.Matrix = Matrix4d.Mult(matrix, GetMatrix());
        }

        private Matrix4d GetMatrix()
        {
            return _3D ? Matrix4d.Identity : GetMatrixFor(_viewDirection);
        }

        internal void SetTintSelectListEnabled(bool enabled)
        {
            SelectedObjectTints.Enabled = enabled;
        }
    }
}
