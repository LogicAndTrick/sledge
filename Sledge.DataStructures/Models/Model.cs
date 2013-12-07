using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Sledge.DataStructures.Models
{
    public class Model
    {
        public string Name { get; set; }
        public List<Bone> Bones { get; private set; }
        public List<Mesh> Meshes { get; private set; }
        public List<Animation> Animations { get; private set; }
        public List<Texture> Textures { get; set; }
        public bool BonesTransformMesh { get; set; }

        public Model()
        {
            Bones = new List<Bone>();
            Meshes = new List<Mesh>();
            Animations = new List<Animation>();
            Textures = new List<Texture>();
        }

        /// <summary>
        /// Combines the textures in this model into one bitmap and modifies all the referenced skins and texture coordinates to use the combined texture.
        /// This modifies the model object.
        /// </summary>
        public void CombineTextures()
        {
            if (Textures.Count <= 1) return;

            // Calculate the dimension of the combined texture
            var width = 0;
            var height = 0;
            var heightList = new int[Textures.Count];
            for (var i = 0; i < Textures.Count; i++)
            {
                var texture = Textures[i];
                width = Math.Max(texture.Width, width);
                heightList[i] = height;
                height += texture.Height;
            }

            // Create the combined texture and draw all the textures onto it
            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                var y = 0;
                foreach (var texture in Textures)
                {
                    g.DrawImage(texture.Image, 0, y);
                    y += texture.Height;
                }
            }

            // Create the texture object and replace the existing textures
            var tex = new Texture
            {
                Flags = Textures[0].Flags,
                Height = height,
                Width = width,
                Image = bmp,
                Index = 0,
                Name = "Combined Texture"
            };
            foreach (var texture in Textures)
            {
                texture.Image.Dispose();
            }
            Textures.Clear();
            Textures.Add(tex);

            // Update all the meshes with the new texture and alter the texture coordinates as needed
            foreach (var mesh in Meshes)
            {
                if (heightList.Length <= mesh.SkinRef)
                {
                    mesh.SkinRef = -1;
                    continue;
                }
                var i = mesh.SkinRef;
                var yVal = heightList[i];
                foreach (var v in mesh.Vertices)
                {
                    v.TextureV += yVal;
                }
                mesh.SkinRef = 0;
            }
        }
    }
}