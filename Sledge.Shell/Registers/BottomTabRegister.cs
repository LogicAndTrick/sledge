using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;

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

            Initialise();
            // Subscribe to context changes
            Oy.Subscribe<IContext>("Context:Changed", ContextChanged);
        }

        private void Initialise()
        {
            _shell.Invoke((MethodInvoker)delegate
            {
                _shell.BottomTabs.TabPages.Clear();
                foreach (var btc in _components)
                {
                    var page = new TabPage(btc.Title) { Tag = btc, Visible = false };
                    page.Controls.Add((Control) btc.Control);
                    _shell.BottomTabs.TabPages.Add(page);
                }
            });
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
                _shell.BottomTabs.SuspendLayout();
                foreach (var tab in _shell.BottomTabs.TabPages.OfType<TabPage>())
                {
                    var btc = tab.Tag as IBottomTabComponent;
                    if (btc == null) continue;

                    var iic = btc.IsInContext(context);
                    var vis = tab.Visible;

                    if (iic != vis) tab.Visible = iic;
                }
                _shell.BottomTabs.ResumeLayout();
            });
        }
    }
}