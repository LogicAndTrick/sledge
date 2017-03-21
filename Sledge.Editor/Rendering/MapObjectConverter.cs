using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering.Converters;

namespace Sledge.Editor.Rendering
{
    public static class MapObjectConverter
    {
        private static readonly List<IMapObjectSceneConverter> Converters;
         
        static MapObjectConverter()
        {
            // todo !plugin inject scene converters
            Converters = new List<IMapObjectSceneConverter>();
            Converters.Add(new HiddenConverter());
            Converters.Add(new DefaultSolidConverter());
            Converters.Add(new DefaultEntityConverter());
            Converters.Add(new EntityDecalConverter());
            Converters.Add(new EntitySpriteConverter());
            Converters.Add(new EntityNameConverter());
            Converters.Add(new EntityAngleConverter());
            Converters.Add(new EntityModelConverter());
            Converters.Add(new CenterHandlesConverter());
            Converters.Add(new ViewportTextConverter());
            Converters.Add(new AxisLinesConverter());
            Converters.Add(new GridConverter());
            Converters.Add(new CordonConverter());
            Converters.Add(new PointfileConverter());
        }

        public static async Task<SceneMapObject> Convert(Document document, MapObject obj)
        {
            var smo = new SceneMapObject(obj);
            foreach (var converter in Converters.OrderBy(x => (int) x.Priority))
            {
                if (!converter.Supports(obj)) continue;
                if (!await converter.Convert(smo, document, obj)) return null;
                if (converter.ShouldStopProcessing(smo, obj)) break;
            }
            return smo;
        }

        public static async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            foreach (var converter in Converters.OrderBy(x => (int)x.Priority))
            {
                if (!converter.Supports(obj)) continue;
                if (!await converter.Update(smo, document, obj)) return false;
                if (converter.ShouldStopProcessing(smo, obj)) break;
            }
            return true;
        }
    }
}