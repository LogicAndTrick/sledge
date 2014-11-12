using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Shell;
using Sledge.Gui.Structures;
using Size = Sledge.Gui.Structures.Size;

namespace Sledge.Gui.Tests
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            UIManager.Initialise("GTK");
            UIManager.Manager.Shell.WindowLoaded += (sender, args) => TestManager.Bootstrap();
            UIManager.Manager.Start();
        }
    }

    public interface ITestCase
    {
        string Name { get; }
        void Setup(IWindow window);
    }

    public class Margins : ITestCase
    {
        public string Name { get { return "Margins"; } }

        public void Setup(IWindow window)
        {
            window.Title = Name;
            // window.PreferredSize = new Size(100, 100);
            window.AutoSize = true;

            var pic = new Bitmap(200, 200);
            using (var g = Graphics.FromImage(pic))
            {
                g.DrawRectangle(Pens.Black, 0, 0, 199, 199);
            }
            pic.Save("D:\\test.png");

            var pb = new PictureBox();
            pb.Image = pic;

            var vbox = new VerticalBox();
            var hbox = new HorizontalBox();
            vbox.Add(hbox);
            hbox.Add(pb);

            vbox.Margin = new Padding(10,10,10,10);

            window.Container.Set(vbox);
        }
    }

    public static class TestManager
    {
        private static List<ITestCase> _testWindows;

        public static void Bootstrap()
        {
            _testWindows = typeof (TestManager).Assembly.GetTypes()
                .Where(x => typeof (ITestCase).IsAssignableFrom(x))
                .Where(x => !x.IsInterface)
                .Select(Activator.CreateInstance)
                .OfType<ITestCase>()
                .ToList();

            var vbox = new VerticalBox();

            foreach (var testWindow in _testWindows)
            {
                var window = testWindow;
                var btn = new Button {Text = window.Name};
                btn.Clicked += (sender, args) =>
                {
                    var win = UIManager.Manager.Construct<IWindow>();
                    window.Setup(win);
                    win.Open();
                };
                vbox.Add(btn);
            }

            UIManager.Manager.Shell.Container.Set(vbox);
        }
    }
}
