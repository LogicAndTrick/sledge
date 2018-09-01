using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Models
{
    public class Model: IDisposable
    {
        public string Name { get; set; }
        public List<Bone> Bones { get; private set; }
        public List<BodyPart> BodyParts { get; private set; }
        public List<Animation> Animations { get; private set; }
        public List<Texture> Textures { get; set; }
        public bool BonesTransformMesh { get; set; }
        private bool _preprocessed;

        private Box _boundingBox;

        public Model()
        {
            Bones = new List<Bone>();
            BodyParts = new List<BodyPart>();
            Animations = new List<Animation>();
            Textures = new List<Texture>();
            _preprocessed = false;
            
        }

        public Box GetBoundingBox()
        {
            if (_boundingBox == null)
            {
                var transforms = GetTransforms();
                var list = 
                    from mesh in GetActiveMeshes()
                    from vertex in mesh.Vertices
                    let transform = transforms[vertex.BoneWeightings.First().Bone.BoneIndex]
                    let cf = Vector3.Transform(vertex.Location, new Matrix3(transform))
                    select new Coordinate((decimal) cf.X, (decimal) cf.Y, (decimal) cf.Z);
                _boundingBox = new Box(list);
            }
            return _boundingBox;
        }

        public IEnumerable<Mesh> GetActiveMeshes()
        {
            return BodyParts.SelectMany(x => x.GetActiveGroup());
        }

        public void AddMesh(string bodyPartName, int groupid, Mesh mesh)
        {
            var g = BodyParts.FirstOrDefault(x => x.Name == bodyPartName);
            if (g == null)
            {
                g = new BodyPart(bodyPartName);
                BodyParts.Add(g);
            }
            g.AddMesh(groupid, mesh);
        }

        public List<Matrix4> GetTransforms(int animation = 0, int frame = 0)
        {
            if (Animations.Count > animation && animation >= 0)
            {
                var ani = Animations[animation];
                if (ani.Frames.Count > 0)
                {
                    if (frame < 0 || frame >= ani.Frames.Count) frame = 0;
                    var frm = ani.Frames[frame];
                    return frm.GetBoneTransforms(BonesTransformMesh, !BonesTransformMesh);
                }
            }
            return Bones.Select(x => x.Transform).ToList();
        }

        /// <summary>
        /// Preprocess the model for rendering purposes.
        /// Normalises the texture coordinates,
        /// pre-computes chrome texture values, and
        /// combines all the textures into a single bitmap.
        /// </summary>
        public void PreprocessModel()
        {
            if (_preprocessed) return;
            _preprocessed = true;

            PreCalculateChromeCoordinates();
            CombineTextures();
            NormaliseTextureCoordinates();
        }

        /// <summary>
        /// Combines the textures in this model into one bitmap and modifies all the referenced skins and texture coordinates to use the combined texture.
        /// This modifies the model object.
        /// </summary>
        private void CombineTextures()
        {
            if (!Textures.Any()) return;
            // Calculate the dimension of the combined texture
            var width = 0;
            var height = 0;
            var heightList = new Dictionary<int, int>();
            foreach (var texture in Textures)
            {
                width = Math.Max(texture.Width, width);
                heightList.Add(texture.Index, height);
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
            Textures.Insert(0, tex);

            // Update all the meshes with the new texture and alter the texture coordinates as needed
            foreach (var mesh in GetActiveMeshes())
            {
                if (!heightList.ContainsKey(mesh.SkinRef))
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
            // Reset the texture indices
            for (var i = 0; i < Textures.Count; i++)
            {
                Textures[i].Index = i;
            }
        }

        /// <summary>
        /// Pre-calculates chrome texture values for the model.
        /// This operation modifies the model vertices.
        /// </summary>
        private void PreCalculateChromeCoordinates()
        {
            var transforms = Bones.Select(x => x.Transform).ToList();
            foreach (var g in GetActiveMeshes().GroupBy(x => x.SkinRef))
            {
                var skin = Textures.FirstOrDefault(x => x.Index == g.Key);
                if (skin == null || (skin.Flags & 0x02) == 0) continue;
                foreach (var v in g.SelectMany(m => m.Vertices))
                {
                    var transform = transforms[v.BoneWeightings.First().Bone.BoneIndex];

                    // Borrowed from HLMV's StudioModel::Chrome function
                    var tmp = transform.ExtractTranslation().Normalized();

                    // Using unitx for the "player right" vector
                    var up = Vector3.Cross(tmp, Vector3.UnitX).Normalized();
                    var right = Vector3.Cross(tmp, up).Normalized();

                    // HLMV is doing an inverse rotate (no translation),
                    // so we set the shift values to zero after inverting
                    var inv = transform.Inverted();
                    inv.Row3 = Vector4.UnitW;
                    up = Vector3.Transform(up, new Matrix3(inv));
                    right = Vector3.Transform(right, new Matrix3(inv));

                    v.TextureU = (Vector3.Dot(v.Normal, right) + 1) * 32;
                    v.TextureV = (Vector3.Dot(v.Normal, up) + 1) * 32;
                }
            }
        }

        /// <summary>
        /// Normalises vertex texture coordinates to be between 0 and 1.
        /// This operation modifies the model vertices.
        /// </summary>
        private void NormaliseTextureCoordinates()
        {
            foreach (var g in GetActiveMeshes().GroupBy(x => x.SkinRef))
            {
                var skin = Textures.FirstOrDefault(x => x.Index == g.Key);
                if (skin == null) continue;
                foreach (var v in g.SelectMany(m => m.Vertices))
                {
                    v.TextureU /= skin.Width;
                    v.TextureV /= skin.Height;
                }
            }
        }

        public void Dispose()
        {
            foreach (var t in Textures)
            {
                if (t.Image != null) t.Image.Dispose();
            }
        }
    }
}