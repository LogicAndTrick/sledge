using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Documents;

namespace Sledge.BspEditor.Documents
{
    /// <summary>
    /// A document that represents a BSP map file.
    /// </summary>
    public class MapDocument : IDocument
    {
        /// <inheritdoc />
        public string Name { get; set; }
        
        private string _fileName;

        /// <inheritdoc />
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;

                var p = System.IO.Path.GetFileName(_fileName);
                if (!String.IsNullOrWhiteSpace(p)) Name = p;
            }
        }

        /// <inheritdoc />
        public object Control => MapDocumentControlHost.Instance;

        /// <inheritdoc />
        public bool HasUnsavedChanges { get; set; }

        /// <summary>
        /// The map for this document
        /// </summary>
        public Map Map { get; }

        /// <summary>
        /// The environment for this document
        /// </summary>
        public IEnvironment Environment { get; }

        private readonly List<Subscription> _subscriptions;

        /// <summary>
        /// A convenience method to get the selection for this document.
        /// If no selection map data object exists, it will be created.
        /// </summary>
        public Selection Selection
        {
            get
            {
                var sel = Map.Data.Get<Selection>().FirstOrDefault();
                if (sel != null) return sel;

                sel = new Selection();
                Map.Data.Add(sel);
                return sel;
            }
        }

        /// <summary>
        /// Create a map document with the given map and environment
        /// </summary>
        /// <param name="map">The map</param>
        /// <param name="environment">The environment</param>
        public MapDocument(Map map, IEnvironment environment)
        {
            FileName = null;
            Name = null;
            Map = map;

            Environment = environment;

            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<IDocument>("Document:Saved", IfThis(Saved)),
                Oy.Subscribe<MapDocumentOperation>("MapDocument:Perform", async c =>
                {
                    if (c.Document == this && !c.Operation.Trivial)
                    {
                        HasUnsavedChanges = true;
                        await Oy.Publish("Document:Changed", this);
                    }
                })
            };
        }

        private Func<IDocument, Task> IfThis(Func<Task> callback)
        {
            return async d =>
            {
                if (d == this) await callback();
            };
        }

        private async Task Saved()
        {
            HasUnsavedChanges = false;
            await Oy.Publish("Document:Changed", this);
        }

        /// <inheritdoc />
        public Task<bool> RequestClose()
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _subscriptions.ForEach(x => x.Dispose());
        }
    }
}