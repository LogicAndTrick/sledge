using System.Collections.Generic;
using System.Linq;
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
            // todo inject these
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

        public static SceneMapObject Convert(Document document, MapObject obj)
        {
            var smo = new SceneMapObject(obj);
            foreach (var converter in Converters.OrderBy(x => (int) x.Priority))
            {
                if (!converter.Supports(obj)) continue;
                if (!converter.Convert(smo, document, obj)) return null;
                if (converter.ShouldStopProcessing(smo, obj)) break;
            }
            return smo;
        }

        public static bool Update(SceneMapObject smo, Document document, MapObject obj)
        {
            foreach (var converter in Converters.OrderBy(x => (int)x.Priority))
            {
                if (!converter.Supports(obj)) continue;
                if (!converter.Update(smo, document, obj)) return false;
                if (converter.ShouldStopProcessing(smo, obj)) break;
            }
            return true;
        }
    }
}