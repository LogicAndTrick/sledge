using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Database;

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

            QuickStartBootstrap.MapFile = @"D:\Github\sledge\_Resources\RMF\entities.rmf";
            QuickStartBootstrap.Game = Context.DBContext.GetAllGames().Single(x => x.ID == 1);
            QuickStartBootstrap.Start();
        }
    }
}
