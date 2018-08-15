using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Scene
{
    /// <summary>
    /// The container for <see cref="IMapObject"/>s that have been converted into <see cref="IRenderable"/>s for the engine.
    /// This class handles converting document changes into engine object changes for a document.
    /// </summary>
    /// <seealso cref="SceneManager">The SceneManager contains a collection of this class.</seealso>
    public class ConvertedScene : IDisposable
    {
        /// <summary>
        /// The document this class is managing.
        /// </summary>
        public MapDocument Document { get; }

        private readonly MapObjectConverter _converter;
        private readonly List<IRenderable> _mapObjects;
        private bool _isActive;

        /// <summary>
        /// Create an instance of this class for the given document.
        /// </summary>
        public ConvertedScene(MapObjectConverter converter, MapDocument document)
        {
            Document = document;
            _converter = converter;

            _isActive = false;
            _mapObjects = new List<IRenderable>();

            Update(document.Map.Root.FindAll());
        }

        /// <summary>
        /// Set the document's active state and add/remove the document's renderables from the scene.
        /// </summary>
        /// <param name="active">Value of the active flag</param>
        public void SetActive(bool active)
        {
            // If already at this state, don't do anything
            if (_isActive == active) return;

            if (active)
            {
                // Add the renderables to the scene
                foreach (var smo in _mapObjects)
                {
                    Engine.Instance.Scene.Add(smo);
                }
            }
            else
            {
                // Remove the renderables from the scene
                foreach (var smo in _mapObjects)
                {
                    Engine.Instance.Scene.Remove(smo);
                }
            }
            _isActive = active;
        }
        
        /// <summary>
        /// Add renderables to a scene.
        /// </summary>
        private void AddToScene(IEnumerable<IRenderable> renderables)
        {
            foreach (var renderable in renderables)
            {
                Engine.Instance.Scene.Add(renderable);
            }
        }

        /// <summary>
        /// Remove a list of renderables from the scene. This does not dispose of the objects.
        /// </summary>
        private void RemoveFromScene(IEnumerable<IRenderable> renderables)
        {
            foreach (var renderable in renderables)
            {
                Engine.Instance.Scene.Remove(renderable);
            }
        }

        /// <summary>
        /// Process a change object and pass them to the engine.
        /// </summary>
        public async Task Update(Change change)
        {
            await Update(change.Document.Map.Root.FindAll());
        }

        private async Task Update(IEnumerable<IMapObject> objects)
        {
            var buffer = new EngineInterface().CreateBufferBuilder();

            foreach (var solid in objects.OfType<Solid>())
            {
                // Pack the vertices like this [ f1v1 ... f1vn ] ... [ fnv1 ... fnvn ]
                var numVertices = (uint)solid.Faces.Sum(x => x.Vertices.Count);

                // Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
                var numSolidIndices = (uint)solid.Faces.Sum(x => (x.Vertices.Count - 2) * 3);
                var numWireframeIndices = numVertices * 2;

                var points = new VertexStandard4[numVertices];
                var indices = new uint[numSolidIndices + numWireframeIndices];

                var c = solid.IsSelected ? Color.Red : solid.Color.Color;
                var colour = new Vector4(c.R, c.G, c.B, c.A) / 255f;

                var tc = Document.Environment.GetTextureCollection().Result;

                var vi = 0u;
                var si = 0u;
                var wi = numSolidIndices;
                foreach (var face in solid.Faces)
                {
                    var t = tc.GetTextureItem(face.Texture.Name).Result;
                    var w = t?.Width ?? 0;
                    var h = t?.Height ?? 0;

                    var offs = vi;
                    var numFaceVerts = (uint)face.Vertices.Count;

                    var textureCoords = face.GetTextureCoordinates(w, h).ToList();

                    var normal = face.Plane.Normal;
                    for (var i = 0; i < face.Vertices.Count; i++)
                    {
                        var v = face.Vertices[i];
                        points[vi++] = new VertexStandard4
                        {
                            Position = v,
                            Colour = colour,
                            Normal = normal,
                            Texture = new Vector2(textureCoords[i].Item2, textureCoords[i].Item3)
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
                //groups.Add(new BufferGroup(PipelineNames.FlatColourGeneric + ".", 0, numSolidIndices));

                uint texOffset = 0;
                foreach (var f in solid.Faces)
                {
                    var texture = $"{Document.Environment.ID}::{f.Texture.Name}";
                    var texInd = (uint) (f.Vertices.Count - 2) * 3;
                    groups.Add(new BufferGroup(PipelineType.TexturedGeneric, texture, texOffset, texInd));
                    texOffset += texInd;

                    new EngineInterface().CreateTexture(texture, () => new EnvironmentTextureSource(Document.Environment, f.Texture.Name));
                }

                groups.Add(new BufferGroup(PipelineType.WireframeGeneric, numSolidIndices, numWireframeIndices));

                buffer.Append(points, indices, groups);
            }

            buffer.Complete();

            var render = new BufferBuilderRenderable(buffer);


            foreach (var o in _mapObjects)
            {
                Engine.Instance.Scene.Remove(o);
                o.Dispose();
            }
            _mapObjects.Clear();

            _mapObjects.Add(render);
            Engine.Instance.Scene.Add(render);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            RemoveFromScene(_mapObjects);

            foreach (var o in _mapObjects)
            {
                o.Dispose();
            }

            _mapObjects.Clear();
        }
    }
}