using System;
using System.Linq;
using System.Reflection;
using OpenTK;

namespace Sledge.Gui
{
    public static class UIManager
    {
        private static UIPlatform? _platform;

        public static UIPlatform Platform
        {
            get
            {
                if (!_platform.HasValue)
                {
                    Toolkit.Init();
                    if (Configuration.RunningOnWindows) _platform = UIPlatform.Windows;
                    else if (Configuration.RunningOnLinux) _platform = UIPlatform.Linux;
                    else if (Configuration.RunningOnMacOS) _platform = UIPlatform.Mac;
                    else throw new NotSupportedException("This operating system is not supported.");
                }
                return _platform.Value;
            }
        }

        public static IUIManager Manager { get; set; }

        public static void Initialise()
        {
            var implementation = DetectImplementation(Platform);
            Initialise(implementation);
        }

        public static void Initialise(string implementation)
        {
            if (String.IsNullOrEmpty(implementation)) implementation = DetectImplementation(Platform);
            if (implementation == "GTK") implementation = "Gtk";
            var assembly = Assembly.Load("Sledge.Gui." + implementation);
            var manager = assembly.GetTypes().First(x => typeof (IUIManager).IsAssignableFrom(x));
            Manager = (IUIManager) Activator.CreateInstance(manager);
        }

        private static string DetectImplementation(UIPlatform platform)
        {
            switch (platform)
            {
                case UIPlatform.Windows:
                    return "WinForms";
                case UIPlatform.Linux:
                    return "GTK";
                case UIPlatform.Mac:
                    return "GTK";
            }
            throw new NotSupportedException();
        }
    }
}