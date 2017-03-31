using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Components;
using Sledge.Common.Context;
using Sledge.Common.Hooks;
using Sledge.Common.Logging;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The bottom register controls bottom tabs
    /// </summary>
    [Export(typeof(IStartupHook))]
    public class BottomTabRegister : IStartupHook
    {
        // The bottom tab register needs direct access to the shell
        [Import] private Forms.Shell _shell;
        [ImportMany] private IEnumerable<Lazy<IBottomTabComponent>> _bottomTabComponents;

        public async Task OnStartup()
        {
            // Register the exported sidebar components
            foreach (var export in _bottomTabComponents)
            {
                Log.Debug("Bottom tabs", "Loaded: " + export.Value.GetType().FullName);
                _components.Add(export.Value);
            }

            // Subscribe to context changes
            Oy.Subscribe<IContext>("Context:Changed", ContextChanged);
        }

        private readonly List<IBottomTabComponent> _components;

        public BottomTabRegister()
        {
            _components = new List<IBottomTabComponent>();
        }

        private async Task ContextChanged(IContext context)
        {
            _shell.Invoke((MethodInvoker) delegate
            {
                _shell.BottomTabs.TabPages.Clear();
                foreach (var btc in _components)
                {
                    var c = btc.IsInContext(context);
                    if (!c) continue;
                    var page = new TabPage(btc.Title);
                    page.Controls.Add((Control) btc.Control);
                    _shell.BottomTabs.TabPages.Add(page);
                }
            });
        }
    }
}