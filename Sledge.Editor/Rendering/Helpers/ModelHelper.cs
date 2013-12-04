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
        public bool Is2DHelper { get { return false; } }
        public bool Is3DHelper { get { return true; } }
        public bool IsDocumentHelper { get { return false; } }
        public HelperType HelperType { get { return HelperType.Replace; } }

        private Model _model;
        private ModelRenderable _renderable;
        public ModelHelper()
        {
            _model = ModelProvider.LoadModel(new NativeFile(@"D:\Github\sledge\_Resources\MDL\HL1_10\barney.mdl"));
            _renderable = new ModelRenderable(_model);
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

        public void Render3D(Viewport3D viewport, MapObject o)
        {
            _renderable.Render(viewport);
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