using System.Collections.Generic;
using System.Drawing;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Rendering.Internal
{
    internal static class InternalMaterials
    {
        public static List<Material> GetInternalMaterials()
        {
            return new List<Material>
                   {
                       Material.Flat(Color.White),
                       Material.Texture("Internal::White", false),
                       Material.Texture("Internal::Debug", false),
                       Material.Texture(HandleElement.SquareHandleTextureName, false),
                   };
        }
    }
}