using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Shell;
using Sledge.Shell.Forms;

namespace Sledge.BspEditor.Tools.Texture
{
    /// <summary>
    /// This dialog is linked directly to the texture tool's context and provides useful operations for editing textures.
    /// </summary>
    [Export(typeof(IDialog))]
    [AutoTranslate]
    public partial class TextureApplicationForm : BaseForm, IDialog, IManualTranslate
    {
        // We want to use the shell for the parent
        [Import("Shell", typeof(Form))] private Form _shell;

        private readonly List<string> _recentTextures = new List<string>();

        // Event stopper to make sure we don't fire change events recursively 
        private bool _freeze;

        // The current values of the texture property controls
        private readonly CurrentTextureProperties _currentTextureProperties;

        private WeakReference<MapDocument> _document = new WeakReference<MapDocument>(null);

        private event EventHandler DebouncedPropertiesChanged;
        private IDisposable _saveChanges;

        private TextureListPanel SelectedTexturesList;
        private TextureListPanel RecentTexturesList;

        public MapDocument Document
        {
            get
            {
                MapDocument d;
                return _document.TryGetTarget(out d) ? d : null;
            }
            set
            {
                _document = new WeakReference<MapDocument>(value);
                var precision = 4; // todo post-beta: environment-specific texture values precision
                                   // _document != null && _document.Game != null && _document.Game.Engine == Engine.Goldsource ? 2 : 4;
                ScaleXValue.DecimalPlaces = ScaleYValue.DecimalPlaces = precision;
            }
        }

        public TextureApplicationForm()
        {
            _freeze = true;

            InitializeComponent();
            InitialiseTextureLists();

            SelectedTexturesList.HighlightedTexturesChanged += TextureListHighlightedTexturesChanged;
            RecentTexturesList.HighlightedTexturesChanged += TextureListHighlightedTexturesChanged;

            _freeze = false;

            _currentTextureProperties = new CurrentTextureProperties();

            Oy.Subscribe<IDocument>("Document:Activated", SetDocument);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
            Oy.Subscribe<object>("TextureTool:SelectionChanged", async x => await FaceSelectionChanged());
            
            // Throttle property changes so they only apply after 500 ms, this should keep the undo stack clear
            _saveChanges = Observable.FromEventPattern(x => DebouncedPropertiesChanged += x, x => DebouncedPropertiesChanged -= x)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(x =>
                {
                    ApplyPropertyChanges(false);
                });
        }

        public string LeftClick { get; set; }
        public string RightClick { get; set; }

        public string ActionLift { get; set; }
        public string ActionSelect { get; set; }

        public string ActionApply { get; set; }
        public string ActionValues { get; set; }
        public string ActionAxis { get; set; }
        public string ActionAlignToView { get; set; }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");

                ScaleLabel.Text = strings.GetString(prefix, "Scale");
                ShiftLabel.Text = strings.GetString(prefix, "Shift");

                BrowseButton.Text = strings.GetString(prefix, "Browse");
                ReplaceButton.Text = strings.GetString(prefix, "Replace");
                ApplyButton.Text = strings.GetString(prefix, "Apply");
                
                RotationLabel.Text = strings.GetString(prefix, "Rotation");
                LightmapLabel.Text = strings.GetString(prefix, "Lightmap");
                SmoothingGroupsButton.Text = strings.GetString(prefix, "SmoothingGroups");

                AlignGroup.Text = strings.GetString(prefix, "Align");
                AlignToWorldCheckbox.Text = strings.GetString(prefix, "World");
                AlignToFaceCheckbox.Text = strings.GetString(prefix, "Face");

                JustifyGroup.Text = strings.GetString(prefix, "Justify");
                JustifyFitButton.Text = strings.GetString(prefix, "Fit");
                TreatAsOneCheckbox.Text = strings.GetString(prefix, "TreatAsOne");

                HideMaskCheckbox.Text = strings.GetString(prefix, "HideMask");
                FilterRecentLabel.Text = strings.GetString(prefix, "FilterRecent");

                LeftClick = strings.GetString(prefix, "LeftClick");
                RightClick = strings.GetString(prefix, "RightClick");
                ActionLift = strings.GetString(prefix, "ActionLift");
                ActionSelect = strings.GetString(prefix, "ActionSelect");
                ActionApply = strings.GetString(prefix, "ActionApply");
                ActionValues = strings.GetString(prefix, "ActionValues");
                ActionAxis = strings.GetString(prefix, "ActionAxis");
                ActionAlignToView = strings.GetString(prefix, "ActionAlignToView");

