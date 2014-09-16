using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Sledge.Editor.Properties;
using Sledge.Gui;
using Sledge.Gui.Gtk;
using Sledge.Gui.Shell;
using Sledge.Gui.WinForms;

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
