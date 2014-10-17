using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Sledge.Editor.Properties;
using Sledge.Gui;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Shell;
using Sledge.Gui.WinForms;
using Sledge.Gui.WinForms.Controls;
using BindingDirection = Sledge.Gui.Bindings.BindingDirection;
using Button = Sledge.Gui.Controls.Button;
using ComboBox = Sledge.Gui.Controls.ComboBox;
using IContainer = Sledge.Gui.Interfaces.IContainer;
using Label = Sledge.Gui.Controls.Label;
using PictureBox = Sledge.Gui.Controls.PictureBox;
using Size = Sledge.Gui.Interfaces.Size;
using TextBox = Sledge.Gui.Controls.TextBox;
using TreeNode = Sledge.Gui.Interfaces.TreeNode;
using TreeView = Sledge.Gui.Controls.TreeView;

namespace Sledge.Editor
{
    class BindingObject : INotifyPropertyChanged
    {
        private string _value1;
        private object _value2;
        private string _buttonText;
        public ObservableCollection<string> StringItems { get; set; }
        public ObservableCollection<object> ObjectItems { get; set; }
        public ObservableCollection<ComboBoxItem> ComboBoxItems { get; set; }

        public BindingObject()
        {
            StringItems = new ObservableCollection<string>();
            ObjectItems = new ObservableCollection<object>();
            ComboBoxItems = new ObservableCollection<ComboBoxItem>();
        }

        public string Value1
        {
            get { return _value1; }
            set
            {
                if (value == _value1) return;
                _value1 = value;
                OnPropertyChanged("Value1");
            }
        }

