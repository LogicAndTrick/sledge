using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.BspEditor.Rendering.Scene;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.BspEditor.Editing.Commands.Pointfile
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class PointfileConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideLow;
        
        private Pointfile GetPointfile(MapDocument doc)
        {
            return doc.Map.Data.GetOne<Pointfile>();
        }

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Root;
        }

        public async Task<bool> Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var pointfile = GetPointfile(document);
            if (pointfile == null) return true;
            
            var r = 255;
            var g = 127;
            var b = 127;
            var change = 128 / (float) pointfile.Lines.Count;

            for (var i = 0; i < pointfile.Lines.Count; i++)
            {
                var line = pointfile.Lines[i];
                var colour = Color.FromArgb(r, g, b);
                smo.SceneObjects.Add(new object(), new Line(colour, line.Start.ToVector3(), line.End.ToVector3()));

                r = 255 - (int)(change * i);
                b = 127 + (int)(change * i);
            }
            return true;
        }

        public async Task<bool> PropertiesChanged(SceneObjectsChangedEventArgs args, SceneMapObject smo,
            MapDocument document, IMapObject obj, HashSet<string> propertyNames)
        {
            return true;
        }
    }
}