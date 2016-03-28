using System.Collections.Generic;
using Sledge.Rendering.DataStructures.Models;

namespace Sledge.Rendering.Interfaces
{
    public class ModelDetails
    {
        public string Name { get; private set; }
        public Model Model { get; private set; }
        public List<TextureDetails> Textures { get; set; }

        public ModelDetails(string name, Model model, List<TextureDetails> textures)
        {
            Name = name;
            Model = model;
            Textures = textures;
        }
    }
}