                LeftClickActionButton.Text = $@"{LeftClick}: {ActionLift}+{ActionSelect}";
                LeftClickActionMenu.Items.Clear();
                LeftClickActionMenu.Items.Add(new ToolStripMenuItem($"{ActionLift}+{ActionSelect}") { Tag = ClickAction.Lift | ClickAction.Select });
                LeftClickActionMenu.Items.Add(new ToolStripMenuItem(ActionLift) { Tag = ClickAction.Lift });
                LeftClickActionMenu.Items.Add(new ToolStripMenuItem(ActionSelect) { Tag = ClickAction.Select });

                RightClickActionButton.Text = $@"{RightClick}: {ActionApply}+{ActionValues}";
                RightClickActionMenu.Items.Clear();
                RightClickActionMenu.Items.Add(new ToolStripMenuItem(ActionApply) { Tag = ClickAction.Apply });
                RightClickActionMenu.Items.Add(new ToolStripMenuItem($"{ActionApply}+{ActionValues}") { Tag = ClickAction.Apply | ClickAction.Values });
                RightClickActionMenu.Items.Add(new ToolStripMenuItem($"{ActionApply}+{ActionValues}+{ActionAxis}") { Tag = ClickAction.Apply | ClickAction.AlignToSample });
                RightClickActionMenu.Items.Add(new ToolStripMenuItem(ActionAlignToView) { Tag = ClickAction.AlignToView });
            });
        }

        private void SetLeftClickAction(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!(e.ClickedItem.Tag is ClickAction)) return;

            var action = (ClickAction) e.ClickedItem.Tag;
            LeftClickActionButton.Text = $@"{LeftClick}: {e.ClickedItem.Text}";
            Oy.Publish("BspEditor:TextureTool:SetLeftClickAction", action);
        }

        private void SetRightClickAction(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!(e.ClickedItem.Tag is ClickAction)) return;

            var action = (ClickAction) e.ClickedItem.Tag;
            RightClickActionButton.Text = $@"{RightClick}: {e.ClickedItem.Text}";
            Oy.Publish("BspEditor:TextureTool:SetRightClickAction", action);
        }

        private void InitialiseTextureLists()
        {
            RecentTexturesList = new TextureListPanel
            {
                AllowMultipleHighlighting = false,
                AllowHighlighting = true,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Black,
                EnableDrag = false,
                ImageSize = 64
            };

            SelectedTexturesList = new TextureListPanel
            {
                AllowMultipleHighlighting = false,
                AllowHighlighting = true,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Black,
                EnableDrag = false,
                ImageSize = 64
            };

            RecentTexturesList.TextureSelected += TexturesListTextureSelected;
            SelectedTexturesList.TextureSelected += TexturesListTextureSelected;

            RecentTextureListPanel.Controls.Add(RecentTexturesList);
            SelectedTextureListPanel.Controls.Add(SelectedTexturesList);
        }

        private async Task SetDocument(IDocument doc)
        {
            var md = doc as MapDocument;
            Document = md;
            if (md != null)
            {
                var tc = await md.Environment.GetTextureCollection();
                SelectedTexturesList.Collection = tc;
                RecentTexturesList.Collection = tc;
            }
            else
            {
                SelectedTexturesList.Collection = null;
                RecentTexturesList.Collection = null;
            }
        }

        private async Task DocumentChanged(Change change)
        {
            if (_document.TryGetTarget(out MapDocument t) && change.Document == t)
            {
                if (change.HasDataChanges && change.AffectedData.Any(x => x is ActiveTexture))
                {
                    var at = t.Map.Data.GetOne<ActiveTexture>()?.Name;
                    ActiveTextureChanged(at);
                }
                else if (change.HasObjectChanges && change.Updated.Intersect(GetFaceSelection().GetSelectedParents()).Any())
                {
                    await FaceSelectionChanged();
                }
            }
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveTool", out TextureTool _);
        }

        public void SetVisible(IContext context, bool visible)
        {
            if (Visible != visible)
            {
                if (visible)
                {
                    Show(_shell);
                    _shell?.Focus();
                }
                else
                {
                    Hide();
                }
            }
        }

        private string GetFirstSelectedTexture()
        {
            return RecentTexturesList
                .GetHighlightedTextures()
                .Union(SelectedTexturesList.GetHighlightedTextures())
                .FirstOrDefault();
        }

        private IEnumerable<string> GetSelectedTextures()
        {
            return RecentTexturesList
                .GetHighlightedTextures()
                .Union(SelectedTexturesList.GetHighlightedTextures());
        }

        public FaceSelection GetFaceSelection()
        {
            var fs = Document.Map.Data.GetOne<FaceSelection>();
            if (fs == null)
            {
                fs = new FaceSelection();
                Document.Map.Data.Add(fs);
            }
            return fs;
        }

        private async void TextureListHighlightedTexturesChanged(object sender, IEnumerable<string> sel)
        {
            if (_freeze) return;

            _freeze = true;

            var selection = sel.ToList();
            var item = selection.FirstOrDefault();

            if (selection.Any())
            {
                if (sender == SelectedTexturesList) RecentTexturesList.SetHighlightedTextures(new string[0]);
                if (sender == RecentTexturesList) SelectedTexturesList.SetHighlightedTextures(new string[0]);
            }
            else
            {
                item = RecentTexturesList
                    .GetHighlightedTextures()
                    .Union(SelectedTexturesList.GetHighlightedTextures())
                    .FirstOrDefault();
            }

            var label = "";
            var d = Document;
            if (item != null && d != null)
            {
                var tex = await d.Environment.GetTextureCollection();
                var ti = await tex.GetTextureItem(item);
                if (ti != null)
                {
                    label = $"{ti.Name} ({ti.Width} x {ti.Height})";
                }

                var at = new ActiveTexture {Name = item};
                await MapDocumentOperation.Perform(Document, new TrivialOperation(x => x.Map.Data.Replace(at), x => x.Update(at)));
            }

            TextureDetailsLabel.InvokeLater(() =>
            {
                TextureDetailsLabel.Text = label;
            });

            _freeze = false;
        }

        private void UpdateRecentTextureList()
        {
            RecentTexturesList.SetTextureList(_recentTextures.Where(x => x.ToLower().Contains(RecentFilterTextbox.Text.ToLower())));
        }

        private void ActiveTextureChanged(string item)
        {
            if (_freeze) return;

            if (item == null)
            {
                SelectedTexturesList.SetHighlightedTextures(new string[0]);
                return;
            }

            _recentTextures.Remove(item);
            _recentTextures.Insert(0, item);
            if (_recentTextures.Count > 10) _recentTextures.RemoveRange(10, _recentTextures.Count - 10);
            UpdateRecentTextureList();

            // If the texture is in the list of selected faces, select the texture in that list
            var sl = SelectedTexturesList.GetTextureList();
            if (sl.Any(x => String.Equals(x, item, StringComparison.InvariantCultureIgnoreCase)))
            {
                SelectedTexturesList.SetHighlightedTextures(new[] { item });
                SelectedTexturesList.ScrollToTexture(item);
            }
            else if (RecentTexturesList.GetTextureList().Contains(item))
            {
                // Otherwise, select the texture in the recent list
                RecentTexturesList.SetHighlightedTextures(new[] {item});
                RecentTexturesList.ScrollToTexture(item);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        private async Task FaceSelectionChanged()
        {
            // The list of selected faces has changed - update the texture properties to match the selection
            _freeze = true;

            var faces = GetFaceSelection();
            _currentTextureProperties.Reset(faces);

            var textures = new List<string>();

            foreach (var face in faces)
            {
                var tex = face.Texture;

                var name = tex.Name;
                if (textures.Any(x => String.Equals(x, name, StringComparison.InvariantCultureIgnoreCase))) continue;
                
                textures.Add(name);
            }

            var labelText = "";
            var d = Document;
            if (textures.Any() && d != null)
            {
                var t = textures[0];
                var tc = await d.Environment.GetTextureCollection();
                var ti = await tc.GetTextureItem(t);
                labelText = ti == null ? $"{t}" : $"{ti.Name} ({ti.Width} x {ti.Height})";
            }
            
            this.InvokeLater(() => {

                ScaleXValue.Value = (decimal) _currentTextureProperties.XScale;
                ScaleYValue.Value = (decimal) _currentTextureProperties.YScale;
                ShiftXValue.Value = (decimal) _currentTextureProperties.XShift;
                ShiftYValue.Value = (decimal) _currentTextureProperties.YShift;
                RotationValue.Value = (decimal) _currentTextureProperties.Rotation;

                if (_currentTextureProperties.DifferentXScaleValues) ScaleXValue.Text = "";
                if (_currentTextureProperties.DifferentYScaleValues) ScaleYValue.Text = "";
                if (_currentTextureProperties.DifferentXShiftValues) ShiftXValue.Text = "";
                if (_currentTextureProperties.DifferentYShiftValues) ShiftYValue.Text = "";
                if (_currentTextureProperties.DifferentRotationValues) RotationValue.Text = "";

                if (_currentTextureProperties.AllAlignedToFace) AlignToFaceCheckbox.CheckState = CheckState.Checked;
                else if (_currentTextureProperties.NoneAlignedToFace) AlignToFaceCheckbox.CheckState = CheckState.Unchecked;
                else AlignToFaceCheckbox.CheckState = CheckState.Indeterminate;

                if (_currentTextureProperties.AllAlignedToWorld) AlignToWorldCheckbox.CheckState = CheckState.Checked;
                else if (_currentTextureProperties.NoneAlignedToWorld) AlignToWorldCheckbox.CheckState = CheckState.Unchecked;
                else AlignToWorldCheckbox.CheckState = CheckState.Indeterminate;

                TextureDetailsLabel.Text = labelText;
                SelectedTexturesList.SetTextureList(textures);
                SelectedTexturesList.SetHighlightedTextures(textures);
                RecentTexturesList.SetHighlightedTextures(new string[0]);
                HideMaskCheckbox.Checked = Document.Map.Data.GetOne<HideFaceMask>()?.Hidden == true;

                _freeze = false;
            });

        }

        private void PropertiesChanged()
        {
            if (_freeze) return;

            if (!_currentTextureProperties.DifferentXScaleValues) _currentTextureProperties.XScale = (float) ScaleXValue.Value;
            if (!_currentTextureProperties.DifferentYScaleValues) _currentTextureProperties.YScale = (float) ScaleYValue.Value;
            if (!_currentTextureProperties.DifferentXShiftValues) _currentTextureProperties.XShift = (float) ShiftXValue.Value;
            if (!_currentTextureProperties.DifferentYShiftValues) _currentTextureProperties.YShift = (float) ShiftYValue.Value;
            if (!_currentTextureProperties.DifferentRotationValues) _currentTextureProperties.Rotation = (float) RotationValue.Value;

            ApplyPropertyChanges(true);
            DebouncedPropertiesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyFaceValues(Face target)
        {
            // apply values
            if (!_currentTextureProperties.DifferentXScaleValues) target.Texture.XScale = _currentTextureProperties.XScale;
            if (!_currentTextureProperties.DifferentXShiftValues) target.Texture.XShift = _currentTextureProperties.XShift;
            if (!_currentTextureProperties.DifferentYScaleValues) target.Texture.YScale = _currentTextureProperties.YScale;
            if (!_currentTextureProperties.DifferentYShiftValues) target.Texture.YShift = _currentTextureProperties.YShift;
            if (!_currentTextureProperties.DifferentRotationValues) target.Texture.SetRotation(_currentTextureProperties.Rotation);
        }

        private Dictionary<Face, Primitives.Texture> _memoTextures;

        private void ApplyPropertyChanges(bool trivial)
        {
            var edit = new Transaction();

            var sel = GetFaceSelection();
            if (trivial)
            {
                // Remember the state before the last change
                if (_memoTextures == null) _memoTextures = sel.ToDictionary(x => x, x => x.Texture.Clone());

                // Once a trivial change is started we know that there will definitely be a matching nontrivial task
                // We aggregate changes so they don't spam the undo stack
                edit.Add(new TrivialOperation(
                    x =>
                    {
                        foreach (var it in sel.GetSelectedFaces())
                        {
                            ApplyFaceValues(it.Value);
                        }
                    },
                    x =>
                    {
                        foreach (var p in sel.GetSelectedParents())
                        {
                            x.Update(p);
                        }
                    }
                ));
            }
            else
            {
                foreach (var it in sel.GetSelectedFaces())
                {
                    // Restore the last committed values
                    if (_memoTextures != null && _memoTextures.ContainsKey(it.Value))
                    {
                        var k = _memoTextures[it.Value];
                        it.Value.Texture.Unclone(k);
                    }

                    var clone = (Face) it.Value.Clone();
                    ApplyFaceValues(clone);

                    edit.Add(new RemoveMapObjectData(it.Key.ID, it.Value));
                    edit.Add(new AddMapObjectData(it.Key.ID, clone));
                }

                // Reset the memory
                _memoTextures = null;
            }

            MapDocumentOperation.Perform(Document, edit);
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            var item = GetFirstSelectedTexture();
            ApplyTexture(item);
        }

        private async Task ApplyChanges(Func<IMapObject, Face, Task<bool>> apply)
        {
            var sel = GetFaceSelection();

            var edit = new Transaction();
            var found = false;

            foreach (var it in sel.GetSelectedFaces())
            {
                var clone = (Face)it.Value.Clone();
                var result = await apply(it.Key, clone);
                if (!result) continue;

                found = true;

                edit.Add(new RemoveMapObjectData(it.Key.ID, it.Value));
                edit.Add(new AddMapObjectData(it.Key.ID, clone));
            }

            if (found)
            {
                await MapDocumentOperation.Perform(Document, edit);
                await FaceSelectionChanged();
            }
        }

        private async Task ApplyTexture(string item)
        {
            if (String.IsNullOrWhiteSpace(item)) return;

            await ApplyChanges((mo, f) =>
            {
                f.Texture.Name = item;
                ApplyFaceValues(f);
                return Task.FromResult(true);
            });
        }

        private void ScaleXValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            _currentTextureProperties.DifferentXScaleValues = false;
            PropertiesChanged();
        }

        private void ScaleYValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            _currentTextureProperties.DifferentYScaleValues = false;
            PropertiesChanged();
        }

        private void ShiftXValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            _currentTextureProperties.DifferentXShiftValues = false;
            PropertiesChanged();
        }

        private void ShiftYValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            _currentTextureProperties.DifferentYShiftValues = false;
            PropertiesChanged();
        }

        private void RotationValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            _currentTextureProperties.DifferentRotationValues = false;
            PropertiesChanged();
        }

        private void LightmapValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            PropertiesChanged();
        }

        private void JustifyTopClicked(object sender, EventArgs e)
        {
            Justify(BoxAlignMode.Top, false);
        }

        private async Task Justify(BoxAlignMode mode, bool fit)
        {
            var sel = GetFaceSelection();
            if (sel.IsEmpty) return;
            
            Cloud cloud = null;
            if (ShouldTreatAsOne()) cloud = new Cloud(sel.GetSelectedFaces().SelectMany(x => x.Value.Vertices));

            var tc = await Document.Environment.GetTextureCollection();
            if (tc == null) return;
            
            await ApplyChanges(async (mo, f) =>
            {
                var tex = await tc.GetTextureItem(f.Texture.Name);
                if (tex == null) return false;

                if (fit) f.Texture.FitToPointCloud(tex.Width, tex.Height, cloud ?? new Cloud(f.Vertices), 1, 1);
                else f.Texture.AlignWithPointCloud(tex.Width, tex.Height, cloud ?? new Cloud(f.Vertices), mode);

                return true;
            });
        }

        private void JustifyLeftClicked(object sender, EventArgs e)
        {
            Justify(BoxAlignMode.Left, false);
        }

        private void JustifyCenterClicked(object sender, EventArgs e)
        {
            Justify(BoxAlignMode.Center, false);
        }

        private void JustifyRightClicked(object sender, EventArgs e)
        {
            Justify(BoxAlignMode.Right, false);
        }

        private void JustifyBottomClicked(object sender, EventArgs e)
        {
            Justify(BoxAlignMode.Bottom, false);
        }

        private void JustifyFitClicked(object sender, EventArgs e)
        {
            Justify(BoxAlignMode.Center, true);
        }

        private void HideMaskCheckboxToggled(object sender, EventArgs e)
        {
            if (_freeze) return;
            var data = Document.Map.Data.GetOne<HideFaceMask>() ?? new HideFaceMask();
            data = new HideFaceMask {Hidden = !data.Hidden};
            MapDocumentOperation.Perform(Document, new TrivialOperation(x => x.Map.Data.Replace(data), x => x.Update(data)));
        }

        private void RecentFilterTextChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            UpdateRecentTextureList();
        }

        private void AlignToWorldClicked(object sender, EventArgs e)
        {
            ApplyChanges((mo, f) =>
            {
                f.Texture.AlignToNormal(f.Plane.GetClosestAxisToNormal());
                return Task.FromResult(true);
            });
        }

        private void AlignToFaceClicked(object sender, EventArgs e)
        {
            ApplyChanges((mo, f) =>
            {
                f.Texture.AlignToNormal(f.Plane.Normal);
                return Task.FromResult(true);
            });
        }

        private void BrowseButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("Command:Run", new CommandMessage("BspEditor:BrowseActiveTexture"));
        }

        private void TexturesListTextureSelected(object sender, string item)
        {
            ApplyTexture(item);
        }

        private void TreatAsOneCheckboxToggled(object sender, EventArgs e)
        {
            if (_freeze) return;
            // Nothing required here
        }

        private void ReplaceButtonClicked(object sender, EventArgs e)
        {
            Oy.Publish("Command:Run", new CommandMessage("BspEditor:ReplaceTextures"));
        }

        private void SmoothingGroupsButtonClicked(object sender, EventArgs e)
        {
            // TODO Source: Texture Smoothing Groups
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Oy.Publish("ActivateTool", "SelectTool");
            }
        }

        private void FocusTextInControl(object sender, EventArgs e)
        {
            var nud = sender as NumericUpDown;
            nud?.Select(0, nud.Text.Length);
        }

        public bool ShouldTreatAsOne()
        {
            return TreatAsOneCheckbox.Checked;
        }

        private class CurrentTextureProperties : Primitives.Texture
        {
            public bool DifferentXScaleValues { get; set; }
            public bool DifferentYScaleValues { get; set; }

            public bool DifferentXShiftValues { get; set; }
            public bool DifferentYShiftValues { get; set; }

            public bool DifferentRotationValues { get; set; }

            public bool AllAlignedToFace { get; set; }
            public bool NoneAlignedToFace { get; set; }

            public bool AllAlignedToWorld { get; set; }
            public bool NoneAlignedToWorld { get; set; }

            public CurrentTextureProperties()
            {
                Reset();
            }

            public void Reset()
            {
                Rotation = XShift = YShift = 0;
                XScale = YScale = 1;
                DifferentXScaleValues = DifferentYScaleValues = DifferentXShiftValues = DifferentYShiftValues = false;
                AllAlignedToFace = AllAlignedToWorld = false;
                NoneAlignedToFace = NoneAlignedToWorld = true;
            }

            public void Reset(IEnumerable<Face> faces)
            {
                Reset();
                var num = 0;
                AllAlignedToWorld = NoneAlignedToWorld = AllAlignedToFace = NoneAlignedToFace = true;
                foreach (var face in faces)
                {
                    if (face.Texture.IsAlignedToNormal(face.Plane.Normal)) NoneAlignedToFace = false;
                    else AllAlignedToFace = false;

                    if (face.Texture.IsAlignedToNormal(face.Plane.GetClosestAxisToNormal())) NoneAlignedToWorld = false;
                    else AllAlignedToWorld = false;

                    if (num == 0)
                    {
                        if (!float.IsNaN(face.Texture.XScale)) XScale = face.Texture.XScale;
                        if (!float.IsNaN(face.Texture.YScale)) YScale = face.Texture.YScale;
                        if (!float.IsNaN(face.Texture.XShift)) XShift = face.Texture.XShift;
                        if (!float.IsNaN(face.Texture.YShift)) YShift = face.Texture.YShift;
                        if (!float.IsNaN(face.Texture.Rotation)) Rotation = face.Texture.Rotation;
                    }
                    else
                    {
                        if (face.Texture.XScale != XScale) DifferentXScaleValues = true;
                        if (face.Texture.YScale != YScale) DifferentYScaleValues = true;
                        if (face.Texture.XShift != XShift) DifferentXShiftValues = true;
                        if (face.Texture.YShift != YShift) DifferentYShiftValues = true;
                        if (face.Texture.Rotation != Rotation) DifferentRotationValues = true;
                    }
                    num++;
                }

                // WinForms hack: use a tiny decimal place so that the NumericUpDown controls work when the value is typed into the box
                // E.g. Different X scale defaults to value of 1, but if 1 is typed in the box, the ValueChanged event won't fire since the backing value hasn't changed
                // Setting the value to 1.000001 instead triggers the change event properly, and since the NUD rounds to 4 decimal places, pressing the up/down buttons will start from the rounded value.
                if (DifferentXScaleValues) XScale = 1.000001f;
                if (DifferentYScaleValues) YScale = 1.000001f;
                if (DifferentXShiftValues) XShift = 0.000001f;
                if (DifferentYShiftValues) YShift = 0.000001f;
                if (DifferentRotationValues) Rotation = 0.000001f;

                if (XScale < -4096 || XScale > 4096) XScale = 1;
                if (YScale < -4096 || YScale > 4096) YScale = 1;
                if (XShift < -4096 || XShift > 4096) XShift = 1;
                if (YShift < -4096 || YShift > 4096) YShift = 1;
                Rotation = (Rotation % 360 + 360) % 360;
            }
        }
    }
}
