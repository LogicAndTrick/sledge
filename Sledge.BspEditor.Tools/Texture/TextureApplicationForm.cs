using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Shell;

namespace Sledge.BspEditor.Tools.Texture
{
    [Export(typeof(IDialog))]
    public partial class TextureApplicationForm : Shell.Forms.BaseForm, IDialog
    {
        // We want to use the shell for the parent
        [Import("Shell", typeof(Form))] private Form _shell;
        [Import] private TextureTool _textureTool;

        private readonly List<string> _recentTextures = new List<string>();
        private bool _freeze;
        private readonly CurrentTextureProperties _currentTextureProperties;
        private WeakReference<MapDocument> _document;

        private event EventHandler DebouncedPropertiesChanged;
        private IDisposable _saveChanges;

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
                var precision = 4; // todo  _document != null && _document.Game != null && _document.Game.Engine == Engine.Goldsource ? 2 : 4;
                ScaleXValue.DecimalPlaces = ScaleYValue.DecimalPlaces = precision;
            }
        }

        public TextureApplicationForm()
        {
            _freeze = true;

            InitializeComponent();

            SelectedTexturesList.SelectionChanged += TextureListSelectionChanged;
            RecentTexturesList.SelectionChanged += TextureListSelectionChanged;

            _freeze = false;

            _currentTextureProperties = new CurrentTextureProperties();

            Oy.Subscribe<IDocument>("Document:Activated", SetDocument);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
            Oy.Subscribe<object>("TextureTool:SelectionChanged", async x => await FaceSelectionChanged());
            
            _saveChanges = Observable.FromEventPattern(x => DebouncedPropertiesChanged += x, x => DebouncedPropertiesChanged -= x)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(x =>
                {
                    ApplyPropertyChanges(false);
                });
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
            if (_document.TryGetTarget(out MapDocument t) && change.Document == t && change.DocumentUpdated)
            {
                var at = t.Map.Data.GetOne<ActiveTexture>()?.Name;
                ActiveTextureChanged(at);
            }
        }

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveTool", out TextureTool _);
        }

        public void SetVisible(bool visible)
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
                .GetSelectedTextures()
                .Union(SelectedTexturesList.GetSelectedTextures())
                .FirstOrDefault();
        }

        private IEnumerable<string> GetSelectedTextures()
        {
            return RecentTexturesList
                .GetSelectedTextures()
                .Union(SelectedTexturesList.GetSelectedTextures());
        }
        
        private async void TextureListSelectionChanged(object sender, IEnumerable<string> sel)
        {
            if (_freeze) return;

            _freeze = true;

            var selection = sel.ToList();
            var item = selection.FirstOrDefault();

            if (selection.Any())
            {
                if (sender == SelectedTexturesList) RecentTexturesList.SetSelectedTextures(new string[0]);
                if (sender == RecentTexturesList) SelectedTexturesList.SetSelectedTextures(new string[0]);
            }
            else
            {
                item = RecentTexturesList
                    .GetSelectedTextures()
                    .Union(SelectedTexturesList.GetSelectedTextures())
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

                await MapDocumentOperation.Perform(Document, new TrivialOperation(
                    x => x.Map.Data.Replace(new ActiveTexture { Name = item }),
                    x => x.UpdateDocument()
                ));
            }

            TextureDetailsLabel.Invoke(() =>
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
                SelectedTexturesList.SetSelectedTextures(new string[0]);
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
                SelectedTexturesList.SetSelectedTextures(new[] { item });
                SelectedTexturesList.ScrollToItem(item);
            }
            else if (RecentTexturesList.GetTextureList().Contains(item))
            {
                // Otherwise, select the texture in the recent list
                RecentTexturesList.SetSelectedTextures(new[] {item});
                RecentTexturesList.ScrollToItem(item);
            }
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        private async Task FaceSelectionChanged()
        {
            // The list of selected faces has changed - update the texture properties to match the selection
            _freeze = true;

            var faces = _textureTool.GetSelection();
            _currentTextureProperties.Reset(faces);

            ScaleXValue.Value = _currentTextureProperties.XScale;
            ScaleYValue.Value = _currentTextureProperties.YScale;
            ShiftXValue.Value = _currentTextureProperties.XShift;
            ShiftYValue.Value = _currentTextureProperties.YShift;
            RotationValue.Value = _currentTextureProperties.Rotation;

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

            TextureDetailsLabel.Text = "";
            var textures = new List<string>();

            foreach (var face in faces)
            {
                var tex = face.Texture;

                var name = tex.Name;
                if (textures.Any(x => String.Equals(x, name, StringComparison.InvariantCultureIgnoreCase))) continue;
                
                textures.Add(name);
            }

            var d = Document;
            if (textures.Any() && d != null)
            {
                var t = textures[0];
                var tc = await d.Environment.GetTextureCollection();
                var ti = await tc.GetTextureItem(t);
                TextureDetailsLabel.Text = $"{ti.Name} ({ti.Width} x {ti.Height})";
            }

            SelectedTexturesList.SetTextureList(textures);
            SelectedTexturesList.SetSelectedTextures(textures);
            RecentTexturesList.SetSelectedTextures(new string[0]);
            // todo HideMaskCheckbox.Checked = Document.Map.HideFaceMask;

            _freeze = false;
        }

        private void PropertiesChanged()
        {
            if (_freeze) return;

            if (!_currentTextureProperties.DifferentXScaleValues) _currentTextureProperties.XScale = ScaleXValue.Value;
            if (!_currentTextureProperties.DifferentYScaleValues) _currentTextureProperties.YScale = ScaleYValue.Value;
            if (!_currentTextureProperties.DifferentXShiftValues) _currentTextureProperties.XShift = ShiftXValue.Value;
            if (!_currentTextureProperties.DifferentYShiftValues) _currentTextureProperties.YShift = ShiftYValue.Value;
            if (!_currentTextureProperties.DifferentRotationValues) _currentTextureProperties.Rotation = RotationValue.Value;

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
            if (!_currentTextureProperties.DifferentRotationValues) target.Texture.Rotation = _currentTextureProperties.Rotation;
        }

        private void ApplyPropertyChanges(bool trivial)
        {
            var edit = new Transaction();

            var sel = _textureTool.GetSelection();
            if (trivial)
            {
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
                    var clone = (Face) it.Value.Clone();
                    ApplyFaceValues(clone);

                    edit.Add(new RemoveMapObjectData(it.Key.ID, it.Value));
                    edit.Add(new AddMapObjectData(it.Key.ID, clone));
                    edit.Add(new TrivialOperation(
                        x =>
                        {
                            sel.Remove(it.Key, it.Value);
                            sel.Add(it.Key, clone);
                        },
                        x => { }
                    ));
                }
            }

            MapDocumentOperation.Perform(Document, edit);
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            var item = GetFirstSelectedTexture();
            ApplyTexture(item);
        }

        private async Task ApplyTexture(string item)
        {
            if (String.IsNullOrWhiteSpace(item)) return;

            var sel = _textureTool.GetSelection();

            var edit = new Transaction();

            foreach (var it in sel.GetSelectedFaces())
            {
                var clone = (Face)it.Value.Clone();
                clone.Texture.Name = item;
                ApplyFaceValues(clone);

                edit.Add(new RemoveMapObjectData(it.Key.ID, it.Value));
                edit.Add(new AddMapObjectData(it.Key.ID, clone));
                edit.Add(new TrivialOperation(
                    x =>
                    {
                        sel.Remove(it.Key, it.Value);
                        sel.Add(it.Key, clone);
                    },
                    x => { }
                ));
            }

            await MapDocumentOperation.Perform(Document, edit);
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
            // todo
        }

        private void JustifyLeftClicked(object sender, EventArgs e)
        {
            // todo
        }

        private void JustifyCenterClicked(object sender, EventArgs e)
        {
            // todo
        }

        private void JustifyRightClicked(object sender, EventArgs e)
        {
            // todo
        }

        private void JustifyBottomClicked(object sender, EventArgs e)
        {
            // todo
        }

        private void JustifyFitClicked(object sender, EventArgs e)
        {
            // todo
        }

        private void HideMaskCheckboxToggled(object sender, EventArgs e)
        {
            if (_freeze) return;
            // todo
        }

        private void RecentFilterTextChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            UpdateRecentTextureList();
        }

        private void AlignToWorldClicked(object sender, EventArgs e)
        {
            // todo
        }

        private void AlignToFaceClicked(object sender, EventArgs e)
        {
            // todo
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
            // todo Mediator.Publish(HotkeysMediator.ReplaceTextures);
        }

        private void SmoothingGroupsButtonClicked(object sender, EventArgs e)
        {
            // TODO SOURCE: Texture Smoothing Groups
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                // todo Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.Selection);
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

            public void Reset(IEnumerable<ITextured> faces)
            {
                Reset();
                var num = 0;
                AllAlignedToWorld = NoneAlignedToWorld = AllAlignedToFace = NoneAlignedToFace = true;
                foreach (var face in faces)
                {
                    // todo
                    // if (face.IsTextureAlignedToFace()) NoneAlignedToFace = false;
                    // else AllAlignedToFace = false;
                    // if (face.IsTextureAlignedToWorld()) NoneAlignedToWorld = false;
                    // else AllAlignedToWorld = false;

                    if (num == 0)
                    {
                        XScale = face.Texture.XScale;
                        YScale = face.Texture.YScale;
                        XShift = face.Texture.XShift;
                        YShift = face.Texture.YShift;
                        Rotation = face.Texture.Rotation;
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
                if (DifferentXScaleValues) XScale = 1.000001m;
                if (DifferentYScaleValues) YScale = 1.000001m;
                if (DifferentXShiftValues) XShift = 0.000001m;
                if (DifferentYShiftValues) YShift = 0.000001m;
                if (DifferentRotationValues) Rotation = 0.000001m;

                if (XScale < -4096 || XScale > 4096) XScale = 1;
                if (YScale < -4096 || YScale > 4096) YScale = 1;
                if (XShift < -4096 || XShift > 4096) XShift = 1;
                if (YShift < -4096 || YShift > 4096) YShift = 1;
                Rotation = (Rotation % 360 + 360) % 360;
            }
        }
    }
}
