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

        private static void GLCoordinateTriangle(Vertex v1, Vertex v2, Vertex v3, bool texture)
        {
            var p1 = v1.Location.ToCoordinateF();
            var p2 = v2.Location.ToCoordinateF();
            var p3 = v3.Location.ToCoordinateF();
            var normal = (p3 - p1).Cross(p2 - p1).Normalise();
            GLCoordinate(v1, normal, texture);
            GLCoordinate(v2, normal, texture);
            GLCoordinate(v3, normal, texture);
        }

        private static void GLCoordinate(Vertex v, CoordinateF normal, bool texture)
        {
            if (texture)
            {
                GL.TexCoord2(v.DTextureU, v.DTextureV);
            }
            GL.Normal3(normal.X, normal.Y, normal.Z);
            GLCoordinate(v.Location);
        }

        private static void GLCoordinate(Vertex v, Coordinate normal, bool texture)
        {
            if (texture)
            {
                GL.TexCoord2(v.DTextureU, v.DTextureV);
            }
            GL.Normal3(normal.DX, normal.DY, normal.DZ);
            GLCoordinate(v.Location);
        }

        private static void GLCoordinate(Coordinate c)
        {
            GL.Vertex3(c.DX, c.DY, c.DZ);
        }

        private static bool RenderTransparent(ITexture texture, Face face)
        {
            //if (face.Parent != null && face.Parent.IsTransparent) return true;
            if (texture == null || texture.Name == null) return false;
            return texture.Name.ToLower() == "aaatrigger";
        }

        public static void CreateFilledList(string displayListName, IEnumerable<Face> faces, bool textured, bool lit, Color color = new Color())
        {
            using (DisplayList.Using(displayListName))
            {
                DrawFilled(faces, color, textured, lit);
            }
        }

        public static void DrawFilled(IEnumerable<Face> faces, Color color, bool textured, bool lit = true)
        {
            if (textured) TextureHelper.EnableTexturing();
            faces = faces.Where(x => x.Parent == null || !(x.Parent.IsCodeHidden || x.Parent.IsVisgroupHidden || x.Parent.IsRenderHidden3D));

            GL.Light(LightName.Light0, LightParameter.Position, new float[] {-10000, -20000, 30000, 1});
            GL.Light(LightName.Light1, LightParameter.Position, new float[] {10000, 20000, 30000, 1});
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] {0.0f, 0.0f, 0.0f, 1});
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] {0.6f, 0.6f, 0.6f, 1.0f});
            GL.Light(LightName.Light1, LightParameter.Ambient, new float[] {0.0f, 0.0f, 0.0f, 1});
            GL.Light(LightName.Light1, LightParameter.Diffuse, new float[] {0.3f, 0.3f, 0.3f, 1.0f});
            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] {0.5f, 0.5f, 0.5f, 1.0f});
            if (lit) GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);

            GL.Begin(BeginMode.Triangles);
            GL.Color4(!textured ? color : Color.FromArgb(255, Color.White));
            if (!textured)
            {
                TextureHelper.DisableTexturing();
                foreach (var face in faces)
                {
                    var tp = RenderTransparent(face.Texture.Texture, face);
                    GL.Color4(Color.FromArgb(tp ? 128 : 255, color.IsEmpty ? face.Colour : color));
                    var disp = face is Displacement;
                    foreach (var tri in face.GetTriangles())
                    {
                        if (disp)
                        {
                            GLCoordinateTriangle(tri[0], tri[1], tri[2], false);
                        }
                        else
                        {
                            GLCoordinate(tri[0], face.Plane.Normal, false);
                            GLCoordinate(tri[1], face.Plane.Normal, false);
                            GLCoordinate(tri[2], face.Plane.Normal, false);
                        }
                    }
                }
            }
            else
            {
                var texgroups = from f in faces
                                group f by new {f.Texture.Texture, IsTransparent = RenderTransparent(f.Texture.Texture, f)}
                                into g
                                select g;
                foreach (var g in texgroups.OrderBy(x => !x.Key.IsTransparent ? 0 : 1))
                {
                    var texture = false;
                    var alpha = g.Key.IsTransparent ? 128 : 255;
                    GL.End();
                    if (g.Key.Texture != null)
                    {
                        texture = true;
                        TextureHelper.EnableTexturing();
                        GL.Color4(Color.FromArgb(alpha, Color.White));
                        g.Key.Texture.Bind();
                    }
                    else
                    {
                        TextureHelper.DisableTexturing();
                    }
                    GL.Begin(BeginMode.Triangles);
                    foreach (var f in g)
                    {
                        var disp = f is Displacement;
                        GL.Color4(Color.FromArgb(alpha, texture ? Color.White : f.Colour));
                        //GL.Color3(f.Colour);
                        foreach (var tri in f.GetTriangles())
                        {
                            if (disp)
                            {
                                GLCoordinateTriangle(tri[0], tri[1], tri[2], texture);
                            }
                            else
                            {
                                GLCoordinate(tri[0], f.Plane.Normal, texture);
                                GLCoordinate(tri[1], f.Plane.Normal, texture);
                                GLCoordinate(tri[2], f.Plane.Normal, texture);
                            }
                        }
                        GL.Color3(Color.White);
                    }
                }
            }

            GL.End();

            GL.Disable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Light1);
            GL.Disable(EnableCap.Light0);
            if (lit) GL.Disable(EnableCap.Lighting);

            TextureHelper.DisableTexturing();
        }

        public static void CreateWireframeList(string displayListName, IEnumerable<Face> faces, bool overrideColor)
        {
            using (DisplayList.Using(displayListName))
            {
                DrawWireframe(faces, overrideColor);
            }
        }

        public static void DrawWireframe(IEnumerable<Face> faces, bool overrideColor)
        {
            TextureHelper.DisableTexturing();
            GL.Begin(BeginMode.Lines);

            faces = faces.Where(x => x.Parent == null || !(x.Parent.IsCodeHidden || x.Parent.IsVisgroupHidden || x.Parent.IsRenderHidden2D));

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
        }
    }
}
