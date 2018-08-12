using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<long, SceneMapObject> _mapObjects;
        private bool _isActive;

        /// <summary>
        /// Create an instance of this class for the given document.
        /// </summary>
        public ConvertedScene(MapObjectConverter converter, MapDocument document)
        {
            Document = document;
            _converter = converter;

            _isActive = false;
            _mapObjects = new ConcurrentDictionary<long, SceneMapObject>();

            Update(document.Map.Root.FindAll(), new IMapObject[0], new IMapObject[0]);
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
                foreach (var smo in _mapObjects.Values)
                {
                    AddToScene(smo.Renderables.Values);
                }
            }
            else
            {
                // Remove the renderables from the scene
                foreach (var smo in _mapObjects.Values)
                {
                    RemoveFromScene(smo.Renderables.Values);
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
            await Update(change.Added, change.Updated, change.Removed);
        }

        private async Task Update(IEnumerable<IMapObject> created, IEnumerable<IMapObject> updated, IEnumerable<IMapObject> deleted)
        {
            // Add new objects
            foreach (var c in created)
            {
                var smo = await _converter.Convert(Document, c);
                _mapObjects[c.ID] = smo;

                // If the scene is active, add them straight into the scene
                if (_isActive) AddToScene(smo.Renderables.Values);
            }

            // Update existing objects
            foreach (var u in updated)
            {
                // The object might not be in the scene yet, check if it is
                if (_mapObjects.TryGetValue(u.ID, out var smo))
                {
                    // Yep, it is, update in place
                    var renderables = smo.Renderables.Values.ToList();
                    var inPlaceChange = await _converter.Update(smo, Document, u);

                    // If the scene is active and it wasn't an in-place change, remove the old renderables and add the new set
                    if (_isActive && !inPlaceChange)
                    {
                        RemoveFromScene(renderables.Except(smo.Renderables.Values));
                        AddToScene(smo.Renderables.Values.Except(renderables));
                    }
                }
                else
                {
                    // Object isn't in the scene, convert to new renderables
                    smo = await _converter.Convert(Document, u);
                    _mapObjects[u.ID] = smo;

                    // If the scene is active, add them straight into the scene
                    if (_isActive) AddToScene(smo.Renderables.Values);
                }
            }

            // Remove and dispose any deleted objects
            foreach (var d in deleted)
            {
                if (_mapObjects.TryRemove(d.ID, out var smo) && _isActive)
                {
                    RemoveFromScene(smo.Renderables.Values);
                    smo.Dispose();
                }
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            foreach (var o in _mapObjects.Values)
            {
                RemoveFromScene(o.Renderables.Values);
                o.Dispose();
            }

            _mapObjects.Clear();
        }
    }
}