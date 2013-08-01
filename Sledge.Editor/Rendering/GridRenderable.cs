using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.Editor.Documents;
using Sledge.Graphics.Arrays;
using Sledge.Graphics.Renderables;
using Sledge.Graphics.Shaders;
using Sledge.Settings;
using Sledge.UI;

namespace Sledge.Editor.Rendering
{
    public class GridRenderable : IRenderable
    {
        #region Shaders

        private const string VertexShader = @"#version 130

layout(location = 0) in vec3 position;
layout(location = 1) in vec4 colour;

smooth out vec4 worldPosition;
smooth out vec4 vertexColour;

uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;

void main()
{
    vec4 pos = vec4(position, 1);
	vec4 cameraPos = cameraMatrix * pos;
	gl_Position = perspectiveMatrix * cameraPos;

    worldPosition = pos;
	vertexColour = colour;
}
";

        public const string FragmentShader = @"#version 130

smooth in vec4 worldPosition;
smooth in vec4 vertexColour;

out vec4 outputColor;
void main()
{
    outputColor = vertexColour;
}
";
        #endregion

        private static readonly BeginMode[] Modes;
        private static readonly ArraySpecification Specification;
        private static readonly int SpecSize;

        static GridRenderable()
        {
            Modes = new[] {BeginMode.Lines};
            Specification = new ArraySpecification(
                ArrayIndex.Vector3("Position"),
                ArrayIndex.Vector4("Colour"));
            SpecSize = Specification.Indices.Sum(x => x.Length);
        }

        private readonly Document _document;
        private readonly ShaderProgram _program;

        private readonly VertexBuffer<float> _buffer;
        private readonly VertexArray<float> _array;


        private decimal _step;
        private bool _needsRebuild;

        public GridRenderable(Document document)
        {
            _program = new ShaderProgram(
                new Shader(ShaderType.VertexShader, VertexShader),
                new Shader(ShaderType.FragmentShader, FragmentShader));
            _document = document;
            _step = -1;
            _buffer = new VertexBuffer<float>(Specification, Modes, 0, sizeof(float), new float[0], new[] { new uint[0] });
            _array = new VertexArray<float>(_buffer);
            RebuildGrid(1);
        }


        public void RebuildGrid(decimal zoom, bool force = false)
        {
            if (!_document.Map.Show2DGrid) return;
            var step = _document.Map.GridSpacing;
            var actualDist = step * zoom;
            if (Grid.HideSmallerOn)
            {
                while (actualDist < Grid.HideSmallerThan)
                {
                    step *= Grid.HideFactor;
                    actualDist *= Grid.HideFactor;
                }
            }
            if (step == _step && !force) return; // This grid is the same as before
            _needsRebuild = true;
            _step = step;
        }

        private void RebuildGrid()
        {
            var array = new List<float>();
            var indices = new List<uint>();

            if (_document.Map.Show2DGrid)
            {
                var lower = _document.GameData.MapSizeLow;
                var upper = _document.GameData.MapSizeHigh;
                for (decimal i = lower; i <= upper; i += _step)
                {
                    var c = Grid.GridLines;
                    if (i == 0) c = Grid.ZeroLines;
                    else if (i % Grid.Highlight2UnitNum == 0 && Grid.Highlight2On) c = Grid.Highlight2;
                    else if (i % (_step * Grid.Highlight1LineNum) == 0 && Grid.Highlight1On) c = Grid.Highlight1;
                    var ifloat = (float)i;
                    MakePoint(array, indices, c, lower, ifloat);
                    MakePoint(array, indices, c, upper, ifloat);
                    MakePoint(array, indices, c, ifloat, lower);
                    MakePoint(array, indices, c, ifloat, upper);
                }

                // Top
                MakePoint(array, indices, Grid.BoundaryLines, lower, upper);
                MakePoint(array, indices, Grid.BoundaryLines, upper, upper);
                // Left
                MakePoint(array, indices, Grid.BoundaryLines, lower, lower);
                MakePoint(array, indices, Grid.BoundaryLines, lower, upper);
                // Right
                MakePoint(array, indices, Grid.BoundaryLines, upper, lower);
                MakePoint(array, indices, Grid.BoundaryLines, upper, upper);
                // Bottom
                MakePoint(array, indices, Grid.BoundaryLines, lower, lower);
                MakePoint(array, indices, Grid.BoundaryLines, upper, lower);
            }

            _buffer.Update(indices.Count, array.ToArray(), new[] { indices.ToArray() });
        }

        private void MakePoint(List<float> array, List<uint> indices, Color colour, float x, float y, float z = 0)
        {
            var point = new[] { x, y, z, colour.R / 255f, colour.G / 255f, colour.B / 255f, colour.A / 255f };
            array.AddRange(point);
            indices.Add((uint)indices.Count);
        }

        public void Render(object sender)
        {
            var viewport = sender as Viewport2D;
            if (viewport == null) return;

            if (_needsRebuild) RebuildGrid();
            _needsRebuild = false;

            _program.Bind();

            _program.Set("perspectiveMatrix", viewport.GetViewportMatrix());
            _program.Set("cameraMatrix", viewport.GetCameraMatrix());

            _array.CreateArrays();
            _array.Bind(0);
            _buffer.DrawElements(0);
            VertexArray<float>.Unbind();

            _program.Unbind();
        }
    }
}
