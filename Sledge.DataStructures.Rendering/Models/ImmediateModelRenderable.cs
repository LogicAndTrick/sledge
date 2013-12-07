using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Models;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;

namespace Sledge.DataStructures.Rendering.Models
{
    public class DisplayListModelRenderable : ImmediateModelRenderable
    {
        private bool _initialised;
        private readonly string _name;

        public DisplayListModelRenderable(Model model) : base(model)
        {
            _initialised = false;
            _name = "Model/" + Model.Name;
        }

        public override void Dispose()
        {
            base.Dispose();
            DisplayList.Delete(_name);
        }

        public override void Render(object sender)
        {
            if (!_initialised)
            {
                using (DisplayList.Using(_name))
                {
                    base.Render(sender);
                }
                _initialised = true;
            }
            DisplayList.Call(_name);
        }
    }

    public class ImmediateModelRenderable : IRenderable, IDisposable
    {
        public Model Model { get; set; }
        public int CurrentAnimation { get; set; }
        public int CurrentFrame { get; set; }

        private readonly Dictionary<int, ITexture> _textures;  

        public ImmediateModelRenderable(Model model)
        {
            Model = model;
            CurrentAnimation = model.Animations.Count - 1;
            CurrentFrame = 0;

            Model.CombineTextures();
            _textures = new Dictionary<int, ITexture>();
            foreach (var tex in Model.Textures)
            {
                var name = "Model/" + Model.Name + "/" + tex.Name;
                var texture = TextureHelper.Create(name, tex.Image, true);
                _textures.Add(tex.Index, texture);
            }
        }

        public virtual void Dispose()
        {
            foreach (var texture in _textures)
            {
                texture.Value.Dispose();
            }
        }

        public virtual void Render(object sender)
        {
            var transforms = Model.Bones.Select(x => x.Transform).ToList();

            GL.Color4(1f, 1f, 1f, 1f);

            foreach (var group in Model.Meshes.GroupBy(x => x.SkinRef))
            {
                ITexture texture = null;
                if (_textures.ContainsKey(group.Key)) texture = _textures[group.Key];

                if (texture != null) texture.Bind();
                foreach (var mesh in group)
                {
                    GL.Begin(BeginMode.Triangles);
                    foreach (var v in mesh.Vertices)
                    {
                        var transform = transforms[v.BoneWeightings.First().Bone.BoneIndex];
                        var c = v.Location * transform;
                        if (texture != null)
                        {
                            GL.TexCoord2(v.TextureU / texture.Width, v.TextureV / texture.Height);
                        }
                        GL.Vertex3(c.X, c.Y, c.Z);
                    }
                    GL.End();
                }
                if (texture != null) texture.Unbind();
            }
        }
    }
}