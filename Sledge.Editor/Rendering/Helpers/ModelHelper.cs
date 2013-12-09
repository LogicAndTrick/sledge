using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.DataStructures.Rendering;
using Sledge.DataStructures.Rendering.Models;
using Sledge.Editor.Documents;
using Sledge.FileSystem;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;
using Sledge.Providers.Model;
using Sledge.UI;
using Tao.OpenGl;

namespace Sledge.Editor.Rendering.Helpers
{
    public class ModelHelper : IHelper
    {
        public Document Document { get; set; }
        public bool Is2DHelper { get { return false; } }
        public bool Is3DHelper { get { return true; } }
        public bool IsDocumentHelper { get { return false; } }
        public HelperType HelperType { get { return HelperType.Replace; } }

        public ModelHelper()
        {
        }

        public bool IsValidFor(MapObject o)
        {
            if (!(o is Entity)) return false;
            var e = (Entity) o;
            if (e.GameData == null) return false;
            var studio = e.GameData.Behaviours.FirstOrDefault(x => x.Name == "studio");
            return studio != null && studio.Values.Count == 1 && studio.Values[0].Trim() != "";
        }

        public void BeforeRender2D(Viewport2D viewport)
        {
            throw new System.NotImplementedException();
        }

        public void Render2D(Viewport2D viewport, MapObject o)
        {
            throw new System.NotImplementedException();
        }

        public void AfterRender2D(Viewport2D viewport)
        {
            throw new System.NotImplementedException();
        }

        public void BeforeRender3D(Viewport3D viewport)
        {

        }

        private List<Model> _cache = new List<Model>(); //todo move this to proper helper
        private Dictionary<Model, IRenderable> _renderables = new Dictionary<Model, IRenderable>(); 
 
        public void Render3D(Viewport3D viewport, MapObject o)
        {
            var e = (Entity) o;
            var loc = viewport.Camera.Location;
            var distance = new Coordinate((decimal)loc.X, (decimal)loc.Y, (decimal)loc.Z) - o.BoundingBox.Center;
            if (distance.VectorMagnitude() < 1000 && e.GameData != null)
            {
                var studio = e.GameData.Behaviours.FirstOrDefault(x => x.Name == "studio");
                if (studio != null && studio.Values.Count == 1 && studio.Values[0].Trim() != "")
                {
                    var path = studio.Values[0].Trim();

                    // todo cache these!
                    var file = Document.Environment.Root.TraversePath(path);
                    if (file != null)
                    {
                        var model = _cache.FirstOrDefault(x => x.Name == file.NameWithoutExtension);
                        if (model == null)
                        {
                            model = ModelProvider.LoadModel(file);
                            _cache.Add(model);
                            _renderables.Add(model, new DisplayListModelRenderable(model));
                            //_renderables.Add(model, new GL3ModelRenderable(model));
                        }
                        var renderable = _renderables[model];

                        var c = o.BoundingBox.Center;
                        c.Z = o.BoundingBox.Start.Z;
                        var tl = new Vector3d(c.DX, c.DY, c.DZ);
                        GL.Translate(tl);
                        renderable.Render(viewport);
                        GL.Translate(-tl);

                        return;
                    }
                }
            }
            GL.Color3(o.Colour);
            GL.Begin(BeginMode.Quads);
            foreach (var face in o.BoundingBox.GetBoxFaces())
            {
                foreach (var v in face)
                {
                    GL.Vertex3(v.DX, v.DY, v.DZ);
                }
            }
            GL.End();
        }

        public void AfterRender3D(Viewport3D viewport)
        {

        }

        public void RenderDocument(ViewportBase viewport, Document document)
        {
            throw new System.NotImplementedException();
        }
    }
}