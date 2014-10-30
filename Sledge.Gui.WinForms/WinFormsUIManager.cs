using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Dialogs;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Resources;
using Sledge.Gui.WinForms.Shell;

namespace Sledge.Gui.WinForms
{
    [UIImplementation("WinForms", 10, UIPlatform.Windows)]
    public class WinFormsUIManager : IUIManager
    {
        private readonly WinFormsShell _shell;
        private IStringProvider _stringProvider = new PassthroughStringProvider();

        public IShell Shell
        {
            get { return _shell; }
        }

        public IStringProvider StringProvider
        {
            get { return _stringProvider; }
            set { _stringProvider = value ?? new PassthroughStringProvider(); }
        }

        public WinFormsUIManager()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RegisterHandlers();

            _shell = new WinFormsShell();
            CacheImplementations();
        }

        private Dictionary<Type, Type> _implementations;

        private void CacheImplementations()
        {
            _implementations = new Dictionary<Type, Type>();
            foreach (var type in typeof(WinFormsUIManager).Assembly.GetTypes())
            {
                var attrs = type.GetCustomAttributes(typeof (ControlImplementationAttribute), false).OfType<ControlImplementationAttribute>().ToList();
                if (!attrs.Any()) continue;
                var interfaces = type.GetInterfaces();
                var direct = interfaces.Where(x => !interfaces.Any(y => y.GetInterfaces().Contains(x)));
                foreach (var iface in direct)
                {
                    if (!iface.GetCustomAttributes(typeof (ControlInterfaceAttribute), false).Any()) continue;
                    _implementations.Add(iface, type);
                }
            }
        }

        public void Start()
        {
            Application.Run(_shell.Form);
        }

        public IWindow CreateWindow()
        {
            return new WinFormsWindow();
        }

        public T Construct<T>() where T : IControl
        {
            return (T) Activator.CreateInstance(_implementations[typeof (T)]);
        }

        public T ConstructDialog<T>() where T : IDialog
        {
            return (T) Activator.CreateInstance(_implementations[typeof(T)]);
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