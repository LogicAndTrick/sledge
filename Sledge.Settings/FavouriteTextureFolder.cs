using System.Collections.Generic;

namespace Sledge.Settings
{
    public class FavouriteTextureFolder
    {
        public string Name { get; set; }
        public List<FavouriteTextureFolder> Children { get; set; }
        public List<string> Items { get; set; }

        public FavouriteTextureFolder()
        {
            Name = "";
            Children = new List<FavouriteTextureFolder>();
            Items = new List<string>();
        }
    }
}