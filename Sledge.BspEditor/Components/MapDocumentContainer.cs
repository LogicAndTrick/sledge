using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Controls;
using Sledge.Shell;

namespace Sledge.BspEditor.Components
{
    public class MapDocumentContainer : UserControl
    {
        public int WindowID { get; }
        public TableSplitControl Table { get; }
        public List<CellReference> MapDocumentControls { get; }

        private readonly List<Subscription> _subscriptions;

        public MapDocumentContainer(int windowId)
        {
            WindowID = windowId;
            MapDocumentControls = new List<CellReference>();
            Table = new TableSplitControl
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(Table);

            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe("BspEditor:SplitView:Autosize", () => this.InvokeLater(() => Table.ResetViews())),
                Oy.Subscribe("BspEditor:SplitView:FocusCurrent", () => this.InvokeLater(FocusOnActiveView))
            };
        }

        private void FocusOnActiveView()
        {
            if (Table.IsFocusing()) Table.Unfocus();
            else
            {
                var focused = MapDocumentControls.FirstOrDefault(x => x.Control.IsFocused);
                if (focused != null) Table.FocusOn(focused.Control.Control);
            }
        }

        public void SetControl(IMapDocumentControl control, int column, int row)
        {
            var controlAt = Table.GetControlFromPosition(column, row);
            if (controlAt != null) Table.Controls.Remove(controlAt);

            foreach (var rem in MapDocumentControls.Where(x => x.Row == row && x.Column == column).ToList())
            {
                rem.Dispose();
                MapDocumentControls.Remove(rem);
            }

            MapDocumentControls.Add(new CellReference(control, column, row));
            Table.Controls.Add(control.Control, column, row);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _subscriptions.ForEach(x => x.Dispose());
            base.Dispose(disposing);
        }

        public class CellReference : IDisposable
        {
            public IMapDocumentControl Control { get; }
            public int Column { get; }
            public int Row { get; }

            public CellReference(IMapDocumentControl control, int column, int row)
            {
                Control = control;
                Column = column;
                Row = row;
            }

            public void Dispose()
            {
                Control?.Dispose();
            }
        }
    }
}