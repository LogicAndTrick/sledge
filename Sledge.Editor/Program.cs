using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Sledge.Editor.Properties;
using Sledge.Gui;
using Sledge.Gui.Controls;
using Sledge.Gui.Gtk;
using Sledge.Gui.Shell;
using Sledge.Gui.WinForms;
using Padding = Sledge.Gui.Controls.Padding;

namespace Sledge.Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IUIManager man;
            man = new WinFormsUIManager();
            //man = new GtkUIManager();

            man.Shell.WindowLoaded += (sender, args) =>
            {
                var vbox = man.Construct<IVerticalBox>();

                var button = man.Construct<IButton>();
                button.Text = "Button";
                button.Enabled = true;
                button.Clicked += (o, eventArgs) =>
                {
                    var window = man.CreateWindow();
                    window.Title = "Test Window";
                    window.AutoSize = true;

                    var box = man.Construct<IVerticalBox>();
                    var btn = man.Construct<IButton>();
                    btn.Text = "Add Button";
                    box.Add(btn);
                    window.Container.Set(box);

                    var col = man.Construct<ICollapsible>();
                    col.Set(man.Construct<IButton>());
                    box.Add(col);

                    btn.Clicked += (sender1, args1) =>
                    {
                        box.Add(man.Construct<IButton>());
                    };

                    window.Open();
                };
                vbox.Add(button);

                var button2 = man.Construct<IButton>();
                button2.Text = "This is another button";
                vbox.Add(button2);

                var table = man.Construct<IResizableTable>();
                table.Insert(0, 0, man.Construct<IButton>());
                table.Insert(0, 1, man.Construct<IButton>());
                table.Insert(1, 0, man.Construct<IButton>());

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
                    var tempSidebar = man.Construct<ICollapsible>();
                    var sideBox = man.Construct<IVerticalBox>();
                    for (int i = 0; i < 3; i++)
                    {
                        sideBox.Add(man.Construct<IButton>());
                    }
                    tempSidebar.Set(sideBox);
                    man.Shell.AddSidebarPanel(tempSidebar, SidebarPanelLocation.Right);
                }

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
