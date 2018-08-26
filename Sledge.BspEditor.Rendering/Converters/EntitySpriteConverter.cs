using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.ChangeHandlers;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class EntitySpriteConverter : IMapObjectSceneConverter
    {
        [Import] private EngineInterface _engine;

        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLow;

        public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Entity e && GetSpriteData(e) != null;
        }

        private EntitySprite GetSpriteData(Entity e)
        {
            var es = e.Data.GetOne<EntitySprite>();
            return es != null && es.ContentsReplaced ? es : null;
        }

        public async Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj)
        {
            var entity = (Entity) obj;
            var tc = await document.Environment.GetTextureCollection();

            var sd = GetSpriteData(entity);
            if (sd == null || !sd.ContentsReplaced) return;

            var name = sd.Name;
            var scale = sd.Scale;

            var width = entity.BoundingBox.Width;
            var height = entity.BoundingBox.Height;

            var t = await tc.GetTextureItem("aaatrigger"); // todo

            var texture = $"{document.Environment.ID}::{name}";
            if (t != null)
            {
                _engine.UploadTexture(texture, () => new EnvironmentTextureSource(document.Environment, t));

                width = t.Width;
                height = t.Height;
            }

            width *= scale;
            height *= scale;

            var tint = sd.Color.ToVector4();
            tint = Vector4.One - tint;
            tint.W = 1;

            builder.Append(
                new [] { new VertexStandard { Position = entity.Origin, Normal = new Vector3(width, height, 0), Colour = Vector4.One, Tint = tint } },
                new [] { 0u },
                new [] { new BufferGroup(PipelineType.TexturedBillboard, CameraType.Perspective, false, Vector3.Zero, texture, 0, 1) }
            );

        }
    }
}