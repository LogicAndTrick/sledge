using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Providers.Texture;
using Sledge.Editor.Editing;
using Sledge.Editor.UI;
using Sledge.Settings;

namespace Sledge.Editor.Tools
{
    public partial class TextureApplicationForm : HotkeyForm
    {

        #region Events

        public delegate void TextureSelectBehaviourChangedEventHandler(object sender, TextureTool.SelectBehaviour left, TextureTool.SelectBehaviour right);
        public delegate void TexturePropertiesChangedEventHandler(object sender, decimal scaleX, decimal scaleY, int shiftX, int shiftY, decimal rotation, int lightmapScale);
        public delegate void TextureChangedEventHandler(object sender, TextureItem texture);
        public delegate void TextureHideMaskToggledEventHandler(object sender, bool hide);
        public delegate void TextureJustifyEventHandler(object sender, TextureTool.JustifyMode justify, bool treatAsOne);
        public delegate void TextureApplyEventHandler(object sender, TextureItem texture);
        public delegate void TextureAlignEventHandler(object sender, TextureTool.AlignMode align);

        public event TexturePropertiesChangedEventHandler PropertyChanged;
        public event TextureChangedEventHandler TextureChanged;
        public event TextureSelectBehaviourChangedEventHandler TextureModeChanged;
        public event TextureHideMaskToggledEventHandler HideMaskToggled;
        public event TextureJustifyEventHandler TextureJustify;
        public event TextureApplyEventHandler TextureApply;
        public event TextureAlignEventHandler TextureAlign;

        protected virtual void OnTextureAlign(TextureTool.AlignMode align)
        {
            if (TextureAlign != null)
            {
                TextureAlign(this, align);
            }
        }

        protected virtual void OnTextureApply(TextureItem texture)
        {
            if (TextureApply != null)
            {
                TextureApply(this, texture);
            }
        }

        protected virtual void OnTextureJustify(TextureTool.JustifyMode mode)
        {
            if (TextureJustify != null)
            {
                TextureJustify(this, mode, TreatAsOneCheckbox.Checked);
            }
        }

        protected virtual void OnHideMaskToggled(bool hide)
        {
            if (HideMaskToggled != null)
            {
                HideMaskToggled(this, hide);
            }
        }

        protected virtual void OnTextureModeChanged(TextureTool.SelectBehaviour left, TextureTool.SelectBehaviour right)
        {
            if (TextureModeChanged != null)
            {
                TextureModeChanged(this, left, right);
            }
        }

