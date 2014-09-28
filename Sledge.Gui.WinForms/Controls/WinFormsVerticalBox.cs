using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Controls;
using Padding = System.Windows.Forms.Padding;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsVerticalBox : WinFormsContainer, IVerticalBox
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
                    width = Math.Max(width, ps.Width);
                    height += ps.Height + ControlPadding;
                }
                width += Margin.Left + Margin.Right;
                height += Margin.Top + Margin.Bottom;
                return new Size(width, height);
            }
        }

        public WinFormsVerticalBox()
            : base(new TableLayoutPanel { RowCount = 1, ColumnCount = 1 })
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
            while (_table.RowStyles.Count > NumChildren) _table.RowStyles.RemoveAt(0);
            while (_table.RowStyles.Count < NumChildren) _table.RowStyles.Add(new RowStyle());
            _table.RowCount = NumChildren;

            var numFill = 0;
            var desiredHeight = 0;
            foreach (var child in Children)
            {
                var meta = Metadata[child];
                var fill = Uniform || meta.Get<bool>("Fill");
                if (fill) numFill++;
                else desiredHeight += child.PreferredSize.Height + ControlPadding;
            }
            var fillVal = numFill == 0 ? 0 : 100.0f / numFill;
            var standardVal = desiredHeight > 0 ? Math.Min(1, ActualSize.Height / (float)desiredHeight) : 1;
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var meta = Metadata[child];
                var fill = Uniform || meta.Get<bool>("Fill");
                _table.RowStyles[i].SizeType = fill ? SizeType.Percent : SizeType.Absolute;
                _table.RowStyles[i].Height = fill ? fillVal : standardVal * child.PreferredSize.Height + ControlPadding;
                child.Control.Dock = DockStyle.Fill;
                child.Control.Margin = new Padding(ControlPadding, ControlPadding / 2, ControlPadding, (int)Math.Ceiling(ControlPadding / 2.0));
                _table.SetCellPosition(child.Control, new TableLayoutPanelCellPosition(0, i));
            }
            if (numFill == 0)
            {
                _table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                _table.RowCount += 1;
            }
        }
    }
}