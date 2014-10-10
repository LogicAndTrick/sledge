using System;

namespace Sledge.ModelViewer
{
	public partial class Dialog : Gtk.Dialog
	{
		public Dialog ()
		{
			this.Build ();

			var model = new Gtk.TreeStore (typeof(string), typeof(bool), typeof(bool));
			var node1 = model.AppendValues ("Node 1", true, true);
			var node2 = model.AppendValues (node1, "Node 2", false, true);
			var node3 = model.AppendValues (node1, "Node 3", false, true);
			var node4 = model.AppendValues (node3, "Node 4", true, true);

			var col1 = new Gtk.TreeViewColumn ();
			col1.Title = "Text";
			var col2 = new Gtk.TreeViewColumn ();
			col2.Title = "Checkbox";

			var ren2 = new Gtk.CellRendererToggle{};
			ren2.Inconsistent = true;
			col2.PackStart (ren2, true);
			var ren1 = new Gtk.CellRendererText ();
			ren1.Editable = true;
			col1.PackStart (ren1, true);

			treeview1.AppendColumn (col2);
			//col2.AddAttribute (ren2, "active", 1);
			//col2.AddAttribute (ren2, "inconsistent", 2);
			col2.SetCellDataFunc (ren2, new Gtk.TreeCellDataFunc(ToggleRender));
			treeview1.AppendColumn (col1);
			col1.AddAttribute (ren1, "text", 0);

			treeview1.Model = model;
		}

		private void ToggleRender (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			var a = (bool) model.GetValue (iter, 1);
			var i = (bool) model.GetValue (iter, 2);
			(cell as Gtk.CellRendererToggle).Active = a;
			(cell as Gtk.CellRendererToggle).Inconsistent = i;
		}
	}

	public class Test : Gtk.CellRenderer
	{
		protected override void Render (Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
		{
			base.Render (window, widget, background_area, cell_area, expose_area, flags);
		}
	}

}

