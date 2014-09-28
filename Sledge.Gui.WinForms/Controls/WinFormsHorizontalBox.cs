using System;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Controls;
using Padding = System.Windows.Forms.Padding;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsHorizontalBox : WinFormsContainer, IHorizontalBox
    {
        private TableLayoutPanel _table;
        public bool Uniform { get; set; }
        public int ControlPadding { get; set; }

        public override Size PreferredSize
        {
            get
            {
                var width = 10;
                var height = 10;
                foreach (var child in Children)
                {
                    var ps = child.PreferredSize;
                    width += ps.Width;
                    height = Math.Max(height, ps.Height);
                }
                width += Margin.Left + Margin.Right;
                height += Margin.Top + Margin.Bottom;
                return new Size(width, height);
            }
        }

        public WinFormsHorizontalBox() : base(new TableLayoutPanel { RowCount = 1, ColumnCount = 1 })
        {
            _table = (TableLayoutPanel)Control;
            _table.Padding = new Padding(0);
            _table.Margin = new Padding(0);
            Uniform = false;
            ControlPadding = 3;
        }

        public void Insert(int index, IControl child, bool fill)
        {
            Insert(index, child, new ContainerMetadata { { "Fill", fill } });
        }

        protected override void OnPreferredSizeChanged()
        {
            CalculateLayout();
            base.OnPreferredSizeChanged();
        }

        protected override void CalculateLayout()
        {
            while (_table.ColumnStyles.Count > NumChildren) _table.ColumnStyles.RemoveAt(0);
            while (_table.ColumnStyles.Count < NumChildren) _table.ColumnStyles.Add(new ColumnStyle());
            _table.ColumnCount = NumChildren;

            var numFill = 0;
            var desiredWidth = 0;
            foreach (var child in Children)
            {
                var meta = Metadata[child];
                var fill = Uniform || meta.Get<bool>("Fill");
                if (fill) numFill++;
                else desiredWidth += child.PreferredSize.Width + ControlPadding;
            }
            var fillVal = numFill == 0 ? 0 : 100.0f / numFill;
            var standardVal = desiredWidth > 0 ? Math.Min(1, ActualSize.Width / (float)desiredWidth) : 1;
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var meta = Metadata[child];
                var fill = Uniform || meta.Get<bool>("Fill");
                _table.ColumnStyles[i].SizeType = fill ? SizeType.Percent : SizeType.Absolute;
                _table.ColumnStyles[i].Width = fill ? fillVal : standardVal * child.PreferredSize.Width + ControlPadding;
                child.Control.Dock = DockStyle.Fill;
                child.Control.Margin = new Padding(ControlPadding, ControlPadding / 2, ControlPadding, (int)Math.Ceiling(ControlPadding / 2.0));
                _table.SetCellPosition(child.Control, new TableLayoutPanelCellPosition(i, 0));
            }
            if (numFill == 0)
            {
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                _table.ColumnCount += 1;
            }
        }
    }
}