using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Groups;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Actions.Visgroups;
using Sledge.Editor.Clipboard;
using Sledge.Editor.Compiling;
using Sledge.Editor.Enums;
using Sledge.Editor.Tools;
using Sledge.Editor.UI;
using Sledge.Editor.Visgroups;
using Sledge.Extensions;
using Sledge.Providers.Map;
using Sledge.QuickForms;
using Sledge.QuickForms.Items;
using Sledge.Settings;
using Sledge.UI;
using Path = System.IO.Path;
using Property = Sledge.DataStructures.MapObjects.Property;
using Quaternion = Sledge.DataStructures.Geometric.Quaternion;

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
            Mediator.Subscribe(EditorMediator.DocumentTreeStructureChanged, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeObjectsChanged, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeFacesChanged, this);

            Mediator.Subscribe(EditorMediator.SettingsChanged, this);

            Mediator.Subscribe(HotkeysMediator.FileClose, this);
            Mediator.Subscribe(HotkeysMediator.FileSave, this);
            Mediator.Subscribe(HotkeysMediator.FileSaveAs, this);
            Mediator.Subscribe(HotkeysMediator.FileCompile, this);

            Mediator.Subscribe(HotkeysMediator.HistoryUndo, this);
            Mediator.Subscribe(HotkeysMediator.HistoryRedo, this);

            Mediator.Subscribe(HotkeysMediator.OperationsCopy, this);
            Mediator.Subscribe(HotkeysMediator.OperationsCut, this);
            Mediator.Subscribe(HotkeysMediator.OperationsPaste, this);
            Mediator.Subscribe(HotkeysMediator.OperationsPasteSpecial, this);
            Mediator.Subscribe(HotkeysMediator.OperationsDelete, this);
            Mediator.Subscribe(HotkeysMediator.SelectionClear, this);
            Mediator.Subscribe(HotkeysMediator.SelectAll, this);
            Mediator.Subscribe(HotkeysMediator.ObjectProperties, this);

            Mediator.Subscribe(HotkeysMediator.QuickHideSelected, this);
            Mediator.Subscribe(HotkeysMediator.QuickHideUnselected, this);
            Mediator.Subscribe(HotkeysMediator.QuickHideShowAll, this);

            Mediator.Subscribe(HotkeysMediator.SwitchTool, this);
            Mediator.Subscribe(HotkeysMediator.ApplyCurrentTextureToSelection, this);

            Mediator.Subscribe(HotkeysMediator.Carve, this);
            Mediator.Subscribe(HotkeysMediator.MakeHollow, this);
            Mediator.Subscribe(HotkeysMediator.GroupingGroup, this);
            Mediator.Subscribe(HotkeysMediator.GroupingUngroup, this);
            Mediator.Subscribe(HotkeysMediator.TieToEntity, this);
            Mediator.Subscribe(HotkeysMediator.TieToWorld, this);
            Mediator.Subscribe(HotkeysMediator.Transform, this);
            Mediator.Subscribe(HotkeysMediator.ReplaceTextures, this);
            Mediator.Subscribe(HotkeysMediator.SnapSelectionToGrid, this);
            Mediator.Subscribe(HotkeysMediator.SnapSelectionToGridIndividually, this);
            Mediator.Subscribe(HotkeysMediator.AlignXMax, this);
            Mediator.Subscribe(HotkeysMediator.AlignXMin, this);
            Mediator.Subscribe(HotkeysMediator.AlignYMax, this);
            Mediator.Subscribe(HotkeysMediator.AlignYMin, this);
            Mediator.Subscribe(HotkeysMediator.AlignZMax, this);
            Mediator.Subscribe(HotkeysMediator.AlignZMin, this);
            Mediator.Subscribe(HotkeysMediator.FlipX, this);
            Mediator.Subscribe(HotkeysMediator.FlipY, this);
            Mediator.Subscribe(HotkeysMediator.FlipZ, this);

            Mediator.Subscribe(HotkeysMediator.GridIncrease, this);
            Mediator.Subscribe(HotkeysMediator.GridDecrease, this);
            Mediator.Subscribe(HotkeysMediator.CenterAllViewsOnSelection, this);
            Mediator.Subscribe(HotkeysMediator.Center2DViewsOnSelection, this);
            Mediator.Subscribe(HotkeysMediator.Center3DViewsOnSelection, this);
            Mediator.Subscribe(HotkeysMediator.GoToBrushID, this);
            Mediator.Subscribe(HotkeysMediator.GoToCoordinates, this);

            Mediator.Subscribe(HotkeysMediator.QuickLoadPointfile, this);
            Mediator.Subscribe(HotkeysMediator.LoadPointfile, this);
            Mediator.Subscribe(HotkeysMediator.UnloadPointfile, this);

            Mediator.Subscribe(HotkeysMediator.ToggleSnapToGrid, this);
            Mediator.Subscribe(HotkeysMediator.ToggleShow2DGrid, this);
            Mediator.Subscribe(HotkeysMediator.ToggleShow3DGrid, this);
            Mediator.Subscribe(HotkeysMediator.ToggleIgnoreGrouping, this);
            Mediator.Subscribe(HotkeysMediator.ToggleTextureLock, this);
            Mediator.Subscribe(HotkeysMediator.ToggleTextureScalingLock, this);
            Mediator.Subscribe(HotkeysMediator.ToggleCordon, this);
            Mediator.Subscribe(HotkeysMediator.ToggleHideFaceMask, this);

            Mediator.Subscribe(HotkeysMediator.ShowSelectedBrushID, this);
            Mediator.Subscribe(HotkeysMediator.ShowMapInformation, this);
            Mediator.Subscribe(HotkeysMediator.ShowEntityReport, this);
            Mediator.Subscribe(HotkeysMediator.CheckForProblems, this);

            Mediator.Subscribe(EditorMediator.ViewportRightClick, this);

            Mediator.Subscribe(EditorMediator.WorldspawnProperties, this);

            Mediator.Subscribe(EditorMediator.VisgroupSelect, this);
            Mediator.Subscribe(EditorMediator.VisgroupShowAll, this);
            Mediator.Subscribe(EditorMediator.VisgroupShowEditor, this);
            Mediator.Subscribe(EditorMediator.VisgroupToggled, this);
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
                    ToolManager.Activate(typeof(SelectTool));
                }
            }
            if (!Mediator.ExecuteDefault(this, message, data))
            {
                throw new Exception("Invalid document message: " + message + ", with data: " + data);
            }
        }

        // ReSharper disable UnusedMember.Global
        // ReSharper disable MemberCanBePrivate.Global

        private void DocumentTreeStructureChanged()
        {
            _document.UpdateDisplayLists();
        }

        private void DocumentTreeObjectsChanged(IEnumerable<MapObject> objects)
        {
            _document.UpdateDisplayLists(objects);
        }

        private void DocumentTreeFacesChanged(IEnumerable<Face> faces)
        {
            _document.UpdateDisplayLists(faces);
        }

        public void SettingsChanged()
        {
            _document.HelperManager.UpdateCache();
            RebuildGrid();
        }

        public void HistoryUndo()
        {
            _document.History.Undo();
        }

        public void HistoryRedo()
        {
            _document.History.Redo();
        }

        public void FileClose()
        {
            if (_document.History.TotalActionsSinceLastSave > 0)
            {
                var result = MessageBox.Show("Would you like to save your changes to this map?", "Changes Detected", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) FileSave();
            }
            DocumentManager.SwitchTo(null);
            DocumentManager.Remove(_document);
            Mediator.Publish(EditorMediator.DocumentClosed);
        }

        public void FileSave()
        {
            var currentFile = _document.MapFile;
            if (currentFile == null)
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = @"VMF Files (*.vmf)|*.vmf|RMF Files (*.rmf)|*.rmf|MAP Files (*.map)|*.map";
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

            _document.History.TotalActionsSinceLastSave = 0;
        }

        public void FileSaveAs()
        {
            string currentFile = null;
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = @"VMF Files (*.vmf)|*.vmf|RMF Files (*.rmf)|*.rmf|MAP Files (*.map)|*.map";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    currentFile = sfd.FileName;
                }
            }
            if (currentFile == null) return;

            MapProvider.SaveMapToFile(currentFile, _document.Map);
            _document.MapFile = currentFile;
            Mediator.Publish(EditorMediator.FileOpened, _document.MapFile);

            _document.History.TotalActionsSinceLastSave = 0;
        }

        public void FileCompile()
        {
            FileSave();
            if (_document.MapFile == null) return;

            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            _document.Map.WorldSpawn.EntityData.Properties.Add(new Property
            {
                Key = "wad",
                Value = string.Join(";", _document.Game.Wads.Select(x => x.Path))
            });
            var map = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(_document.MapFile) + ".map");
            SaveWithCordon(map);

            var build = SettingsManager.Builds.FirstOrDefault(x => x.ID == _document.Game.BuildID);
            var batch = new Batch(_document.Game, build, map, _document.MapFile);
            BatchCompiler.Compile(batch);
        }

        private void SaveWithCordon(string file)
        {
            var map = _document.Map;
            if (_document.Map.Cordon)
            {
                map = new Map();
                map.WorldSpawn.EntityData = _document.Map.WorldSpawn.EntityData.Clone();
                var entities = _document.Map.WorldSpawn.GetAllNodesContainedWithin(_document.Map.CordonBounds);
                foreach (var mo in entities)
                {
                    var clone = mo.Clone();
                    clone.SetParent(map.WorldSpawn);
                }
                var outside = new Box(map.WorldSpawn.Children.Select(x => x.BoundingBox).Union(new[] {_document.Map.CordonBounds}));
                outside = new Box(outside.Start - Coordinate.One, outside.End + Coordinate.One);
                var inside = _document.Map.CordonBounds;

                var brush = new Brushes.BlockBrush();

                var cordon = (Solid) brush.Create(map.IDGenerator, outside, null).First();
                var carver = (Solid) brush.Create(map.IDGenerator, inside, null).First();
                cordon.Faces.ForEach(x => x.Texture.Name = "BLACK");

                // Do a carve (TODO: move carve into helper method?)
                foreach (var plane in carver.Faces.Select(x => x.Plane))
                {
                    Solid back, front;
                    if (!cordon.Split(plane, out back, out front, map.IDGenerator)) continue;
                    front.SetParent(map.WorldSpawn);
                    cordon = back;
                }

            }
            MapProvider.SaveMapToFile(file, map);
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

            list.ForEach(x => x.IsSelected = true);
            _document.Selection.SwitchToObjectSelection();

            var name = "Pasted " + list.Count + " item" + (list.Count == 1 ? "" : "s");
            var selected = _document.Selection.GetSelectedObjects().ToList();
            _document.PerformAction(name, new ActionCollection(
                                              new Deselect(selected), // Deselect the current objects
                                              new Create(list))); // Add and select the new objects
        }

        public void OperationsPasteSpecial()
        {
            if (!ClipboardManager.CanPaste()) return;

            var content = ClipboardManager.GetPastedContent(_document);
            if (content == null) return;

            var list = content.ToList();
            if (!list.Any()) return;

            var box = new Box(list.Select(x => x.BoundingBox));

            using (var psd = new PasteSpecialDialog(box))
            {
                if (psd.ShowDialog() == DialogResult.OK)
                {
                    var name = "Paste special (" + psd.NumberOfCopies + (psd.NumberOfCopies == 1 ? " copy)" : " copies)");
                    var action = new PasteSpecial(list, psd.NumberOfCopies, psd.StartPoint, psd.Grouping,
                                                  psd.AccumulativeOffset, psd.AccumulativeRotation,
                                                  psd.MakeEntitiesUnique, psd.PrefixEntityNames, psd.EntityNamePrefix);
                    _document.PerformAction(name, action);
                }
            }
        }

        public void OperationsDelete()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                var sel = _document.Selection.GetSelectedParents().Select(x => x.ID).ToList();
                var name = "Removed " + sel.Count + " item" + (sel.Count == 1 ? "" : "s");
                _document.PerformAction(name, new Delete(sel));
            }
        }

        public void SelectionClear()
        {
            var selected = _document.Selection.GetSelectedObjects().ToList();
            _document.PerformAction("Clear selection", new Deselect(selected));
        }

        public void SelectAll()
        {
            var all = _document.Map.WorldSpawn.Find(x => !(x is World));
            _document.PerformAction("Select all", new Actions.MapObjects.Selection.Select(all));
        }

        public void ObjectProperties()
        {
            var pd = new EntityEditor(_document);
            pd.Show(Editor.Instance);
        }

        public void SwitchTool(HotkeyTool tool)
        {
            ToolManager.Activate(tool);
        }

        public void ApplyCurrentTextureToSelection()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection || Editor.Instance == null) return;
            var texture = Editor.Instance.GetSelectedTexture();
            if (texture == null) return;
            var ti = texture.GetTexture();
            if (ti == null) return;
            Action<Document, Face> action = (document, face) =>
            {
                face.Texture.Name = texture.Name;
                face.Texture.Texture = ti;
                face.CalculateTextureCoordinates();
            };
            var faces = _document.Selection.GetSelectedObjects().OfType<Solid>().SelectMany(x => x.Faces);
            _document.PerformAction("Apply current texture", new EditFace(faces, action, true));
        }

        public void QuickHideSelected()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var autohide = _document.Map.GetAllVisgroups().FirstOrDefault(x => x.Name == "Autohide");
            if (autohide == null) return;

            var objects = _document.Selection.GetSelectedObjects();
            _document.PerformAction("Hide objects", new QuickHideObjects(objects));
        }

        public void QuickHideUnselected()
        {
            if (_document.Selection.InFaceSelection) return;

            var autohide = _document.Map.GetAllVisgroups().FirstOrDefault(x => x.Name == "Autohide");
            if (autohide == null) return;

            var objects = _document.Map.WorldSpawn.Find(x => !x.IsSelected);
            _document.PerformAction("Hide objects", new QuickHideObjects(objects));
        }

        public void QuickHideShowAll()
        {
            var autohide = _document.Map.GetAllVisgroups().FirstOrDefault(x => x.Name == "Autohide");
            if (autohide == null) return;

            var objects = _document.Map.WorldSpawn.Find(x => x.IsInVisgroup(autohide.ID));
            _document.PerformAction("Show hidden objects", new QuickShowObjects(objects));
        }

        public void WorldspawnProperties()
        {
            var pd = new EntityEditor(_document) { FollowSelection = false, AllowClassChange = false };
            pd.SetObjects(new[] { _document.Map.WorldSpawn });
            pd.Show(Editor.Instance);
        }

        public void Carve()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var carver = _document.Selection.GetSelectedObjects().OfType<Solid>().FirstOrDefault();
            if (carver == null) return;

            var carvees = _document.Map.WorldSpawn.Find(x => x is Solid && x.BoundingBox.IntersectsWith(carver.BoundingBox)).OfType<Solid>();

            _document.PerformAction("Carve objects", new Carve(carvees, carver));
        }

        public void MakeHollow()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var solids = _document.Selection.GetSelectedObjects().OfType<Solid>().ToList();
            if (!solids.Any()) return;

            if (solids.Count > 1)
            {
                if (MessageBox.Show("This will hollow out every selected solid, are you sure?", "Multiple solids selected", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
            }

            var qf = new QuickForm("Select wall width").NumericUpDown("Wall width (negative to hollow outwards)", -1024, 1024, 0, 32).OkCancel();

            decimal width;
            do
            {
                if (qf.ShowDialog() == DialogResult.Cancel) return;
                width = qf.Decimal("Wall width (negative to hollow outwards)");
                if (width == 0) MessageBox.Show("Please select a non-zero value.");
            } while (width == 0);

            _document.PerformAction("Make objects hollow", new MakeHollow(solids, width));
        }

        public void GroupingGroup()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                _document.PerformAction("Grouped objects", new GroupAction(_document.Selection.GetSelectedObjects()));
            }
        }

        public void GroupingUngroup()
        {
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection)
            {
                _document.PerformAction("Ungrouped objects", new UngroupAction(_document.Selection.GetSelectedObjects()));
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
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var entities = _document.Selection.GetSelectedObjects().OfType<Entity>().ToList();

            Entity existing = null;
                
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

            var ac = new ActionCollection();

            if (existing == null)
            {
                var def = _document.Game.DefaultBrushEntity;
                var entity = _document.GameData.Classes.FirstOrDefault(x => x.Name.ToLower() == def.ToLower())
                             ?? _document.GameData.Classes.Where(x => x.ClassType == ClassType.Solid).OrderBy(x => x.Name.StartsWith("trigger_once") ? 0 : 1).FirstOrDefault();
                if (entity == null)
                {
                    MessageBox.Show("No solid entities found. Please make sure your FGDs are configured correctly.", "No entities found!");
                    return;
                }
                existing = new Entity(_document.Map.IDGenerator.GetNextObjectID())
                               {
                                   EntityData = new EntityData(entity),
                                   ClassName = entity.Name,
                                   Colour = Colour.GetDefaultEntityColour()
                               };
                ac.Add(new Create(existing));
            }
                
            var reparent = _document.Selection.GetSelectedObjects().Where(x => x != existing).ToList();
            ac.Add(new Reparent(existing.ID, reparent));
            ac.Add(new Actions.MapObjects.Selection.Select(existing));

            _document.PerformAction("Tie to Entity", ac);

            Mediator.Publish(HotkeysMediator.ObjectProperties);
        }

        public void TieToWorld()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var entities = _document.Selection.GetSelectedObjects().OfType<Entity>().ToList();
            var children = entities.SelectMany(x => x.Children).ToList();

            var ac = new ActionCollection();
            ac.Add(new Reparent(_document.Map.WorldSpawn.ID, children));
            ac.Add(new Delete(entities.Select(x => x.ID)));

            _document.PerformAction("Tie to World", ac);
        }

        private IUnitTransformation GetSnapTransform(Box box)
        {
            var offset = box.Start.Snap(_document.Map.GridSpacing) - box.Start;
            return new UnitTranslate(offset);
        }

        public void Transform()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;
            var box = _document.Selection.GetSelectionBoundingBox();
            using (var td = new TransformDialog(box))
            {
                if (td.ShowDialog() != DialogResult.OK) return;

                var value = td.TransformValue;
                IUnitTransformation transform = null;
                switch (td.TransformType)
                {
                    case TransformType.Rotate:
                        var mov = Matrix.Translation(-box.Center); // Move to zero
                        var rot = Matrix.Rotation(Quaternion.EulerAngles(value * DMath.PI / 180)); // Do rotation
                        var fin = Matrix.Translation(box.Center); // Move to final origin
                        transform = new UnitMatrixMult(fin * rot * mov);
                        break;
                    case TransformType.Translate:
                        transform = new UnitTranslate(value);
                        break;
                    case TransformType.Scale:
                        transform = new UnitScale(value, box.Center);
                        break;
                }

                if (transform == null) return;

                var selected = _document.Selection.GetSelectedParents();
                _document.PerformAction("Transform selection", new Edit(selected, (d, x) => x.Transform(transform, d.Map.GetTransformFlags())));
            }
        }

        public void ReplaceTextures()
        {
            using (var trd = new TextureReplaceDialog(_document))
            {
                if (trd.ShowDialog() == DialogResult.OK)
                {
                    var action = trd.GetAction();
                    _document.PerformAction("Replace textures", action);
                }
            }
        }

        public void SnapSelectionToGrid()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selected = _document.Selection.GetSelectedParents();

            var box = _document.Selection.GetSelectionBoundingBox();
            var transform = GetSnapTransform(box);

            _document.PerformAction("Snap to grid", new Edit(selected, (d, x) => x.Transform(transform, d.Map.GetTransformFlags())));
        }

        public void SnapSelectionToGridIndividually()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selected = _document.Selection.GetSelectedParents();

            _document.PerformAction("Snap to grid individually", new Edit(selected, (d, x) => x.Transform(GetSnapTransform(x.BoundingBox), d.Map.GetTransformFlags())));
        }

        private IUnitTransformation GetAlignTransform(Box selection, Box item, Func<Box, decimal> extractor, Func<decimal, Coordinate> creator)
        {
            var current = extractor(item);
            var target = extractor(selection);
            var value = target - current;
            var translate = creator(value);
            return new UnitTranslate(translate);
        }

        private void AlignObjects(Func<Box, decimal> extractor, Func<decimal, Coordinate> creator)
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selected = _document.Selection.GetSelectedParents();
            var box = _document.Selection.GetSelectionBoundingBox();

            _document.PerformAction("Align Objects", new Edit(selected, (d, x) => x.Transform(GetAlignTransform(box, x.BoundingBox, extractor, creator), d.Map.GetTransformFlags())));
        }

        public void AlignXMax()
        {
            AlignObjects(x => x.End.X, x => new Coordinate(x, 0, 0));
        }

        public void AlignXMin()
        {
            AlignObjects(x => x.Start.X, x => new Coordinate(x, 0, 0));
        }

        public void AlignYMax()
        {
            AlignObjects(y => y.End.Y, y => new Coordinate(0, y, 0));
        }

        public void AlignYMin()
        {
            AlignObjects(y => y.Start.Y, y => new Coordinate(0, y, 0));
        }

        public void AlignZMax()
        {
            AlignObjects(z => z.End.Z, z => new Coordinate(0, 0, z));
        }

        public void AlignZMin()
        {
            AlignObjects(z => z.Start.Z, z => new Coordinate(0, 0, z));
        }

        private void FlipObjects(Coordinate scale)
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selected = _document.Selection.GetSelectedParents();
            var box = _document.Selection.GetSelectionBoundingBox();

            _document.PerformAction("Flip Objects", new Edit(selected, (d, x) => x.Transform(new UnitScale(scale, box.Center), d.Map.GetTransformFlags())));
        }

        public void FlipX()
        {
            FlipObjects(new Coordinate(-1, 1, 1));
        }

        public void FlipY()
        {
            FlipObjects(new Coordinate(1, -1, 1));
        }

        public void FlipZ()
        {
            FlipObjects(new Coordinate(1, 1, -1));
        }

        public void GridIncrease()
        {
            var curr = _document.Map.GridSpacing;
            if (curr >= 1024) return;
            _document.Map.GridSpacing *= 2;
            RebuildGrid();
        }

        public void GridDecrease()
        {
            var curr = _document.Map.GridSpacing;
            if (curr <= 1) return;
            _document.Map.GridSpacing /= 2;
            RebuildGrid();
        }

        public void RebuildGrid()
        {
            _document.Renderer.Bind();
            _document.Renderer.GridSpacing = (float)_document.Map.GridSpacing;
            _document.Renderer.Unbind();
            foreach (var kv in _document.Renderer.GridRenderables)
            {
                kv.Value.RebuildGrid(((Viewport2D)kv.Key).Zoom, true);
            }
            Mediator.Publish(EditorMediator.DocumentGridSpacingChanged, _document.Map.GridSpacing);
        }

        public void CenterAllViewsOnSelection()
        {
            var box = _document.Selection.GetSelectionBoundingBox();
            if (box == null) return;
            foreach (var vp in ViewportManager.Viewports)
            {
                vp.FocusOn(box);
            }
        }

        public void Center2DViewsOnSelection()
        {
            var box = _document.Selection.GetSelectionBoundingBox();
            if (box == null) return;
            foreach (var vp in ViewportManager.Viewports.OfType<Viewport2D>())
            {
                vp.FocusOn(box);
            }
        }

        public void Center3DViewsOnSelection()
        {
            var box = _document.Selection.GetSelectionBoundingBox();
            if (box == null) return;
            foreach (var vp in ViewportManager.Viewports.OfType<Viewport3D>())
            {
                vp.FocusOn(box);
            }
        }

        public void GoToCoordinates()
        {
            using (var qf = new QuickForm("Enter Coordinates") { LabelWidth = 50, UseShortcutKeys = true }
                .TextBox("X", "0")
                .TextBox("Y", "0")
                .TextBox("Z", "0")
                .OkCancel())
            {
                qf.ClientSize = new Size(180, qf.ClientSize.Height);
                if (qf.ShowDialog() != DialogResult.OK) return;

                decimal x, y, z;
                if (!Decimal.TryParse(qf.String("X"), out x)) return;
                if (!Decimal.TryParse(qf.String("Y"), out y)) return;
                if (!Decimal.TryParse(qf.String("Z"), out z)) return;

                var coordinate = new Coordinate(x, y, z);

                ViewportManager.Viewports.ForEach(vp => vp.FocusOn(coordinate));
            }
        }

        public void GoToBrushID()
        {
            using (var qf = new QuickForm("Enter Brush ID") { LabelWidth = 100, UseShortcutKeys = true }
                .TextBox("Brush ID")
                .OkCancel())
            {
                qf.ClientSize = new Size(230, qf.ClientSize.Height);

                if (qf.ShowDialog() != DialogResult.OK) return;

                long id;
                if (!long.TryParse(qf.String("Brush ID"), out id)) return;

                var obj = _document.Map.WorldSpawn.FindByID(id);
                if (obj == null) return;

                // Select and go to the brush
                _document.PerformAction("Select brush ID " + id, new ChangeSelection(new[] { obj }, _document.Selection.GetSelectedObjects()));
                ViewportManager.Viewports.ForEach(x => x.FocusOn(obj.BoundingBox));
            }
        }

        private void OpenPointfile(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show("The pointfile was not found.");
                return;
            }

            var text = File.ReadAllLines(file);
            try
            {
                _document.Pointfile = Pointfile.Parse(text);

                var end = _document.Pointfile.Lines.LastOrDefault();
                if (end == null) return;

                var vp = ViewportManager.Viewports.OfType<Viewport3D>().FirstOrDefault();
                if (vp == null) return;

                vp.Camera.Location = new Vector3((float)end.End.DX, (float)end.End.DY, (float)end.End.DZ);
                vp.Camera.LookAt = new Vector3((float)end.Start.DX, (float)end.Start.DY, (float)end.Start.DZ);
            }
            catch
            {
                MessageBox.Show(Path.GetFileName(file) + " is not a valid pointfile!");
            }
        }

        public void QuickLoadPointfile()
        {
            var dir = Path.GetDirectoryName(_document.MapFile);
            var file = Path.GetFileNameWithoutExtension(_document.MapFile);
            if (dir != null && file != null)
            {
                var lin = Path.Combine(dir, file + ".lin");
                if (File.Exists(lin))
                {
                    OpenPointfile(lin);
                    return;
                }
                var pts = Path.Combine(dir, file + ".lin");
                if (File.Exists(pts))
                {
                    OpenPointfile(pts);
                    return;
                }
            }
            if (MessageBox.Show("No pointfile found. Would you like to browse for one?", "Pointfile not found", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                LoadPointfile();
            }
        }

        public void LoadPointfile()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Pointfiles (*.lin, *.pts)|*.lin;*.pts";
                ofd.InitialDirectory = Path.GetDirectoryName(_document.MapFile);
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    OpenPointfile(ofd.FileName);
                }
            }
        }

        public void UnloadPointfile()
        {
            _document.Pointfile = null;
        }

        public void ToggleSnapToGrid()
        {
            _document.Map.SnapToGrid = !_document.Map.SnapToGrid;
            Mediator.Publish(EditorMediator.UpdateToolstrip);
        }

        public void ToggleShow2DGrid()
        {
            _document.Map.Show2DGrid = !_document.Map.Show2DGrid;
            RebuildGrid();
            Mediator.Publish(EditorMediator.UpdateToolstrip);
        }

        public void ToggleShow3DGrid()
        {
            _document.Map.Show3DGrid = !_document.Map.Show3DGrid;
            _document.Renderer.Bind();
            _document.Renderer.Show3DGrid = _document.Map.Show3DGrid;
            _document.Renderer.GridSpacing = (float)_document.Map.GridSpacing;
            _document.Renderer.Unbind();
            Mediator.Publish(EditorMediator.UpdateToolstrip);
        }

        public void ToggleIgnoreGrouping()
        {
            _document.Map.IgnoreGrouping = !_document.Map.IgnoreGrouping;
            Mediator.Publish(EditorMediator.UpdateToolstrip);
        }

        public void ToggleTextureLock()
        {
            _document.Map.TextureLock = !_document.Map.TextureLock;
            Mediator.Publish(EditorMediator.UpdateToolstrip);
        }

        public void ToggleTextureScalingLock()
        {
            _document.Map.TextureScalingLock = !_document.Map.TextureScalingLock;
            Mediator.Publish(EditorMediator.UpdateToolstrip);
        }

        public void ToggleCordon()
        {
            _document.Map.Cordon = !_document.Map.Cordon;
            Mediator.Publish(EditorMediator.UpdateToolstrip);
        }

        public void ToggleHideFaceMask()
        {
            _document.Map.HideFaceMask = !_document.Map.HideFaceMask;
        }

        public void ShowSelectedBrushID()
        {
            if (_document.Selection.IsEmpty() || _document.Selection.InFaceSelection) return;

            var selectedIds = _document.Selection.GetSelectedObjects().Select(x => x.ID);
            var idString = String.Join(", ", selectedIds);

            MessageBox.Show("Selected Object IDs: " + idString);
        }

        public void ShowMapInformation()
        {
            using (var mid = new MapInformationDialog(_document))
            {
                mid.ShowDialog();
            }
        }

        public void ShowEntityReport()
        {
            var erd = new EntityReportDialog();
            erd.Show(Editor.Instance);
        }

        public void CheckForProblems()
        {
            using (var cfpd = new CheckForProblemsDialog(_document))
            {
                cfpd.ShowDialog(Editor.Instance);
            }
        }

        public void ViewportRightClick(Viewport2D vp, ViewportEvent e)
        {
            ViewportContextMenu.Instance.AddNonSelectionItems(_document, vp);
            if (!_document.Selection.IsEmpty() && !_document.Selection.InFaceSelection && ToolManager.ActiveTool is SelectTool)
            {
                var selectionBoundingBox = _document.Selection.GetSelectionBoundingBox();
                var point = vp.ScreenToWorld(e.X, vp.Height - e.Y);
                var start = vp.Flatten(selectionBoundingBox.Start);
                var end = vp.Flatten(selectionBoundingBox.End);
                if (point.X >= start.X && point.X <= end.X && point.Y >= start.Y && point.Y <= end.Y)
                {
                    // Clicked inside the selection bounds
                    ViewportContextMenu.Instance.AddSelectionItems(_document, vp);
                }
            }
            ViewportContextMenu.Instance.Show(vp, e.X, e.Y);
        }

        public void VisgroupSelect(int visgroupId)
        {
            if (_document.Selection.InFaceSelection) return;
            var objects = _document.Map.WorldSpawn.Find(x => x.IsInVisgroup(visgroupId), true).Where(x => !x.IsVisgroupHidden);
            _document.PerformAction("Select visgroup", new ChangeSelection(objects, _document.Selection.GetSelectedObjects()));
        }

        public void VisgroupShowEditor()
        {
            using (var vef = new VisgroupEditForm(_document))
            {
                if (vef.ShowDialog() == DialogResult.OK)
                {
                    var nv = new List<Visgroup>();
                    var cv = new List<Visgroup>();
                    var dv = new List<Visgroup>();
                    vef.PopulateChangeLists(_document, nv, cv, dv);
                    if (nv.Any() || cv.Any() || dv.Any())
                    {
                        _document.PerformAction("Edit visgroups", new CreateEditDeleteVisgroups(nv, cv, dv));
                    }
                }
            }
        }

        public void VisgroupShowAll()
        {
            _document.PerformAction("Show all visgroups", new ShowAllVisgroups());
        }

        public void VisgroupToggled(int visgroupId, CheckState state)
        {
            if (state == CheckState.Indeterminate) return;
            var visible = state == CheckState.Checked;
            _document.PerformAction((visible ? "Show" : "Hide") + " visgroup", new ToggleVisgroup(visgroupId, visible));
        }
    }
}