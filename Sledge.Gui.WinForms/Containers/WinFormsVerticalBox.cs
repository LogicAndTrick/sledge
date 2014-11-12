using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;
using Padding = System.Windows.Forms.Padding;
using Size = Sledge.Gui.Structures.Size;

namespace Sledge.Gui.WinForms.Containers
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
                var height = 0;
                foreach (var child in Children)
                {
                    var ps = child.PreferredSize;
                    width = Math.Max(width, ps.Width);
                    height += ps.Height;
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
            Uniform = false;
            ControlPadding = 3;
        }

        public void Insert(int index, IControl child, bool fill)
        {
            Insert(index, child, new ContainerMetadata { { "Fill", fill } });
        }

        protected override void CalculateLayout()
        {
            if (_table == null) return;

            Control.SuspendLayout();

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
                child.Control.Margin = new Padding(0, i == 0 ? 0 : ControlPadding / 2, 0, i == Children.Count - 1 ? 0 : (int)Math.Ceiling(ControlPadding / 2.0));
                _table.SetCellPosition(child.Control, new TableLayoutPanelCellPosition(0, i));
            }
            if (numFill == 0)
            {
                _table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                _table.RowCount += 1;
            }

            Control.ResumeLayout();
        }
    }
}