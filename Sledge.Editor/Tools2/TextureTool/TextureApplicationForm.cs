using System.Collections.Generic;
using System.Drawing;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Documents;
using Sledge.Gui;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Shell;
using Sledge.Gui.Structures;
using Sledge.Gui.WinForms.Controls;
using Sledge.Providers.Texture;
using Size = Sledge.Gui.Structures.Size;

namespace Sledge.EditorNew.Tools.TextureTool
{
    public class TextureApplicationForm : Window
    {
        public Document Document { get; set; }
        public CurrentTextureProperties CurrentProperties { get; set; }

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

                if (XScale < -4096 || XScale > 4096) XScale = 1;
                if (YScale < -4096 || YScale > 4096) YScale = 1;
                if (XShift < -4096 || XShift > 4096) XShift = 1;
                if (YShift < -4096 || YShift > 4096) YShift = 1;
                Rotation = (Rotation % 360 + 360) % 360;
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
                TextureJustify(this, mode, ShouldTreatAsOne());
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

        public TextureApplicationForm()
        {
            Owner = UIManager.Manager.Shell;
            Build();
        }

        private void Build()
        {
            const string key = "Tools/TextureTool/Controls/";

            var outer = new VerticalBox();
            var top = new HorizontalBox();

            var topLeft = new VerticalBox();

            var numberTable = new Table();
            numberTable.Insert(0, 1, new Label { TextKey = key + "Scale" });
            numberTable.Insert(0, 2, new Label { TextKey = key + "Shift" });
            numberTable.Insert(1, 0, new Label { TextKey = key + "X" });
            numberTable.Insert(2, 0, new Label { TextKey = key + "Y" });

            var xScale = new NumericSpinner { PreferredSize = new Size(50, 20) };
            var yScale = new NumericSpinner { PreferredSize = new Size(50, 20) };
            var xShift = new NumericSpinner { PreferredSize = new Size(50, 20) };
            var yShift = new NumericSpinner { PreferredSize = new Size(50, 20) };

            numberTable.Insert(1, 1, xScale);
            numberTable.Insert(2, 1, yScale);
            numberTable.Insert(1, 2, xShift);
            numberTable.Insert(2, 2, yShift);
            topLeft.Add(numberTable);

            var browse = new Button { TextKey = key + "Browse" };
            var replace = new Button { TextKey = key + "Replace" };
            var apply = new Button { TextKey = key + "Apply" };

            var buttonsPanel = new HorizontalBox();
            var twoButtons = new VerticalBox();
            twoButtons.Add(browse);
            twoButtons.Add(replace);
            buttonsPanel.Add(twoButtons, true);
            buttonsPanel.Add(apply, true);
            topLeft.Add(buttonsPanel);

            top.Add(topLeft);

            var topMiddle = new VerticalBox();

            var rotation = new NumericSpinner {PreferredSize = new Size(50, 20)};
            var lightmap = new NumericSpinner {PreferredSize = new Size(50, 20)};
            var smoothingGroups = new Button {TextKey = key + "SmoothingGroups"};

            var rotBox = new HorizontalBox();
            rotBox.Add(new Label { TextKey = key + "Rotation" }, true);
            rotBox.Add(rotation);
            topMiddle.Add(rotBox);

            var litBox = new HorizontalBox();
            litBox.Add(new Label { TextKey = key + "Lightmap" }, true);
            litBox.Add(lightmap);
            topMiddle.Add(litBox);

            topMiddle.Add(smoothingGroups);

            // Group box for world/face

            top.Add(topMiddle);

            var topRight = new VerticalBox();

            // group box for justify buttons

            var treatAsOne = new CheckBox { TextKey = key + "TreatAsOne" };
            var hideMask = new CheckBox { TextKey = key + "HideMask" };

            topRight.Add(treatAsOne);
            topRight.Add(hideMask);

            top.Add(topRight);
            
            outer.Add(top);

            var selectedTextureLabel = new Label();

            outer.Add(selectedTextureLabel);

            // asset browser panels

            // left/right click actions
            // filter box

            Container.Set(outer);
        }

        public bool ShouldTreatAsOne()
        {
            return false;
        }

        public void SelectionChanged()
        {
            // 
        }

        public void SelectTexture(TextureItem selectedTexture)
        {
            // 
        }

        public void Clear()
        {
            // 
        }

        public TextureTool.SelectBehaviour GetLeftClickBehaviour(bool ctrl, bool shift, bool alt)
        {
            return TextureTool.SelectBehaviour.Lift;
        }

        public TextureTool.SelectBehaviour GetRightClickBehaviour(bool ctrl, bool shift, bool alt)
        {
            return TextureTool.SelectBehaviour.Apply;
        }

        public TextureItem GetFirstSelectedTexture()
        {
            return null;
        }
    }
}