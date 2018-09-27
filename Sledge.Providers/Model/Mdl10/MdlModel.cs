using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Model.Mdl10.Format;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Veldrid;
using Buffer = Sledge.Rendering.Resources.Buffer;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Sledge.Providers.Model.Mdl10
{
    public class MdlModel : IModel
    {
        public MdlFile Model { get; }

        private readonly Guid _guid;
        private uint[][] _bodyPartIndices;

        private Rendering.Resources.Texture _textureResource;
        private Buffer _buffer;
        private uint _numTexturedIndices;
        private uint _numWireframeIndices;

        private string TextureName => $"{nameof(MdlModel)}::{_guid}";

        public MdlModel(MdlFile model)
        {
            Model = model;
            _guid = Guid.NewGuid();
        }

        public List<string> GetSequences()
        {
            return Model.Sequences.Select(x => x.Name).ToList();
        }

        public (Vector3, Vector3) GetBoundingBox(int sequence, int frame, float subframe)
        {
            var transforms = new Matrix4x4[Model.Bones.Count];
            Model.GetTransforms(sequence, frame, subframe, ref transforms);

            var list =
                from part in Model.BodyParts
                from mesh in part.Models[0].Meshes
                from vertex in mesh.Vertices
                let transform = transforms[vertex.VertexBone]
                select Vector3.Transform(vertex.Vertex, transform);

            var box = new Box(list);
            return (box.Start, box.End);
        }

        private static Bitmap CreateBitmap(int width, int height, byte[] data, byte[] palette, bool lastTextureIsTransparent)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // Set palette
            var pal = bmp.Palette;
            for (var j = 0; j <= byte.MaxValue; j++)
            {
                var k = j * 3;
                pal.Entries[j] = Color.FromArgb(255, palette[k], palette[k + 1], palette[k + 2]);
            }

            if (lastTextureIsTransparent)
            {
                pal.Entries[pal.Entries.Length - 1] = Color.Transparent;
            }
            bmp.Palette = pal;

            // Write entries
            var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private List<Rectangle> CreateTexuture(EngineInterface engine, RenderContext context)
        {
            if (!Model.Textures.Any()) return new List<Rectangle>();

            // Combine all the textures into one long texture
            var textures = Model.Textures.Select(x => CreateBitmap(x.Width, x.Height, x.Data, x.Palette, x.Flags.HasFlag(TextureFlags.Masked))).ToList();

            var width = textures.Max(x => x.Width);
            var height = textures.Sum(x => x.Height);

            var rectangles = new List<Rectangle>();

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                var y = 0;
                foreach (var texture in textures)
                {
                    rectangles.Add(new Rectangle(0, y, texture.Width, texture.Height));
                    g.DrawImageUnscaled(texture, 0, y);
                    y += texture.Height;
                }
            }

            textures.ForEach(x => x.Dispose());

            // Upload the texture to the engine
            var lb = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var data = new byte[lb.Stride * lb.Height];
            Marshal.Copy(lb.Scan0, data, 0, data.Length);
            bmp.UnlockBits(lb);

            _textureResource = engine.UploadTexture(TextureName, bmp.Width, bmp.Height, data, TextureSampleType.Standard);

            bmp.Dispose();

            return rectangles;
        }

        public void CreateResources(EngineInterface engine, RenderContext context)
        {
            // textures = model textures 0 - count
            // vertex buffer = all vertices
            // index buffer = indices grouped by texture, in texture order
            // indices per texture - number of indices per texture, in texture order

            var vertices = new List<VertexModel3>();
            var indices = new Dictionary<short, List<uint>>();
            for (short i = 0; i < Model.Textures.Count; i++) indices[i] = new List<uint>();

            var wireframeIndices = new List<uint>();

            var rectangles = CreateTexuture(engine, context);
            var texHeight = rectangles.Max(x => x.Bottom);
            var texWidth = rectangles.Max(x => x.Right);

            _bodyPartIndices = new uint[Model.BodyParts.Count][];

            uint vi = 0;
            var skin = Model.Skins[0].Textures;
            for (var bpi = 0; bpi < Model.BodyParts.Count; bpi++)
            {
                var part = Model.BodyParts[bpi];
                _bodyPartIndices[bpi] = new uint[part.Models.Length];
                
                // Only render the first submodel
                var model = part.Models[0];
                _bodyPartIndices[bpi][0] = (uint) model.Meshes.Sum(x => x.Vertices.Length);

                foreach (var mesh in model.Meshes)
                {
                    var texId = skin[mesh.SkinRef];
                    var rec = rectangles.Count > texId ? rectangles[texId] : Rectangle.Empty;
                    for (var i = 0; i < mesh.Vertices.Length; i++)
                    {
                        var x = mesh.Vertices[i];
                        vertices.Add(new VertexModel3
                        {
                            Position = x.Vertex,
                            Normal = x.Normal,
                            Texture = (x.Texture + new Vector2(rec.X, rec.Y)) / new Vector2(texWidth, texHeight),
                            Bone = (uint) x.VertexBone
                        });
                        indices[texId].Add(vi);
                        wireframeIndices.Add(vi);
                        wireframeIndices.Add(i % 3 == 2 ? vi - 2 : vi + 1);
                        vi++;
                    }
                }
            }

            var flatIndices = new uint[vi + wireframeIndices.Count];
            var currentIndexCount = 0;
            foreach (var kv in indices.OrderBy(x => x.Key))
            {
                var num = kv.Value.Count;
                Array.Copy(kv.Value.ToArray(), 0, flatIndices, currentIndexCount, num);
                currentIndexCount += num;
            }
            Array.Copy(wireframeIndices.ToArray(), 0, flatIndices, currentIndexCount, wireframeIndices.Count);

            _buffer = engine.CreateBuffer();
            _buffer.Update(vertices, flatIndices);

            _numTexturedIndices = (uint) (flatIndices.Length - wireframeIndices.Count);
            _numWireframeIndices = (uint) wireframeIndices.Count;
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            _buffer.Bind(cl, 0);

            if (pipeline.Type == PipelineType.TexturedModel)
            {
                _textureResource.BindTo(cl, 1);
                uint ci = 0;

                foreach (var bpi in _bodyPartIndices)
                {
                    const int model = 0;
                    for (var j = 0; j < bpi.Length; j++)
                    {
                        if (j == model) cl.DrawIndexed(bpi[j], 1, ci, 0, 0);
                        ci += bpi[j];
                    }
                }
            }
            else if (pipeline.Type == PipelineType.WireframeModel)
            {
                cl.DrawIndexed(_numWireframeIndices, 1, _numTexturedIndices, 0, 0);
            }
        }

        public void DestroyResources()
        {
            _buffer?.Dispose();
            _textureResource?.Dispose();
        }

        public void Dispose()
        {
            //
        }
    }
}