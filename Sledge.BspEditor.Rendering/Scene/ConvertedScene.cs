using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;

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
        private readonly EngineInterface _engine;
        private readonly List<IRenderable> _mapObjects;
        private bool _isActive;

        /// <summary>
        /// Create an instance of this class for the given document.
        /// </summary>
        public ConvertedScene(MapObjectConverter converter, MapDocument document, EngineInterface engine)
        {
            Document = document;
            _converter = converter;
            _engine = engine;

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
                    _engine.Add(smo);
                }
            }
            else
            {
                // Remove the renderables from the scene
                foreach (var smo in _mapObjects)
                {
                    _engine.Remove(smo);
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
                _engine.Add(renderable);
            }
        }

        /// <summary>
        /// Remove a list of renderables from the scene. This does not dispose of the objects.
        /// </summary>
        private void RemoveFromScene(IEnumerable<IRenderable> renderables)
        {
            foreach (var renderable in renderables)
            {
                _engine.Remove(renderable);
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
            var builder = await _converter.Convert(Document, objects);

            // Add before removing - to avoid objects blinking in and out
            var rem = _mapObjects.ToList();
            _mapObjects.Clear();

            // Add new renderables
            _mapObjects.Add(builder.MainRenderable);
            _engine.Add(builder.MainRenderable);

            foreach (var r in builder.Renderables)
            {
                _mapObjects.Add(r);
                _engine.Add(r);
            }

            // Remove old renderables
            foreach (var o in rem)
            {
                _engine.Remove(o);
                o.Dispose();
            }
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