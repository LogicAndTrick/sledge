using System.ComponentModel.Composition;
using System.Drawing;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
    public class EntitySprite : IMapObjectData, IContentsReplaced
    {
        public string Name { get; set; }
        public float Scale { get; }
        public Color Color { get; set; }

        public bool ContentsReplaced => !string.IsNullOrWhiteSpace(Name);

        public EntitySprite(string name, float scale, Color color)
        {
            Name = name;
            Scale = scale;
            Color = color;
        }

        public EntitySprite(SerialisedObject obj)
        {
            Name = obj.Get<string>("Name");
            Scale = obj.Get<float>("Scale");
            Color = obj.GetColor("Color");
        }

        [Export(typeof(IMapElementFormatter))]
        public class ActiveTextureFormatter : StandardMapElementFormatter<Origin> { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Scale", Scale);
            info.AddValue("Color", Color);
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new EntitySprite(Name, Scale, Color);
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject(nameof(EntitySprite));
            so.Set(nameof(Name), Name);
            so.Set(nameof(Scale), Scale);
            so.SetColor(nameof(Color), Color);
            return so;
        }
    }
}