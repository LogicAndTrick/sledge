using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Shell;
using Sledge.Gui.WinForms.Shell;

namespace Sledge.Gui.WinForms
{
    [UIImplementation("WinForms", 10, UIPlatform.Windows)]
    public class WinFormsUIManager : IUIManager
    {
        private readonly WinFormsShell _shell;

        public IShell Shell
        {
            get { return _shell; }
        }

        public WinFormsUIManager()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RegisterHandlers();

            _shell = new WinFormsShell();
        }

        public void Start()
        {
            Application.Run(_shell);
        }

        private static void RegisterHandlers()
        {
            Application.ThreadException += ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            LogException((Exception)args.ExceptionObject);
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
            Debug.WriteLine(msg);
            // todo Logging.Logger.ShowException(new Exception(msg, ex), "Unhandled exception");
        }
    }
}