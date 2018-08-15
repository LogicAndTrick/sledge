using System.ComponentModel.Composition;
using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Buffer = Sledge.Rendering.Renderables.Buffer;
using Sledge.BspEditor.Environment;
using Sledge.Providers.Texture;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class DefaultSolidConverter : IMapObjectSceneConverter
    {
        private static readonly object Holder1 = new object();
        private static readonly object Holder2 = new object();
        [Import] private EngineInterface _engine;

        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLowest;

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Solid;
        }

        private const string MaxVertCountMetaDataString = nameof(DefaultSolidConverter) + "MaxVertCount";

        public Task Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var solid = (Solid) obj;
            var buffer = _engine.CreateBuffer();

            var numVertices = solid.Faces.Sum(x => x.Vertices.Count);
            var numSolidIndices = solid.Faces.Sum(x => (x.Vertices.Count - 2) * 3);
            var numWireframeIndices = numVertices * 2;

            UpdateBuffer(solid, buffer);
            smo.Buffers.Add(Holder1, buffer);

            smo.Renderables.Add(Holder1, new SimpleRenderable(buffer, PipelineType.FlatColourGeneric, 0, numSolidIndices));
            smo.Renderables.Add(Holder2, new SimpleRenderable(buffer, PipelineType.WireframeGeneric, numSolidIndices, numWireframeIndices));

            UpdateRenderables(solid, smo, document);
            
            smo.MetaData[MaxVertCountMetaDataString] = numVertices;

            return Task.FromResult(0);
        }

        public Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            var solid = (Solid)obj;

            var buffer = smo.Buffers[Holder1];

            var numVertices = solid.Faces.Sum(x => x.Vertices.Count);

            // Recreate the buffer if it's too small
            var maxVertCount = (int) smo.MetaData[MaxVertCountMetaDataString];
            if (maxVertCount < numVertices)
            {
                buffer.Dispose();
                buffer = _engine.CreateBuffer();
                smo.Buffers[Holder1] = buffer;
                smo.MetaData[MaxVertCountMetaDataString] = numVertices;
            }

            UpdateBuffer(solid, buffer);
            UpdateRenderables(solid, smo, document);

            return Task.FromResult(true);
        }

        private static void UpdateBuffer(Solid solid, Buffer buffer)
        {
            // Pack the vertices like this [ f1v1 ... f1vn ] ... [ fnv1 ... fnvn ]
            var numVertices = (uint) solid.Faces.Sum(x => x.Vertices.Count);

            // Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
            var numSolidIndices = (uint) solid.Faces.Sum(x => (x.Vertices.Count - 2) * 3);
            var numWireframeIndices = numVertices * 2;

            var points = new VertexStandard4[numVertices];
            var indices = new uint[numSolidIndices + numWireframeIndices];

            var c = solid.IsSelected ? Color.Red : solid.Color.Color;
            var colour = new Vector4(c.R, c.G, c.B, c.A) / 255f;

            var vi = 0u;
            var si = 0u;
            var wi = numSolidIndices;
            foreach (var face in solid.Faces)
            {
                var offs = vi;
                var numFaceVerts = (uint) face.Vertices.Count;

                var normal = face.Plane.Normal;
                foreach (var v in face.Vertices)
                {
                    points[vi++] = new VertexStandard4
                    {
                        Position = v,
                        Colour = colour,
                        Normal = normal,
                        Texture = Vector2.Zero
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

            buffer.Update(points, indices);
        }

        private void UpdateRenderables(Solid solid, SceneMapObject smo, MapDocument document)
        {
            var numVertices = solid.Faces.Sum(x => x.Vertices.Count);
            var numSolidIndices = solid.Faces.Sum(x => (x.Vertices.Count - 2) * 3);
            var numWireframeIndices = numVertices * 2;

            var solidRenderable = (SimpleRenderable) smo.Renderables[Holder1];
            var wireframeRenderable = (SimpleRenderable) smo.Renderables[Holder2];

            solidRenderable.IndexOffset = 0;
            solidRenderable.IndexCount = numSolidIndices;

            wireframeRenderable.IndexOffset = numSolidIndices;
            wireframeRenderable.IndexCount = numWireframeIndices;
        }

    }

    public class EnvironmentTextureSource : ITextureDataSource
    {
        public int Width => _item.Width;
        public int Height => _item.Height;

        private readonly TextureCollection _textureCollection;
        private readonly TextureItem _item;

        public EnvironmentTextureSource(IEnvironment environment, string name)
        {
            _textureCollection = environment.GetTextureCollection().Result;
            _item = _textureCollection.GetTextureItem(name).Result;
        }

        public byte[] GetData()
        {
            using (var bitmap = _textureCollection.GetStreamSource().GetImage(_item.Name, 512, 512).Result)
            {
                var lb = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var data = new byte[lb.Stride * lb.Height];
                Marshal.Copy(lb.Scan0, data, 0, data.Length);
                bitmap.UnlockBits(lb);
                return data;
            }
        }
    }
}