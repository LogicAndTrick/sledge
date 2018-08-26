using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Providers.Texture;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class DefaultSolidConverter : IMapObjectSceneConverter
    {
        [Import] private EngineInterface _engine;

        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLowest;

        public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Solid;
        }

        public async Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj)
        {
            var displayFlags = document.Map.Data.GetOne<DisplayFlags>();
            var hideNull = displayFlags?.HideNullTextures == true;

            var solid = (Solid) obj;

            // Pack the vertices like this [ f1v1 ... f1vn ] ... [ fnv1 ... fnvn ]
            var numVertices = (uint)solid.Faces.Sum(x => x.Vertices.Count);

            // Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
            var numSolidIndices = (uint)solid.Faces.Sum(x => (x.Vertices.Count - 2) * 3);
            var numWireframeIndices = numVertices * 2;

            var points = new VertexStandard[numVertices];
            var indices = new uint[numSolidIndices + numWireframeIndices];

            var c = solid.IsSelected ? Color.Red : solid.Color.Color;
            var colour = new Vector4(c.R, c.G, c.B, c.A) / 255f;

            c = solid.IsSelected ? Color.FromArgb(255, 128, 128) : Color.White;
            var tint = new Vector4(c.R, c.G, c.B, c.A) / 255f;

            var tc = await document.Environment.GetTextureCollection();

            var flags = solid.IsSelected ? VertexFlags.SelectiveTransformed : VertexFlags.None;

            var vi = 0u;
            var si = 0u;
            var wi = numSolidIndices;
            foreach (var face in solid.Faces)
            {
                var opacity = tc.GetOpacity(face.Texture.Name);
                var t = await tc.GetTextureItem(face.Texture.Name);
                var w = t?.Width ?? 0;
                var h = t?.Height ?? 0;

                var tintModifier = new Vector4(0, 0, 0, 1 - opacity);

                var offs = vi;
                var numFaceVerts = (uint)face.Vertices.Count;

                var textureCoords = face.GetTextureCoordinates(w, h).ToList();

                var normal = face.Plane.Normal;
                for (var i = 0; i < face.Vertices.Count; i++)
                {
                    var v = face.Vertices[i];
                    points[vi++] = new VertexStandard
                    {
                        Position = v,
                        Colour = colour,
                        Normal = normal,
                        Texture = new Vector2(textureCoords[i].Item2, textureCoords[i].Item3),
                        Tint = tint - tintModifier,
                        Flags = flags
                    };
                }

                // Triangles - [0 1 2]  ... [0 n-1 n]
                for (uint i = 2; i < numFaceVerts; i++)
                {
                    indices[si++] = offs;
                    indices[si++] = offs + i - 1;
                    indices[si++] = offs + i;
                }

                // Lines - [0 1] ... [n-1 n] [n 0]
                for (uint i = 0; i < numFaceVerts; i++)
                {
                    indices[wi++] = offs + i;
                    indices[wi++] = offs + (i == numFaceVerts - 1 ? 0 : i + 1);
                }
            }

            var groups = new List<BufferGroup>();

            uint texOffset = 0;
            foreach (var f in solid.Faces)
            {
                var texInd = (uint)(f.Vertices.Count - 2) * 3;

                if (hideNull && tc.IsNullTexture(f.Texture.Name))
                {
                    texOffset += texInd;
                    continue;
                }

                var opacity = tc.GetOpacity(f.Texture.Name);
                var t = await tc.GetTextureItem(f.Texture.Name);
                var transparent = opacity < 0.95f || t?.Flags.HasFlag(TextureFlags.Transparent) == true;

                var texture = $"{document.Environment.ID}::{f.Texture.Name}";
                groups.Add(new BufferGroup(t == null ? PipelineType.FlatColourGeneric : PipelineType.TexturedGeneric, CameraType.Perspective, transparent, f.Origin, texture, texOffset, texInd));
                texOffset += texInd;

                if (t != null)
                {
                    _engine.UploadTexture(texture, () => new EnvironmentTextureSource(document.Environment, t));
                }
            }

            // groups.Add(new BufferGroup(PipelineType.FlatColourGeneric, 0, numSolidIndices));
            groups.Add(new BufferGroup(PipelineType.WireframeGeneric, solid.IsSelected ? CameraType.Both : CameraType.Orthographic, false, solid.BoundingBox.Center, numSolidIndices, numWireframeIndices));
            
            builder.Append(points, indices, groups);

            _engine.UploadTexture(CenterHandleTextureDataSource.Name, () => HandleDataSource);
            builder.Append(
                new [] { new VertexStandard { Position =  solid.BoundingBox.Center, Normal = new Vector3(9, 9, 0), Colour = colour, Tint = Vector4.One } },
                new [] { 0u },
                new [] {new BufferGroup(PipelineType.TexturedBillboard, CameraType.Orthographic, false, solid.BoundingBox.Center, CenterHandleTextureDataSource.Name, 0, 1) }
            );
        }

        private static readonly CenterHandleTextureDataSource HandleDataSource = new CenterHandleTextureDataSource();
        private class CenterHandleTextureDataSource : ITextureDataSource
        {
            public const string Name = "DefaultSolidConverter::CenterHandle::X";
            private readonly byte[] _data;

            public TextureSampleType SampleType => TextureSampleType.Point;
            public int Width => 9;
            public int Height => 9;

            public CenterHandleTextureDataSource()
            {
                using (var img = new Bitmap(Width, Height))
                {
                    using (var g = Graphics.FromImage(img))
                    {
                        g.FillRectangle(Brushes.Transparent, 0, 0, img.Width, img.Height);
                        g.DrawLine(Pens.White, 1, 1, img.Width - 2, img.Height - 2);
                        g.DrawLine(Pens.White, img.Width - 2, 1, 1, img.Height - 2);
                    }
                    var lb = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    _data = new byte[lb.Stride * lb.Height];
                    Marshal.Copy(lb.Scan0, _data, 0, _data.Length);
                    img.UnlockBits(lb);
                }
            }

            public Task<byte[]> GetData()
            {
                return Task.FromResult(_data);
            }
        }
    }
}