        protected virtual void OnPropertyChanged(decimal scaleX, decimal scaleY, int shiftX, int shiftY, decimal rotation, int lightmapScale)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, scaleX, scaleY, shiftX, shiftY, rotation, lightmapScale);
            }
        }

        protected virtual void OnTextureChanged(TextureItem texture)
        {
            if (TextureChanged != null)
            {
                TextureChanged(this, texture);
            }
        }

        #endregion

        private bool _freeze;
        private decimal _lastScaleX;
        private decimal _lastScaleY;
        private List<TextureItem> _recentTextures;

        public Documents.Document Document { get; set; }

        public TextureApplicationForm()
        {
            _freeze = true;
            _recentTextures = new List<TextureItem>();
            InitializeComponent();
            SelectedTexturesList.SelectionChanged += TextureSelectionChanged;
            RecentTexturesList.SelectionChanged += TextureSelectionChanged;
            _freeze = false;
            _lastScaleX = _lastScaleY = 1;
        }

        public void Clear()
        {
            SelectedTexturesList.Clear();
            _recentTextures.Clear();
            RecentTexturesList.Clear();
        }

        public TextureItem GetFirstSelectedTexture()
        {
            return RecentTexturesList
                .GetSelectedTextures()
                .Union(SelectedTexturesList.GetSelectedTextures())
                .FirstOrDefault();
        }

        public IEnumerable<TextureItem> GetSelectedTextures()
        {
            return RecentTexturesList
                .GetSelectedTextures()
                .Union(SelectedTexturesList.GetSelectedTextures());
        }

        private void TextureSelectionChanged(object sender, IEnumerable<TextureItem> selection)
        {
            if (_freeze) return;

            _freeze = true;
            var item = selection.FirstOrDefault();
            if (selection.Any())
            {
                if (sender == SelectedTexturesList) RecentTexturesList.SetSelectedTextures(new TextureItem[0]);
                if (sender == RecentTexturesList) SelectedTexturesList.SetSelectedTextures(new TextureItem[0]);
            }
            else
            {
                item = RecentTexturesList
                    .GetSelectedTextures()
                    .Union(SelectedTexturesList.GetSelectedTextures())
                    .FirstOrDefault();
            }
            TextureDetailsLabel.Text = "";
            if (item != null)
            {
                TextureDetailsLabel.Text = string.Format("{0} ({1}x{2})", item.Name, item.Width, item.Height);
            }
            _freeze = false;
        }

        private void UpdateRecentTextureList()
        {
            RecentTexturesList.SetTextureList(_recentTextures.Where(x => x.Name.ToLower().Contains(RecentFilterTextbox.Text.ToLower())));
        }

        public void SelectTexture(TextureItem item)
        {
            if (item == null)
            {
                SelectedTexturesList.SetSelectedTextures(new TextureItem[0]);
                return;
            }
            // Add the texture to the recent texture list
            if (!_recentTextures.Contains(item))
            {
                _recentTextures.Insert(0, item);
                UpdateRecentTextureList();
            }

            // If the texture is in the list of selected faces, select the texture in that list
            var sl = SelectedTexturesList.GetTextures();
            if (sl.Contains(item))
            {
                SelectedTexturesList.SetSelectedTextures(new[] { item });
                SelectedTexturesList.ScrollToItem(item);
            }
            else if (RecentTexturesList.GetTextures().Contains(item))
            {
                // Otherwise, select the texture in the recent list
                RecentTexturesList.SetSelectedTextures(new[] {item});
                RecentTexturesList.ScrollToItem(item);
            }
            RecentTexturesList.Refresh();
            SelectedTexturesList.Refresh();
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        public TextureTool.SelectBehaviour GetLeftClickBehaviour(bool ctrl, bool shift, bool alt)
        {
            switch (LeftClickCombo.SelectedItem.ToString())
            {
                case "Lift and Select":
                    return TextureTool.SelectBehaviour.LiftSelect;
                case "Lift":
                    return TextureTool.SelectBehaviour.Lift;
                case "Select":
                    return TextureTool.SelectBehaviour.Select;
            }
            TextureTool.SelectBehaviour b;
            if (Enum.TryParse(LeftClickCombo.SelectedItem.ToString(), true, out b))
            {
                return b;
            }
            return TextureTool.SelectBehaviour.LiftSelect;
        }

        public TextureTool.SelectBehaviour GetRightClickBehaviour(bool ctrl, bool shift, bool alt)
        {
            switch (RightClickCombo.SelectedItem.ToString())
            {
                case "Apply Texture":
                    return alt ? TextureTool.SelectBehaviour.ApplyWithValues : TextureTool.SelectBehaviour.Apply;
                case "Apply Texture and Values":
                    return TextureTool.SelectBehaviour.ApplyWithValues;
                case "Align To View":
                    return TextureTool.SelectBehaviour.AlignToView;
            }
            TextureTool.SelectBehaviour b;
            if (Enum.TryParse(RightClickCombo.SelectedItem.ToString(), true, out b))
            {
                if (b == TextureTool.SelectBehaviour.Apply && alt) return TextureTool.SelectBehaviour.ApplyWithValues;
                return b;
            }
            return alt ? TextureTool.SelectBehaviour.ApplyWithValues : TextureTool.SelectBehaviour.Apply;
        }

        public void SelectionChanged()
        {
            _freeze = true;

            ScaleYValue.Value = ScaleYValue.Value = 1;
            ShiftXValue.Value = ShiftYValue.Value = 0;
            TextureDetailsLabel.Text = "";
            var textures = new List<TextureItem>();
            var first = true;
            foreach (var face in Document.Selection.GetSelectedFaces())
            {
                var tex = face.Texture;
                if (tex.Texture != null && textures.All(x => !String.Equals(x.Name, tex.Texture.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var item = Document.TextureCollection.GetItem(tex.Texture.Name);
                    if (item != null)
                    {
                        textures.Add(item);
                    }
                }

                if (first) ScaleXValue.Value = tex.XScale;
                else if (ScaleXValue.Text != "" && ScaleXValue.Value != tex.XScale)
                {
                    ScaleXValue.Value = 1;
                    ScaleXValue.Text = "";
                }

                if (first) ScaleYValue.Value = tex.YScale;
                else if (ScaleYValue.Text != "" && ScaleYValue.Value != tex.YScale)
                {
                    ScaleYValue.Value = 1;
                    ScaleYValue.Text = "";
                }

                if (first) ShiftXValue.Value = tex.XShift;
                else if (ShiftXValue.Text != "" && ShiftXValue.Value != tex.XShift)
                {
                    ShiftXValue.Value = 0;
                    ShiftXValue.Text = "";
                }

                if (first) ShiftYValue.Value = tex.YShift;
                else if (ShiftYValue.Text != "" && ShiftYValue.Value != tex.YShift)
                {
                    ShiftYValue.Value = 0;
                    ShiftYValue.Text = "";
                }

                if (first) RotationValue.Value = tex.Rotation;
                else if (RotationValue.Text != "" && RotationValue.Value != tex.Rotation)
                {
                    RotationValue.Value = 0;
                    RotationValue.Text = "";
                }

                first = false;
            }

            if (textures.Any())
            {
                var t = textures[0];
                TextureDetailsLabel.Text = string.Format("{0} ({1}x{2})", t.Name, t.Width, t.Height);
            }

            SelectedTexturesList.SetTextureList(textures);
            SelectedTexturesList.SetSelectedTextures(textures);
            RecentTexturesList.SetSelectedTextures(new TextureItem[0]);
            HideMaskCheckbox.Checked = Document.Map.HideFaceMask;
            if (LeftClickCombo.SelectedIndex < 0) LeftClickCombo.SelectedIndex = 0;
            if (RightClickCombo.SelectedIndex < 0) RightClickCombo.SelectedIndex = 0;

            _freeze = false;
        }

        private void PropertiesChanged()
        {
            if (_freeze) return;

            _freeze = true;
            if (ScaleXValue.Value == 0)
            {
                ScaleXValue.Value -= _lastScaleX;
            }
            if (ScaleYValue.Value == 0)
            {
                ScaleYValue.Value -= _lastScaleY;
            }
            _lastScaleX = ScaleXValue.Value;
            _lastScaleY = ScaleYValue.Value;
            _freeze = false;

            OnPropertyChanged(ScaleXValue.Value, ScaleYValue.Value,
                              (int) ShiftXValue.Value, (int) ShiftYValue.Value,
                              RotationValue.Value, (int) LightmapValue.Value);
        }

        private void ScaleXValueChanged(object sender, EventArgs e)
        {
            PropertiesChanged();
        }

        private void ScaleYValueChanged(object sender, EventArgs e)
        {
            PropertiesChanged();
        }

        private void ShiftXValueChanged(object sender, EventArgs e)
        {
            PropertiesChanged();
        }

        private void ShiftYValueChanged(object sender, EventArgs e)
        {
            PropertiesChanged();
        }

        private void RotationValueChanged(object sender, EventArgs e)
        {
            PropertiesChanged();
        }

        private void LightmapValueChanged(object sender, EventArgs e)
        {
            PropertiesChanged();
        }

        private void JustifyTopClicked(object sender, EventArgs e)
        {
            OnTextureJustify(TextureTool.JustifyMode.Top);
        }

        private void JustifyLeftClicked(object sender, EventArgs e)
        {
            OnTextureJustify(TextureTool.JustifyMode.Left);
        }

        private void JustifyCenterClicked(object sender, EventArgs e)
        {
            OnTextureJustify(TextureTool.JustifyMode.Center);
        }

        private void JustifyRightClicked(object sender, EventArgs e)
        {
            OnTextureJustify(TextureTool.JustifyMode.Right);
        }

        private void JustifyBottomClicked(object sender, EventArgs e)
        {
            OnTextureJustify(TextureTool.JustifyMode.Bottom);
        }

        private void JustifyFitClicked(object sender, EventArgs e)
        {
            OnTextureJustify(TextureTool.JustifyMode.Fit);
        }

        private void LeftClickComboChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            // Nothing needed
        }

        private void RightClickComboChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            // Nothing needed
        }

        private void HideMaskCheckboxToggled(object sender, EventArgs e)
        {
            if (_freeze) return;
            OnHideMaskToggled(HideMaskCheckbox.Checked);
        }

        private void RecentFilterTextChanged(object sender, EventArgs e)
        {
            if (_freeze) return;
            UpdateRecentTextureList();
        }

        private void AlignToWorldClicked(object sender, EventArgs e)
        {
            OnTextureAlign(TextureTool.AlignMode.World);
        }

        private void AlignToFaceClicked(object sender, EventArgs e)
        {
            OnTextureAlign(TextureTool.AlignMode.Face);
        }

        private void BrowseButtonClicked(object sender, EventArgs e)
        {
            using (var browser = new TextureBrowser())
            {
                browser.SetTextureList(Document.TextureCollection.GetAllItems());
                browser.ShowDialog();

                if (browser.SelectedTexture == null) return;
                Mediator.Publish(EditorMediator.TextureSelected, browser.SelectedTexture);
                if (Sledge.Settings.Select.ApplyTextureImmediately)
                {
                    ApplyButtonClicked(sender, e);
                }
            }
        }

        private void ApplyButtonClicked(object sender, EventArgs e)
        {
            var item = GetFirstSelectedTexture();
            if (item != null)
            {
                OnTextureApply(item);
            }
        }

        private void TreatAsOneCheckboxToggled(object sender, EventArgs e)
        {
            if (_freeze) return;
            // Nothing required here
        }

        private void ReplaceButtonClicked(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.ReplaceTextures);
        }

        private void SmoothingGroupsButtonClicked(object sender, EventArgs e)
        {
            // TODO SOURCE: Texture Smoothing Groups
        }
    }
}
