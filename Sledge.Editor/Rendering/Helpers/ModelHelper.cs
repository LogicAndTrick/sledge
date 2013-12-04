using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Models;
using Sledge.DataStructures.Rendering;
using Sledge.Editor.Documents;
using Sledge.FileSystem;
using Sledge.Providers.Model;
using Sledge.UI;

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
 
        public void Render3D(Viewport3D viewport, MapObject o)
        {
            var e = (Entity) o;
            if (e.GameData == null) return;
            var studio = e.GameData.Behaviours.FirstOrDefault(x => x.Name == "studio");
            if (studio == null || studio.Values.Count != 1 || studio.Values[0].Trim() == "") return;
            var path = studio.Values[0].Trim();

            // todo cache these!
            var file = Document.Environment.Root.TraversePath(path);
            if (file == null) return;


            var model = _cache.FirstOrDefault(x => x.Name == file.NameWithoutExtension);
            if (model == null)
            {
                model = ModelProvider.LoadModel(file);
                _cache.Add(model);
            }
            var renderable = new ModelRenderable(model);
            renderable.Render(viewport);
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