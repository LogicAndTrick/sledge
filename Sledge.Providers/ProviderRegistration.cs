using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using Sledge.Common.Shell.Hooks;

namespace Sledge.Providers
{
    [Export(typeof(IStartupHook))]
    public class ProviderRegistration : IStartupHook
    {
        public async Task OnStartup()
        {
            Gimme.Register(new Texture.Wad.WadTextureItemProvider());
            Gimme.Register(new Texture.Wad.WadTexturePackageProvider());
            Gimme.Register(new Texture.Wad.WadTextureStreamSourceProvider());

            Gimme.Register(new GameData.FgdGameDataProvider());
        }
    }
}
