using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Settings;
using Sledge.Settings.GameDetection;

namespace Sledge.Sandbox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());

            var wd = new WonDetector();
            wd.Detect();

            //return;

            QuickStartBootstrap.MapFile = @"D:\Github\sledge\_Resources\RMF\entities.rmf";
            SettingsManager.Read();
            QuickStartBootstrap.Game = SettingsManager.Games.Single(x => x.ID == 1);
            QuickStartBootstrap.Start();
        }
    }
}