        public object Value2
        {
            get { return _value2; }
            set
            {
                if (value == _value2) return;
                _value2 = value;
                OnPropertyChanged("Value2");
            }
        }

        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                if (value == _buttonText) return;
                _buttonText = value;
                OnPropertyChanged("ButtonText");
            }
        }

        public void ButtonClicked(object sender, EventArgs e)
        {
            var r = new Random();
            ButtonText = r.NextDouble().ToString("N");

            StringItems.Add("String: " + r.NextDouble());
            ObjectItems.Add(r.NextDouble());
            ComboBoxItems.Add(new ComboBoxItem{Text = "Item: " + r.NextDouble(), DrawBorder = r.NextDouble() > 0.5});

            Value2 = ComboBoxItems.Last();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class BindingControl : VerticalBox
    {
        public BindingControl()
        {
            var source = new BindingObject();
            source.Value1 = "A";
            source.Value2 = "B";
            source.ButtonText = "Click me!";

            var hbox1 = new HorizontalBox();
            var hbox2 = new HorizontalBox();
            var label1 = new Label();
            var textbox1 = new TextBox();
            var pic = new PictureBox();

            var label2 = new Label();
            var combo2 = new ComboBox();

            label1.Text = "Value 1";
            label2.Text = "Value 2";

            var bmp = new Bitmap(100, 100);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.FillRectangle(System.Drawing.Brushes.Red, 0, 0, 100, 100);
                g.FillRectangle(System.Drawing.Brushes.Blue, 25, 25, 50, 50);
            }
            pic.Image = bmp;

            combo2.Items.Add(new ComboBoxItem { Text = "Item 1" });
            combo2.Items.Add(new ComboBoxItem { Text = "Item 2", DrawBorder = true });
            combo2.Items.Add(new ComboBoxItem { Text = "Item 3", DisplayText = "Item 3\n100x100", Image = bmp });
            combo2.Items.Add((ComboBoxItem) "Item 4");
            combo2.Items.Add(new ComboBoxItem { Text = "Item 5", Value = "B" });
            combo2.Items.Add((ComboBoxItem) "Item 6");
            combo2.Items.Add((ComboBoxItem) "Item 7");
            combo2.Items.Add((ComboBoxItem) "Item 8");
            combo2.SelectedItem = combo2.Items[1];

            hbox1.Add(label1);
            hbox1.Add(textbox1, true);
            this.Add(hbox1);

            hbox2.Add(label2);
            hbox2.Add(combo2, true);
            this.Add(hbox2);

            var b1 = new Button();
            b1.Text = "Set Value 1";
            b1.Clicked += (sender, args) => { source.Value1 = new Random().NextDouble().ToString("N"); };
            this.Add(b1);

            var b2 = new Button();
            this.Add(b2);

            this.Add(pic);

            var bbox = new VerticalBox();
            this.Add(bbox);

            BindingSource = source;
            textbox1.Bind("Text", "Value1");
            combo2.Bind("SelectedValue", "Value2");
            combo2.Bind("Items", "ComboBoxItems");

            b2.Bind("Text", "ButtonText");
            b2.Bind("Clicked", "ButtonClicked");

            bbox.Bind("Children", "ComboBoxItems", meta: new Dictionary<string, object> { { "Control", typeof(ChildItemControl) } });
        }
    }

    public class ChildItemControl : HorizontalBox
    {
        public ChildItemControl()
        {
            var lbl = new Label();
            this.Add(lbl, true);
            lbl.Bind("Text", "Text");
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //*/
            IUIManager man;
            man = new WinFormsUIManager();
            //man = new GtkUIManager();

            UIManager.Manager = man; // todo

            man.Shell.WindowLoaded += (sender, args) =>
            {
                var vbox = new VerticalBox();

                // var button = man.Construct<IButton>();
                var button = new Button();
                button.Text = "Button";
                button.Enabled = true;
                button.Clicked += (o, eventArgs) =>
                {
                    var window = man.CreateWindow();
                    window.Title = "Test Window";
                    window.AutoSize = true;

                    var box = new VerticalBox();
                    var btn = new Button();
                    btn.Text = "Add Button";
                    box.Add(btn);
                    window.Container.Set(box);

                    var col = new Collapsible();
                    col.Set(new Button());
                    box.Add(col);

                    btn.Clicked += (sender1, args1) =>
                    {
                        box.Add(new Button());
                    };

                    window.Open();
                };
                vbox.Add(button);

                var button2 = new Button();
                button2.Text = "This is another button";
                button2.PreferredSize = new Size(50, 100);
                vbox.Add(button2);

                var tc = new Table();
                tc.Insert(0, 0, new Button { Text = "asdf 1" });
                tc.Insert(0, 1, new Button { Text = "asdf 2" });
                tc.Insert(1, 1, new Button { Text = "asdf 3" });
                tc.Insert(1, 2, new Button { Text = "asdf 4" }, columnFill: true, rowFill:true);
                tc.Insert(2, 0, new Button { Text = "asdf 5" }, columnSpan: 2);
                vbox.Add(tc);

                var ts = new TabStrip();
                ts.Tabs.Add(new Tab { Text = "Tab 1", Dirty = true });
                ts.Tabs.Add(new Tab { Text = "Tab 2" });
                ts.Tabs.Add(new Tab { Text = "Tab 3", Closable = false });
                ts.SelectedIndex = 1;
                vbox.Add(ts);

                var table = man.Construct<IResizableTable>();
                table.Insert(0, 0, new Button());
                table.Insert(0, 1, new Button());
                table.Insert(1, 0, new Button());

                //var scroll = man.Construct<IVerticalScrollContainer>();
                //var scrollInner = man.Construct<IVerticalBox>();
                //for (int i = 0; i < 10; i++)
                //{
                //    var b = man.Construct<IButton>();
                //    b.Text = i.ToString(CultureInfo.InvariantCulture);
                //    scrollInner.Add(b);
                //}
                //scroll.Set(scrollInner);
                //table.Insert(1, 1, scroll);

                vbox.Add(table, true);

                for (int j = 0; j < 2; j++)
                {
                    var tempSidebar = new Collapsible();
                    var sideBox = new VerticalBox();
                    for (int i = 0; i < 3; i++)
                    {
                        sideBox.Add(new TextBox());
                    }
                    tempSidebar.Set(sideBox);
                    man.Shell.AddSidebarPanel(tempSidebar, SidebarPanelLocation.Right);
                }

                var sb = new Collapsible();
                var bc = new BindingControl();
                sb.Set(bc);
                man.Shell.AddSidebarPanel(sb, SidebarPanelLocation.Right);

                var tvc = new Collapsible();
                var tv = new TreeView{ShowCheckboxes = true};
                tvc.Set(tv);
                man.Shell.AddSidebarPanel(tvc, SidebarPanelLocation.Right);

                var r1 = new TreeNode { Text = "Root 1", Indeterminate = true };
                tv.Model.AddRootNode(r1);
                tv.Model.AddChildNode(r1, new TreeNode { Text = "Child 1", Checked = true });
                var r2 = new TreeNode { Text = "Root 2", Checked = false };
                tv.Model.AddRootNode(r2);

                man.Shell.Container.Set(vbox);

                // we be loaded
                man.Shell.Title = "Sledge Editor";
                man.Shell.AddMenu();
                man.Shell.AddToolbar();

                var m = man.Shell.Menu.AddMenuItem("File", "File").AddSubMenuItem("New", "New");
                m.Icon = Resources.Menu_New;
                m.Clicked += (o, eventArgs) =>
                {
                    Debug.WriteLine("New");
                };

                var t = man.Shell.Toolbar.AddToolbarItem("Open", "Open");
                t.Icon = Resources.Menu_Open;
                t.Clicked += (o, eventArgs) =>
                {
                    button.Enabled = !button.Enabled;
                    Debug.WriteLine("Open");
                };
            };

            man.Start();
            return;
            //*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RegisterHandlers();
            SingleInstance.Start(typeof(Editor));
        }

        private static void RegisterHandlers()
        {
            Application.ThreadException += ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogException((Exception) args.ExceptionObject);
        }

        private static void ThreadException(object sender, ThreadExceptionEventArgs args)
        {
            LogException(args.Exception);
        }

        private static void LogException(Exception ex)
        {
            var st = new StackTrace();
            var frames = st.GetFrames() ?? new StackFrame[0];
            var msg = "Unhandled exception";
            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
            }
            Logging.Logger.ShowException(new Exception(msg, ex), "Unhandled exception");
        }
    }
}
