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

            QuickStartBootstrap.MapFile = @"K:\Half-Life\Sledge\cliptest.rmf";
            QuickStartBootstrap.Game = Context.DBContext.GetAllGames().Single(x => x.ID == 1);
            QuickStartBootstrap.Game.Wads = Context.DBContext.GetAllWads().Where(x => x.GameID == 1).ToList();
            QuickStartBootstrap.Game.Fgds = Context.DBContext.GetAllFgds().Where(x => x.GameID == 1).ToList();
            QuickStartBootstrap.Start();
        }
    }
}
