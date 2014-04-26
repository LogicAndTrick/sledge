using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Graphics.Helpers;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
{
    public class EntitySpriteHelper : IHelper
    {
        public Document Document { get; set; }
        public bool Is2DHelper { get { return false; } }
        public bool Is3DHelper { get { return true; } }
        public bool IsDocumentHelper { get { return false; } }
        public HelperType HelperType { get { return HelperType.Replace; } }

        public bool IsValidFor(MapObject o)
        {
            return !Sledge.Settings.View.DisableSpriteRendering && o is Entity && ((Entity) o).HasSprite();
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
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0);
        }

        public void Render3D(Viewport3D vp, MapObject o)
        {
            var right = vp.Camera.GetRight();
            var up = Vector3.Cross(right, (vp.Camera.LookAt - vp.Camera.Location).Normalized());
            var entity = (Entity) o;

            var orig = new Vector3((float)entity.Origin.X, (float)entity.Origin.Y, (float)entity.Origin.Z);
            if (entity.IsSelected)
            {
                orig = Vector3.TransformPosition(orig, Document.SelectListTransform);
            }
            var normal = Vector3.Subtract(vp.Camera.Location, orig);

            var tex = Document.GetTexture(entity.GetSprite());
            if (tex == null) TextureHelper.Unbind();
            else tex.Bind();

            GL.Color3(Color.White);

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

            // todo rotation/orientation types

            var tup = Vector3.Multiply(up, (float)entity.BoundingBox.Height / 2f);
            var tright = Vector3.Multiply(right, (float)entity.BoundingBox.Width / 2f);

            GL.Begin(PrimitiveType.Quads);

            GL.Normal3(normal); GL.TexCoord2(1, 1); GL.Vertex3(Vector3.Subtract(orig, Vector3.Add(tup, tright)));
            GL.Normal3(normal); GL.TexCoord2(1, 0); GL.Vertex3(Vector3.Add(orig, Vector3.Subtract(tup, tright)));
            GL.Normal3(normal); GL.TexCoord2(0, 0); GL.Vertex3(Vector3.Add(orig, Vector3.Add(tup, tright)));
            GL.Normal3(normal); GL.TexCoord2(0, 1); GL.Vertex3(Vector3.Subtract(orig, Vector3.Subtract(tup, tright)));

            GL.End();
        }

        public void AfterRender3D(Viewport3D viewport)
        {
            GL.AlphaFunc(AlphaFunction.Always, 0);
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Texture2D);
        }

        public void RenderDocument(ViewportBase viewport, Document document)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MapObject> Order(ViewportBase viewport, IEnumerable<MapObject> mapObjects)
        {
            var vp3 = viewport as Viewport3D;
            if (vp3 == null) return mapObjects;
            var cam = vp3.Camera.Location.ToCoordinate();
            return mapObjects.OrderByDescending(x => (x.BoundingBox.Center - cam).LengthSquared());
        }
    }
}