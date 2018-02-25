using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Arrays;
using Sledge.DataStructures.Geometric;
using OpenTK;
using OpenTK.Graphics;
using OGL = OpenTK.Graphics.OpenGL.GL;
using BeginMode = OpenTK.Graphics.OpenGL.BeginMode;

namespace Sledge.Editor.Rendering.Arrays
{
    public class GridArray : VBO<object, MapObjectVertex>
    {
        private const int Grid = 0;

        public GridArray()
            : base(new object[0])
        {
        }

        public void Render(OpenTK.Graphics.IGraphicsContext context, Sledge.UI.Viewport2D vp)
        {
            foreach (var subset in GetSubsets(Grid))
            {
                Render(context, PrimitiveType.Lines, subset);
            }

            if (!Sledge.Settings.Grid.DottedGrid)
                return;

            // Using direct mode to draw dots here is necessary, although this is somewhat hackish.
            // The VBO's can't handle the huge number of primitives at low grid sizes and chokes hard, so we must render dots directly every frame.

            // Upper left and lower right of viewport in world space
            Coordinate ulworld = vp.ScreenToWorld(0, 0);
            Coordinate lrworld = vp.ScreenToWorld(vp.Width, vp.Height);

            // Snap to grid point (nearest multiple of _step, flooring and ceiling to not forget any at the edges.
            Coordinate uldot = new Coordinate(Math.Floor(_step * Math.Floor(ulworld.X/_step)), Math.Floor(_step * Math.Floor(ulworld.Y/_step)), 0);
            Coordinate lrdot = new Coordinate(Math.Ceiling(_step * Math.Ceiling(lrworld.X/_step)), Math.Ceiling(_step * Math.Ceiling(lrworld.Y/_step)), 0);

            OGL.PointSize(2.0f);    // Hardcoded value, could be added to settings
            OGL.Begin(PrimitiveType.Points);
            OGL.Color3(Sledge.Settings.Grid.GridLines);
            for (decimal i = Math.Max(uldot.X, _low); i <= Math.Min(lrdot.X, _high); i += _step)
            {
                for (decimal j = Math.Max(uldot.Y, _low); j <= Math.Min(lrdot.Y, _high); j += _step)
                {
                    // Skip points that lie on the highlight lines.
                    if ((i % Sledge.Settings.Grid.Highlight2UnitNum == 0 || j % Sledge.Settings.Grid.Highlight2UnitNum == 0) && Sledge.Settings.Grid.Highlight2On)
                        continue;
                    if ((i % (_step * Sledge.Settings.Grid.Highlight1LineNum) == 0 || j % (_step * Sledge.Settings.Grid.Highlight1LineNum) == 0) && Sledge.Settings.Grid.Highlight1On)
                        continue;
                    OGL.Vertex2((float)i, (float)j);
                }
            }
            OGL.End();
            OGL.PointSize(1.0f);
        }

        private decimal _step = 64;
        private int _low = -4096;
        private int _high = 4096;

        public void Update(int low, int high, decimal gridSpacing, decimal zoom, bool force = false)
        {
            var actualDist = gridSpacing * zoom;
            if (Sledge.Settings.Grid.HideSmallerOn)
            {
                while (actualDist < Sledge.Settings.Grid.HideSmallerThan)
                {
                    gridSpacing *= Sledge.Settings.Grid.HideFactor;
                    actualDist *= Sledge.Settings.Grid.HideFactor;
                }
            }
            if (gridSpacing == _step && !force && low == _low && high == _high) return; // This grid is the same as before
            _step = gridSpacing;
            _low = low;
            _high = high;
            Update(new object[0]);
        }

        protected override void CreateArray(IEnumerable<object> objects)
        {
            StartSubset(Grid);

            for (decimal i = _low; i <= _high; i += _step)
            {
                var c = Sledge.Settings.Grid.GridLines;
                if (i == 0)
                    c = Sledge.Settings.Grid.ZeroLines;
                else if (i % Sledge.Settings.Grid.Highlight2UnitNum == 0 && Sledge.Settings.Grid.Highlight2On)
                    c = Sledge.Settings.Grid.Highlight2;
                else if (i % (_step * Sledge.Settings.Grid.Highlight1LineNum) == 0 && Sledge.Settings.Grid.Highlight1On)
                    c = Sledge.Settings.Grid.Highlight1;
                else if (Sledge.Settings.Grid.DottedGrid)   // Don't draw regular gridlines if using dotted grid
                    continue;

                var ifloat = (float)i;
                MakePoint(c, _low, ifloat);
                MakePoint(c, _high, ifloat);
                MakePoint(c, ifloat, _low);
                MakePoint(c, ifloat, _high);
            }

            // Top
            MakePoint(Sledge.Settings.Grid.BoundaryLines, _low, _high);
            MakePoint(Sledge.Settings.Grid.BoundaryLines, _high, _high);
            // Left
            MakePoint(Sledge.Settings.Grid.BoundaryLines, _low, _low);
            MakePoint(Sledge.Settings.Grid.BoundaryLines, _low, _high);
            // Right
            MakePoint(Sledge.Settings.Grid.BoundaryLines, _high, _low);
            MakePoint(Sledge.Settings.Grid.BoundaryLines, _high, _high);
            // Bottom
            MakePoint(Sledge.Settings.Grid.BoundaryLines, _low, _low);
            MakePoint(Sledge.Settings.Grid.BoundaryLines, _high, _low);

            PushSubset(Grid, (object)null);
        }

        private void MakePoint(Color colour, float x, float y, float z = 0)
        {
            PushIndex(Grid, PushData(new[]
            {
                new MapObjectVertex
                {
                    Position = new Vector3(x, y, z),
                    Colour = new Color4(colour.R, colour.G, colour.B, colour.A),
                    Normal = Vector3.Zero,
                    Texture = Vector2.Zero,
                    IsSelected = 0
                }
            }), new uint[] { 0 });
        }
    }
}