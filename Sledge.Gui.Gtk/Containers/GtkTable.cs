using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Gtk.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Gtk.Containers
{
    [ControlImplementation("GTK")]
    public class GtkTable : GtkContainer, ITable
    {
        protected Table Table { get; private set; }

        public int ControlPadding
        {
            get { return (int) Table.ColumnSpacing; }
            set { Table.ColumnSpacing = Table.RowSpacing = (uint) value; }
        }

        public GtkTable() : base(new Table(1, 1, false))
        {
            Table = (Table) Container;
        }

        protected override void CalculateLayout()
        {

        }

        public int[] GetColumnWidths()
        {
            return new int[Table.NColumns]; // todo
        }

        public int[] GetRowHeights()
        {
            return new int[Table.NRows]; // todo
        }

        public void Insert(int row, int column, IControl child, int rowSpan = 1, int columnSpan = 1, bool rowFill = false, bool columnFill = false)
        {
            Insert(NumChildren, child, new ContainerMetadata
            {
                {"Row", row},
                {"Column", column},
                {"RowSpan", rowSpan},
                {"ColumnSpan", columnSpan},
                {"RowFill", rowFill},
                {"ColumnFill", columnFill}
            });
        }

        public override void Insert(int index, IControl child, ContainerMetadata metadata)
        {
            if (metadata.Get("Row", -1) < 0) throw new Exception("The Row must be specified and non-negative.");
            if (metadata.Get("Column", -1) < 0) throw new Exception("The Column must be specified and non-negative.");
            if (metadata.Get("RowSpan", 0) <= 0) metadata["RowSpan"] = 1;
            if (metadata.Get("ColumnSpan", 0) <= 0) metadata["ColumnSpan"] = 1;
            if (!metadata.Get("RowFill", false)) metadata["RowFill"] = false;
            if (!metadata.Get("ColumnFill", false)) metadata["ColumnFill"] = false;
            base.Insert(index, child, metadata);
        }

        protected override void AppendChild(int index, GtkControl child)
        {
            var meta = Metadata[child];
            var r = (uint) meta.Get<int>("Row");
            var rs = (uint) meta.Get<int>("RowSpan");
            var c = (uint) meta.Get<int>("Column");
            var cs = (uint) meta.Get<int>("ColumnSpan");
            var rf = meta.Get<bool>("RowFill");
            var cf = meta.Get<bool>("ColumnFill");
            Table.Attach(child.Control, c, c + cs, r, r + rs,
                rf ? AttachOptions.Expand | AttachOptions.Fill : 0,
                cf ? AttachOptions.Expand | AttachOptions.Fill : 0,
                0, 0);
            child.Control.ShowAll();
        }

        public void SetColumnWidth(int column, int width)
        {
            throw new NotImplementedException();
        }

        public void SetRowHeight(int row, int height)
        {
            throw new NotImplementedException();
        }
    }
}
