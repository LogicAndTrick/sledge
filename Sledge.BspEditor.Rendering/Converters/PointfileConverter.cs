using System.Drawing;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.MapObjects;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.BspEditor.Rendering.Converters
{
    public class PointfileConverter : IMapObjectSceneConverter
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

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            if (document.Pointfile == null) return true;

            var r = 255;
            var g = 127;
            var b = 127;
            var change = 128 / (float)document.Pointfile.Lines.Count;

            for (int i = 0; i < document.Pointfile.Lines.Count; i++)
            {
                var line = document.Pointfile.Lines[i];
                var colour = Color.FromArgb(r, g, b);
                smo.SceneObjects.Add(new object(), new Line(colour, line.Start.ToVector3(), line.End.ToVector3()));

                r = 255 - (int)(change * i);
                b = 127 + (int)(change * i);
            }
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }
    }
}