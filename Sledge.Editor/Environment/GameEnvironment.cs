using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sledge.FileSystem;
using Sledge.Settings.Models;

namespace Sledge.Editor.Environment
{
    public class GameEnvironment
    {
        public Game Game { get; private set; }
        private IFile _root;

        public IFile Root
        {
            get
            {
                if (_root == null)
                {
                    var dirs = GetGameDirectories().Where(Directory.Exists).ToList();
                    if (dirs.Any()) _root = new RootFile(Game.Name, dirs.Select(x => new NativeFile(x)));
                    else _root = new VirtualFile(null, "");
                }
                return _root;
            }
        }

        public GameEnvironment(Game game)
        {
            Game = game;
        }

        public IEnumerable<string> GetGameDirectories()
        {
            if (Game.SteamInstall)
            {
                yield return Path.Combine(Sledge.Settings.Steam.SteamDirectory, "steamapps", "common", Game.SteamGameDir, Game.ModDir);
                if (!String.Equals(Game.BaseDir, Game.ModDir, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return Path.Combine(Sledge.Settings.Steam.SteamDirectory, "steamapps", "common", Game.SteamGameDir, Game.BaseDir);
                }
            }
            else
            {
                yield return Path.Combine(Game.WonGameDir, Game.ModDir);
                if (!String.Equals(Game.BaseDir, Game.ModDir, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return Path.Combine(Game.WonGameDir, Game.BaseDir);
                }
            }
        }
    }
}
