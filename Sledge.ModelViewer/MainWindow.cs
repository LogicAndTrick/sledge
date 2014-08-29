using System;
using System.Threading.Tasks;
using Gdk;
using GLib;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Components.Docking;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.ModelViewer;
using Sledge.UI;
using Color = System.Drawing.Color;
using TabStrip = MonoDevelop.Components.Tabstrip;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Xwt.Application.Initialize (Xwt.ToolkitType.Gtk);

		Build ();
        var file_menu = new Menu();

	    this.WidthRequest = 1200;
	    this.HeightRequest = 800;

	    var newitem = new MenuItem("New");
	    var openitem = new MenuItem("Open");
        file_menu.Append(newitem);
        file_menu.Append(openitem);
        newitem.Show();
        openitem.Show();


	    var mb = new MenuBar();
        mb.ShowAll();
	    var fileMenuItem = new MenuItem("File");
        fileMenuItem.Show();
	    fileMenuItem.Submenu = file_menu;
        mb.Append(fileMenuItem);
        vbox1.PackEnd(mb, false, false, 0);

		DockFrame frame = new DockFrame ();
		//DockContainer con = new DockContainer (frame);
		//DockBar bar = new DockBar (frame, PositionType.Left);

		TabStrip ts = new TabStrip ();
		ts.AddTab (new MonoDevelop.Components.Tab (ts, "Blah Blah"));
		ts.AddTab (new MonoDevelop.Components.Tab (ts, "Blah Blah 2"));
		ts.AddTab (new MonoDevelop.Components.Tab (ts, "Blah Blah 3"));
		ts.AddTab (new MonoDevelop.Components.Tab (ts, "Blah Blah 4"));
		ts.AddTab (new MonoDevelop.Components.Tab (ts, "Blah Blah 5"));

	    ViewportBase gl;
	    gl = new ViewportBase();
        
	    //var lab = new Notebook ();
		//lab.Scrollable = true;
		//lab.Add (new Label{ Text = "Test1" });
		//lab.Add (new Label{ Text = "Test2" });
		//lab.Add (new Label{ Text = "Test3" });
		//lab.Add (new Label{ Text = "Test4" });
		//lab.Add (new Label{ Text = "Test5" });
		//lab.Add (new Label{ Text = "Test6" });
		//var tablabel = lab.GetTabLabel (lab.Children [0]);
		//lab.SetTabLabel (lab.Children [0], new Label{ Text = "This is a test" });
		//lab.SetTabReorderable (lab.Children [0], true);
        //lab.ShowAll ();

		var main = frame.AddItem ("Main");
		main.Behavior = DockItemBehavior.Locked;
		main.Expand = true;
		main.DrawFrame = false;
		main.Label = "Main Window";
        main.Content = gl;

	    gl.ButtonPressEvent += (o, args) =>
	    {
	        int x = 1;
	    };

        gl.Listeners.Add(new SampleListener());

        gl.ShowAll();
        gl.Run();

		var item = frame.AddItem ("Something");
		item.DefaultLocation = "Main/Left";
		item.Label = "Test";
		item.Content = new Label () { Name = "Something", Text = "This is a test" };

		var item2 = frame.AddItem ("SomethingElse");
		item2.DefaultLocation = "Main/Right";
		item2.Label = "Test2";
		item2.Content = new Label () { Name = "SomethingElse", Text = "This is another test" };

		this.vbox1.Add (frame);

        var tab2 = new Sledge.ModelViewer.Components.TabStrip();
        tab2.HeightRequest = 30;
        tab2.AddTab("Tab 1", false, true);
        tab2.AddTab("Tab 2 Tab 2 Tab 2");
        tab2.AddTab("Tab 3");
        tab2.AddTab("Tab 4");
        tab2.AddTab("Tab 5");

		var vb = new VBox ();
	    vb.Homogeneous = false;
        vb.Add(tab2);



		var fr = new Frame ("Blah");
        vbox1.PackStart(vb, false, false, 0);
        vbox1.PackStart(fr, true, true, 0);

		vb.ShowAll ();
		fr.ShowAll ();


		//Add (frame);
		frame.CreateLayout ("Default");
		frame.CurrentLayout = "Default";
		frame.ShowAll ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
