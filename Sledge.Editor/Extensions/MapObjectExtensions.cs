using Sledge.Common;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Extensions
{
    public static class MapObjectExtensions
    {
        private const string DecalMetaKey = "Decal";

        public static void SetDecal(this Entity entity, ITexture texture)
        {
            entity.MetaData.Set(DecalMetaKey, texture);
        }

        public static ITexture GetDecal(this Entity entity)
        {
            return entity.MetaData.Get<ITexture>(DecalMetaKey);
        }

        public static bool HasDecal(this Entity entity)
        {
            return entity.MetaData.Has<ITexture>(DecalMetaKey);
        }
    }
}
