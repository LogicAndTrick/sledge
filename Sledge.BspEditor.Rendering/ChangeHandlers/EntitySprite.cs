using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
    public class EntitySprite : IMapObjectData, IContentsReplaced, IBoundingBoxProvider
    {
        public string Name { get; set; }
        public float Scale { get; }
        public Color Color { get; set; }
        public SizeF Size { get; }

        public bool ContentsReplaced => !string.IsNullOrWhiteSpace(Name);

        public EntitySprite(string name, float scale, Color color, SizeF? size)
        {
            Name = name;
            Scale = scale;
            Color = color;
            Size = size ?? SizeF.Empty;
        }

        public EntitySprite(SerialisedObject obj)
        {
            Name = obj.Get<string>("Name");
            Scale = obj.Get<float>("Scale");
            Color = obj.GetColor("Color");
            Size = new SizeF(obj.Get<float>("Width"), obj.Get<float>("Height"));
        }

        [Export(typeof(IMapElementFormatter))]
        public class ActiveTextureFormatter : StandardMapElementFormatter<EntitySprite> { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Scale", Scale);
            info.AddValue("Color", Color);
            info.AddValue("Width", Size.Width);
            info.AddValue("Height", Size.Height);
        }

        public Box GetBoundingBox(IMapObject obj)
        {
            if (string.IsNullOrWhiteSpace(Name) || Size.IsEmpty) return null;
            var origin = obj.Data.GetOne<Origin>()?.Location ?? Vector3.Zero;
            var half = new Vector3(Size.Width, Size.Width, Size.Height) * Scale / 2;
            return new Box(origin - half, origin + half);
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new EntitySprite(Name, Scale, Color, Size);
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject(nameof(EntitySprite));
            so.Set(nameof(Name), Name);
            so.Set(nameof(Scale), Scale);
            so.SetColor(nameof(Color), Color);
            so.Set("Width", Size.Width);
            so.Set("Height", Size.Height);
            return so;
        }
    }
}