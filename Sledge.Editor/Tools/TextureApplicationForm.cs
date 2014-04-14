﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Providers.Texture;
using Sledge.Editor.UI;
using Sledge.Settings;

namespace Sledge.Editor.Tools
{
    public partial class TextureApplicationForm : HotkeyForm
    {
        public class CurrentTextureProperties : TextureReference
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
                    if (face.IsTextureAlignedToFace()) NoneAlignedToFace = false;
                    else AllAlignedToFace = false;
                    if (face.IsTextureAlignedToWorld()) NoneAlignedToWorld = false;
                    else AllAlignedToWorld = false;

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
            }
        }

        #region Events

        public delegate void TextureSelectBehaviourChangedEventHandler(object sender, TextureTool.SelectBehaviour left, TextureTool.SelectBehaviour right);
        public delegate void TexturePropertiesChangedEventHandler(object sender, CurrentTextureProperties properties);
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

        protected virtual void OnPropertyChanged(CurrentTextureProperties properties)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, properties);
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

        private readonly CurrentTextureProperties _currentTextureProperties;

        public Documents.Document Document { get; set; }

        public TextureApplicationForm()
        {
            _freeze = true;
            InitializeComponent();
            SelectedTexturesList.SelectionChanged += TextureSelectionChanged;
            RecentTexturesList.SelectionChanged += TextureSelectionChanged;
            _freeze = false;
            _currentTextureProperties = new CurrentTextureProperties();
        }

        public void Clear()
        {
            SelectedTexturesList.Clear();
            RecentTexturesList.Clear();
            _currentTextureProperties.Reset();
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
                TextureDetailsLabel.Text = string.Format("{0} ({1} x {2})", item.Name, item.Width, item.Height);
            }
            _freeze = false;
        }

        private void UpdateRecentTextureList()
        {
            RecentTexturesList.SetTextureList(Document.TextureCollection.GetRecentTextures().Where(x => x.Name.ToLower().Contains(RecentFilterTextbox.Text.ToLower())));
        }

        public void SelectTexture(TextureItem item)
        {
            if (item == null)
            {
                SelectedTexturesList.SetSelectedTextures(new TextureItem[0]);
                return;
            }

            UpdateRecentTextureList();

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

            var faces = Document.Selection.GetSelectedFaces().ToList();
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
            var textures = new List<TextureItem>();

            foreach (var face in faces)
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

            if (!_currentTextureProperties.DifferentXScaleValues) _currentTextureProperties.XScale = ScaleXValue.Value;
            if (!_currentTextureProperties.DifferentYScaleValues) _currentTextureProperties.YScale = ScaleYValue.Value;
            if (!_currentTextureProperties.DifferentXShiftValues) _currentTextureProperties.XShift = ShiftXValue.Value;
            if (!_currentTextureProperties.DifferentYShiftValues) _currentTextureProperties.YShift = ShiftYValue.Value;
            if (!_currentTextureProperties.DifferentRotationValues) _currentTextureProperties.Rotation = RotationValue.Value;

            OnPropertyChanged(_currentTextureProperties);
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

        //mxd
        private void TexturesListTextureSelected(object sender, TextureItem item)
        {
            OnTextureApply(item);
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

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.Texture);
            }
        }
    }
}
