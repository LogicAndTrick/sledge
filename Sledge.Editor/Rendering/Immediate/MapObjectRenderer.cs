using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Graphics.Helpers;

namespace Sledge.Editor.Rendering.Immediate
{
    public static class MapObjectRenderer
    {
        public static Color Tint(Coordinate sun, Coordinate normal, Color c)
        {
            var tintvar = (double)sun.Normalise().Dot(normal.Normalise());
            // tint variation = 128
            var diff = (int)(64 * (tintvar + 1));

            return Color.FromArgb(c.A, Math.Max(0, c.R - diff), Math.Max(0, c.G - diff), Math.Max(0, c.B - diff));
        }

        private static void GLCoordinateTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            var p1 = v1.Location.ToCoordinateF();
            var p2 = v2.Location.ToCoordinateF();
            var p3 = v3.Location.ToCoordinateF();
            var normal = (p3 - p1).Cross(p2 - p1).Normalise();
            GLCoordinate(v1, normal);
            GLCoordinate(v2, normal);
            GLCoordinate(v3, normal);
        }

        private static void GLCoordinate(Vertex v, CoordinateF normal)
        {
            GL.TexCoord2(v.DTextureU, v.DTextureV);
            GL.Normal3(normal.X, normal.Y, normal.Z);
            GLCoordinate(v.Location);
        }

        private static void GLCoordinate(Vertex v, Coordinate normal)
        {
            GL.TexCoord2(v.DTextureU, v.DTextureV);
            GL.Normal3(normal.DX, normal.DY, normal.DZ);
            GLCoordinate(v.Location);
        }

        private static void GLCoordinate(Coordinate c)
        {
            GL.Vertex3(c.DX, c.DY, c.DZ);
        }

        private static float GetOpacity(ITexture texture, Face face)
        {
            return face.Opacity;
        }

        public static void DrawFilled(IEnumerable<Face> faces, Color color, bool textured, bool blend = true)
        {
            if (color.IsEmpty) color = Color.White;
            faces = faces.Where(x => x.Parent == null || !(x.Parent.IsCodeHidden || x.Parent.IsVisgroupHidden || x.Parent.IsRenderHidden3D));

            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(color);

            var texgroups = from f in faces
                            group f by new
                            {
                                f.Texture.Texture,
                                Opacity = GetOpacity(f.Texture.Texture, f),
                                Transparent = GetOpacity(f.Texture.Texture, f) < 0.9 || (f.Texture.Texture != null && f.Texture.Texture.HasTransparency())
                            }
                                into g
                                select g;
            foreach (var g in texgroups.OrderBy(x => x.Key.Transparent ? 1 : 0))
            {
                var texture = false;
                var alpha = g.Key.Opacity * 255;
                var blendAlpha = (byte)((color.A) / 255f * (alpha / 255f) * 255);
                GL.End();
                if (g.Key.Texture != null && textured)
                {
                    texture = true;
                    GL.Color4(Color.FromArgb(blendAlpha, color));
                    g.Key.Texture.Bind();
                }
                else
                {
                    TextureHelper.Unbind();
                }
                GL.Begin(PrimitiveType.Triangles);
                foreach (var f in g)
                {
                    var disp = f is Displacement;
                    GL.Color4(Color.FromArgb(blendAlpha, texture ? color : (blend ? f.Colour.Blend(color) : color)));
                    foreach (var tri in f.GetTriangles())
                    {
                        if (disp)
                        {
                            GLCoordinateTriangle(tri[0], tri[1], tri[2]);
                        }
                        else
                        {
                            GLCoordinate(tri[0], f.Plane.Normal);
                            GLCoordinate(tri[1], f.Plane.Normal);
                            GLCoordinate(tri[2], f.Plane.Normal);
                        }
                    }
                    GL.Color3(Color.White);
                }
            }

            GL.End();
            GL.Color4(Color.White);
        }

        public static void DrawWireframe(IEnumerable<Face> faces, bool overrideColor, bool drawVertices)
        {
            faces = faces.Where(x => x.Parent == null || !(x.Parent.IsCodeHidden || x.Parent.IsVisgroupHidden || x.Parent.IsRenderHidden2D)).ToList();

            TextureHelper.Unbind();
            GL.Begin(PrimitiveType.Lines);

            foreach (var f in faces)
            {
                if (!overrideColor) GL.Color4(f.Colour);
                foreach (var line in f.GetLines())
                {
                    GLCoordinate(line.Start);
                    GLCoordinate(line.End);
                }
            }

            GL.End();

            if (!drawVertices || !Sledge.Settings.View.Draw2DVertices) return;

            GL.PointSize(Sledge.Settings.View.VertexPointSize);
            GL.Begin(PrimitiveType.Points);
            GL.Color4(Sledge.Settings.View.VertexOverrideColour);
            foreach (var f in faces)
            {
                if (!Sledge.Settings.View.OverrideVertexColour) GL.Color4(f.Colour);
                foreach (var line in f.GetLines())
                {
                    GLCoordinate(line.Start);
                    GLCoordinate(line.End);
                }
            }
            GL.End();
        }

        public static void EnableLighting()
        {
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { -10000, -20000, -30000, 1 });
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.0f, 0.0f, 0.0f, 1 });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });

            GL.Light(LightName.Light1, LightParameter.Position, new float[] { 10000, 20000, 30000, 1 });
            GL.Light(LightName.Light1, LightParameter.Ambient, new float[] { 0.0f, 0.0f, 0.0f, 1 });
            GL.Light(LightName.Light1, LightParameter.Diffuse, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });

            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.8f, 0.8f, 0.8f, 1.0f });

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
        }

        public static void DisableLighting()
        {
            GL.Disable(EnableCap.Light1);
            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Lighting);
        }
    }
}
