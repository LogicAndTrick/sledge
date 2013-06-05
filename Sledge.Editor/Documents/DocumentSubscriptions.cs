using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Groups;
using Sledge.Editor.Clipboard;
using Sledge.Editor.Compiling;
using Sledge.Editor.History;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools;
using Sledge.Editor.UI;
using Sledge.Providers.Map;
using Sledge.QuickForms.Items;
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
            Mediator.Subscribe(HotkeysMediator.OperationsCut, this);
            Mediator.Subscribe(HotkeysMediator.OperationsPaste, this);
            Mediator.Subscribe(HotkeysMediator.OperationsDelete, this);
            Mediator.Subscribe(HotkeysMediator.GroupingGroup, this);
            Mediator.Subscribe(HotkeysMediator.GroupingUngroup, this);
            Mediator.Subscribe(HotkeysMediator.TieToEntity, this);
            Mediator.Subscribe(HotkeysMediator.TieToWorld, this);
            Mediator.Subscribe(HotkeysMediator.ObjectProperties, this);

            Mediator.Subscribe(EditorMediator.ViewportRightClick, this);

            Mediator.Subscribe(EditorMediator.WorldspawnProperties, this);
        }

        public void Unsubscribe()
        {
            Mediator.UnsubscribeAll(this);
        }

        public void Notify(string message, object data)
        {
            HotkeysMediator val;
            if (ToolManager.ActiveTool != null && Enum.TryParse(message, true, out val))
            {
                var result = ToolManager.ActiveTool.InterceptHotkey(val);
                if (result == HotkeyInterceptResult.Abort) return;
                if (result == HotkeyInterceptResult.SwitchToSelectTool)
                {
                    ToolManager.Activate(ToolManager.Tools.OfType<SelectTool>().First());
                }
            }
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
                _document.Map.WorldSpawn.EntityData.Properties.Add(new Property
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
            Mediator.Publish(EditorMediator.FileOpened, _document.MapFile);
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
            foreach (var kv in _document.Renderer.GridRenderables)
            {
                kv.Value.RebuildGrid(((Viewport2D) kv.Key).Zoom, true);
            }
        }

        public void OperationsCopy()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                ClipboardManager.Push(_document.Selection.GetSelectedObjects());
            }
        }

        public void OperationsCut()
        {
            OperationsCopy();
            OperationsDelete();
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
                o.UpdateBoundingBox();
            }
            _document.Selection.Select(list.SelectMany(x => x.FindAll()));

            var hc = new HistoryCreate("Create", list);
            var hs = new HistorySelect("Select", list, true);
            var hi = new HistoryItemCollection("Pasted " + list.Count + " item" + (list.Count == 1 ? "" : "s"), new IHistoryItem[] { hc, hs });
            _document.History.AddHistoryItem(hi);

            _document.UpdateDisplayLists();
        }

        public void OperationsDelete()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                var sel = _document.Selection.GetSelectedParents().ToList();
                foreach (var o in sel)
                {
                    o.Parent.Children.Remove(o);
                    o.Parent.UpdateBoundingBox();
                }
                _document.Selection.Clear();

                var hs = new HistorySelect("Deselect", sel, false);
                var hd = new HistoryDelete("Delete", sel);
                var hi = new HistoryItemCollection("Removed " + sel.Count + " item" + (sel.Count == 1 ? "" : "s"), new IHistoryItem[] { hs, hd });
                _document.History.AddHistoryItem(hi);

                _document.UpdateDisplayLists();
            }
        }

        public void GroupingGroup()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                var action = new GroupAction(_document.Selection.GetSelectedObjects());
                action.Perform(_document);
                _document.History.AddHistoryItem(new HistoryAction("Grouped objects", action));
                _document.UpdateDisplayLists();
            }
        }

        public void GroupingUngroup()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                var action = new UngroupAction(_document.Selection.GetSelectedObjects());
                action.Perform(_document);
                _document.History.AddHistoryItem(new HistoryAction("Ungrouped objects", action));
                _document.UpdateDisplayLists();
            }
        }

        private class EntityContainer
        {
            public Entity Entity { get; set; }
            public override string ToString()
            {
                var name = Entity.EntityData.Properties.FirstOrDefault(x => x.Key.ToLower() == "targetname");
                if (name != null) return name.Value + " (" + Entity.EntityData.Name + ")";
                return Entity.EntityData.Name;
            }
        }

        public void TieToEntity()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                var entities = _document.Selection.GetSelectedObjects().OfType<Entity>().ToList();

                Entity existing = null;

                var hi = new HistoryItemCollection("Tie to Entity");
                
                if (entities.Count == 1)
                {
                    var result = new QuickForms.QuickForm("Existing Entity in Selection") { Width = 400 }
                        .Label(String.Format("You have selected an existing entity (a '{0}'), how would you like to proceed?", entities[0].ClassName))
                        .Label(" - Keep the existing entity and add the selected items to the entity")
                        .Label(" - Create a new entity and add the selected items to the new entity")
                        .Item(new QuickFormDialogButtons()
                                  .Button("Keep Existing", DialogResult.Yes)
                                  .Button("Create New", DialogResult.No)
                                  .Button("Cancel", DialogResult.Cancel))
                        .ShowDialog();
                    if (result == DialogResult.Yes)
                    {
                        existing = entities[0];
                    }
                }
                else if (entities.Count > 1)
                {
                    var qf = new QuickForms.QuickForm("Multiple Entities Selected") {Width = 400}
                        .Label("You have selected multiple entities, which one would you like to keep?")
                        .ComboBox("Entity", entities.Select(x => new EntityContainer {Entity = x}))
                        .OkCancel();
                    var result = qf.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var cont = qf.Object("Entity") as EntityContainer;
                        if (cont != null) existing = cont.Entity;
                    }
                }

                if (existing == null)
                {
                    existing = new Entity(_document.Map.IDGenerator.GetNextObjectID())
                    {
                        EntityData = new EntityData
                        {
                            Name = _document.Game.DefaultBrushEntity
                        },
                        ClassName = _document.Game.DefaultBrushEntity,
                        Colour = Colour.GetDefaultEntityColour()
                    };
                    existing.SetParent(_document.Map.WorldSpawn);
                    hi.Add(new HistoryCreate("Create entity", new[] { existing }));
                }

                var removed = _document.Selection.GetSelectedObjects().Where(x => x != existing).ToList();
                removed.ForEach(x => x.SetParent(existing));

                hi.Add(new HistorySelect("Deselect", removed, false));
                hi.Add(new HistoryDelete("Delete", removed));
                hi.Add(new HistoryCreate("Create", removed));
                hi.Add(new HistorySelect("Reselect", removed, true));

                _document.History.AddHistoryItem(hi);

                _document.UpdateDisplayLists();
            }
        }

        public void TieToWorld()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                var entities = _document.Selection.GetSelectedObjects().OfType<Entity>().ToList();
                entities.ForEach(x => x.SetParent(null));

                var children = entities.SelectMany(x => x.Children).ToList();
                children.ForEach(x => x.SetParent(_document.Map.WorldSpawn));

                _document.Selection.Clear();
                _document.Selection.Select(children);

                var hs = new HistorySelect("Deselect", entities, false);
                var hd = new HistoryDelete("Delete", entities);
                var ha = new HistoryCreate("Create", children);
                var hr = new HistorySelect("Reselect", children, true);
                var hi = new HistoryItemCollection("Tie to World", new IHistoryItem[] { hs, hd, ha, hr });
                _document.History.AddHistoryItem(hi);

                _document.UpdateDisplayLists();
            }
        }

        public void ObjectProperties()
        {
            var pd = new EntityEditor(_document);
            pd.Show(Editor.Instance);
        }

        public void WorldspawnProperties()
        {
            var pd = new EntityEditor(_document) {FollowSelection = false, AllowClassChange = false};
            pd.SetObjects(new[] {_document.Map.WorldSpawn});
            pd.Show(Editor.Instance);
        }

        public void ViewportRightClick(Viewport2D vp, MouseEventArgs e)
        {
            ViewportContextMenu.Instance.AddNonSelectionItems();
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection && ToolManager.ActiveTool is SelectTool)
            {
                var selectionBoundingBox = _document.Selection.GetSelectionBoundingBox();
                var point = vp.ScreenToWorld(e.X, vp.Height - e.Y);
                var start = vp.Flatten(selectionBoundingBox.Start);
                var end = vp.Flatten(selectionBoundingBox.End);
                if (point.X >= start.X && point.X <= end.X && point.Y >= start.Y && point.Y <= end.Y)
                {
                    // Clicked inside the selection bounds
                    ViewportContextMenu.Instance.AddSelectionItems();
                }
            }
            ViewportContextMenu.Instance.Show(vp, e.X, e.Y);
        }
    }
}