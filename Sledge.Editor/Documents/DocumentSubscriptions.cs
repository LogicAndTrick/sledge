using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Clipboard;
using Sledge.Editor.Compiling;
using Sledge.Editor.Rendering;
using Sledge.Editor.UI;
using Sledge.Providers.Map;
using Sledge.Settings;
using Sledge.UI;
using Path = System.IO.Path;

namespace Sledge.Editor.Documents
{
    /// <summary>
    /// A simple container to separate out the document mediator listeners from the document itself.
    /// </summary>
    public class DocumentSubscriptions : IMediatorListener
    {
        private readonly Document _document;

        public DocumentSubscriptions(Document document)
        {
            _document = document;
        }

        public void Subscribe()
        {
            Mediator.Subscribe(HotkeysMediator.HistoryUndo, this);
            Mediator.Subscribe(HotkeysMediator.HistoryRedo, this);
            Mediator.Subscribe(HotkeysMediator.FileCompile, this);
            Mediator.Subscribe(HotkeysMediator.FileSave, this);
            Mediator.Subscribe(HotkeysMediator.GridIncrease, this);
            Mediator.Subscribe(HotkeysMediator.GridDecrease, this);
            Mediator.Subscribe(HotkeysMediator.OperationsCopy, this);
            Mediator.Subscribe(HotkeysMediator.OperationsPaste, this);
        }

        public void Unsubscribe()
        {
            Mediator.UnsubscribeAll(this);
        }

        public void Notify(string message, object data)
        {
            if (!Mediator.ExecuteDefault(this, message, data))
            {
                throw new Exception("Invalid document message: " + message + ", with data: " + data);
            }
        }

        public void HistoryUndo()
        {
            _document.History.Undo();
        }

        public void HistoryRedo()
        {
            _document.History.Redo();
        }

        public void FileCompile()
        {
            FileSave();
            var currentFile = _document.MapFile;
            if (currentFile == null) return;
            if (!currentFile.EndsWith("map"))
            {
                _document.Map.WorldSpawn.EntityData.Properties.Add(new DataStructures.MapObjects.Property
                                                                       {
                                                                           Key = "wad",
                                                                           Value = string.Join(";", _document.Game.Wads.Select(x => x.Path))
                                                                       });
                var map = Path.ChangeExtension(_document.MapFile, "map");
                MapProvider.SaveMapToFile(map, _document.Map);
                currentFile = map;
            }
            var batch = new Batch(_document.Game, currentFile);
            BatchCompiler.Compile(batch);
        }

        public void FileSave()
        {
            var currentFile = _document.MapFile;
            if (currentFile == null)
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = @"RMF Files (*.rmf)|*.rmf";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        currentFile = sfd.FileName;
                    }
                }
            }
            if (currentFile == null) return;

            MapProvider.SaveMapToFile(currentFile, _document.Map);
            _document.MapFile = currentFile;
        }

        public void GridIncrease()
        {
            var curr = _document.GridSpacing;
            if (curr >= 1024) return;
            _document.GridSpacing *= 2;
            RebuildGrid();
        }

        public void GridDecrease()
        {
            var curr = _document.GridSpacing;
            if (curr <= 1) return;
            _document.GridSpacing /= 2;
            RebuildGrid();
        }

        public void RebuildGrid()
        {
            foreach (var vp in ViewportManager.Viewports.OfType<Viewport2D>())
            {
                var grid = vp.RenderContext.FindRenderable<GridRenderable>();
                if (grid != null) grid.RebuildGrid(vp.Zoom, true);
            }
        }

        public void OperationsCopy()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                ClipboardManager.Push(_document.Selection.GetSelectedObjects());
            }
        }

        public void OperationsPaste()
        {
            var content = ClipboardManager.GetPastedContent(_document);
            if (content == null) return;

            var list = content.ToList();
            if (!list.Any()) return;

            _document.Selection.SwitchToObjectSelection();
            _document.Selection.Clear();
            foreach (var o in list)
            {
                o.Parent = _document.Map.WorldSpawn;
                _document.Map.WorldSpawn.Children.Add(o);
            }
            _document.Selection.Select(list.SelectMany(x => x.FindAll()));
        }
    }
}