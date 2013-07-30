using System;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Graphics.Helpers;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
{
    public class EntitySpriteHelper : IHelper
    {
        public bool Is2DHelper { get { return false; } }
        public bool Is3DHelper { get { return true; } }
        public bool IsDocumentHelper { get { return false; } }
        public HelperType HelperType { get { return HelperType.Replace; } }

        public bool IsValidFor(MapObject o)
        {
            return o is Entity && ((Entity) o).Sprite != null;
        }

        public void BeforeRender2D(Viewport2D viewport)
        {
            throw new NotImplementedException();
        }

        public void Render2D(Viewport2D viewport, MapObject o)
        {
            throw new NotImplementedException();
        }

        public void AfterRender2D(Viewport2D viewport)
        {
            throw new NotImplementedException();
        }

        public void BeforeRender3D(Viewport3D viewport)
        {

        }

        public void Render3D(Viewport3D vp, MapObject o)
        {
            // These billboards aren't perfect but they'll do (they rotate with the lookat vector rather than the location vector)

            var right = vp.Camera.GetRight();
            var up = vp.Camera.GetUp();
            var entity = (Entity) o;

            var orig = new Vector3((float)entity.Origin.X, (float)entity.Origin.Y, (float)entity.Origin.Z);
            var normal = Vector3.Subtract(vp.Camera.Location, orig);

            var tex = entity.Sprite;
            TextureHelper.EnableTexturing();
            GL.Color3(Color.White);
            tex.Bind();

            if (entity.GameData != null)
            {
                var col = entity.GameData.Properties.FirstOrDefault(x => x.VariableType == VariableType.Color255);
                if (col != null)
                {
                    var val = entity.EntityData.Properties.FirstOrDefault(x => x.Key == col.Name);
                    if (val != null)
                    {
                        GL.Color3(val.GetColour(Color.White));
                    }
                }
            }

            var tup = Vector3.Multiply(up, (float)entity.BoundingBox.Height / 2f);
            var tright = Vector3.Multiply(right, (float)entity.BoundingBox.Width / 2f);

            GL.Begin(BeginMode.Quads);

            GL.Normal3(normal); GL.TexCoord2(1, 1); GL.Vertex3(Vector3.Subtract(orig, Vector3.Add(tup, tright)));
            GL.Normal3(normal); GL.TexCoord2(1, 0); GL.Vertex3(Vector3.Add(orig, Vector3.Subtract(tup, tright)));
            GL.Normal3(normal); GL.TexCoord2(0, 0); GL.Vertex3(Vector3.Add(orig, Vector3.Add(tup, tright)));
            GL.Normal3(normal); GL.TexCoord2(0, 1); GL.Vertex3(Vector3.Subtract(orig, Vector3.Subtract(tup, tright)));

            GL.End();
        }

        public void AfterRender3D(Viewport3D viewport)
        {

        }

        public void RenderDocument(ViewportBase viewport, Document document)
        {
            throw new NotImplementedException();
        }
    }
}