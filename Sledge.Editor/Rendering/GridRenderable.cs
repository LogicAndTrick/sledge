using System;
using Sledge.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Settings;

namespace Sledge.Editor.Rendering
{
    public class GridRenderable : DisplayListRenderable
    {
        protected decimal LastBuiltStep { get; set; }

        public GridRenderable(string listName) : base(listName)
        {
            LastBuiltStep = -1;
            RebuildGrid(1);
        }

        public void RebuildGrid(decimal zoom, bool force = false)
        {
            var lower = Document.GameData.MapSizeLow;
            var upper = Document.GameData.MapSizeHigh;
            var step = Document.GridSpacing;
            var actualDist = step * zoom;
            if (Grid.HideSmallerOn)
            {
                while (actualDist < Grid.HideSmallerThan)
                {
                    step *= Grid.HideFactor;
                    actualDist *= Grid.HideFactor;
                }
            }
            if (step == LastBuiltStep) return; // This grid is the same as before
            using (DisplayList.Using(ListName))
            {
                GL.Begin(BeginMode.Lines);
                for (decimal i = lower; i <= upper; i += step)
                {
                    var c = Grid.GridLines;
                    if (i == 0) c = Grid.ZeroLines;
                    else if (i % Grid.Highlight2UnitNum == 0 && Grid.Highlight2On) c = Grid.Highlight2;
                    else if (i % (step * Grid.Highlight1LineNum) == 0 && Grid.Highlight1On) c = Grid.Highlight1;
                    var ifloat = (float) i;
                    GL.Color3(c);
                    GL.Vertex2(lower, ifloat);
                    GL.Vertex2(upper, ifloat);
                    GL.Vertex2(ifloat, lower);
                    GL.Vertex2(ifloat, upper);
                }
                GL.Color3(Grid.BoundaryLines);
                // Top
                GL.Vertex2(lower, upper);
                GL.Vertex2(upper, upper);
                // Left
                GL.Vertex2(lower, lower);
                GL.Vertex2(lower, upper);
                // Right
                GL.Vertex2(upper, lower);
                GL.Vertex2(upper, upper);
                // Bottom
                GL.Vertex2(lower, lower);
                GL.Vertex2(upper, lower);
                GL.End();
            }
        }
    }
}
