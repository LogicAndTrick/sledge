using System.Drawing;
using System.Threading.Tasks;
using OpenTK;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes.Renderables;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.Editor.Rendering.Converters
{
    public class AxisLinesConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority { get { return MapObjectSceneConverterPriority.OverrideLow; } }

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return obj is World;
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            smo.SceneObjects.Add(new Holder(), new Line(Color.FromArgb(255, Color.Red), Vector3.Zero, Vector3.UnitX * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            smo.SceneObjects.Add(new Holder(), new Line(Color.FromArgb(255, Color.Lime), Vector3.Zero, Vector3.UnitY * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            smo.SceneObjects.Add(new Holder(), new Line(Color.FromArgb(255, Color.Blue), Vector3.Zero, Vector3.UnitZ * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            return true;
        }

        private class Holder { }
    }
}