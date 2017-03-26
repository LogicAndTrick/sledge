using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Components;
using Sledge.Common.Context;
using Sledge.Common.Settings;
using Sledge.Shell.Controls;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The sidebar register controls sidebar components, positioning, and visibility
    /// </summary>
    [Export(typeof(IShellStartupHook))]
    [Export(typeof(ISettingsContainer))]
    public class SidebarRegister : IShellStartupHook, ISettingsContainer
    {
        private Forms.Shell _shell;

        public async Task OnStartup(Forms.Shell shell, CompositionContainer container)
        {
            // The sidebar register needs direct access to the shell
            _shell = shell;

            // Register the exported sidebar components
            foreach (var export in container.GetExports<ISidebarComponent>())
            {
                Add(export.Value);
            }

            // Subscribe to context changes
            Oy.Subscribe<IContext>("Context:Changed", ContextChanged);
        }

        private readonly List<SidebarComponent> _left;
        private readonly List<SidebarComponent> _right;   

        public SidebarRegister()
        {
            _left = new List<SidebarComponent>();
            _right = new List<SidebarComponent>();
        }

        /// <summary>
        /// Add a sidebar component to the right sidebar
        /// </summary>
        /// <param name="component">The component to add</param>
        private void Add(ISidebarComponent component)
        {
            var sc = new SidebarComponent(component);
            _right.Add(sc);
            _shell.RightSidebarContainer.Add(sc.Panel);
        }

        private async Task ContextChanged(IContext context)
        {
            foreach (var sc in _left.Union(_right))
            {
                sc.ContextChanged(context);
            }
        }

        // Settings provider
        // The settings provider is what moves the sidebar components between left and right.
        // If no settings exist, they'll sit in the right sidebar by default.

        public string Name => "Sledge.Shell.Sidebar";

        public IEnumerable<SettingKey> GetKeys()
        {
            foreach (var sc in _left.Union(_right))
            {
                yield return new SettingKey($"{sc.ID}:Side", "", typeof(string));
                yield return new SettingKey($"{sc.ID}:Order", "", typeof(string));
                yield return new SettingKey($"{sc.ID}:Expanded", "", typeof(bool));
            }
        }

        public void SetValues(IEnumerable<SettingValue> values)
        {
            _shell.Invoke((MethodInvoker) delegate
            {
                var controls = _left.Union(_right).ToDictionary(x => x.ID, x => x);
                foreach (var sv in values)
                {
                    if (sv.Name.EndsWith(":Side"))
                    {
                        var key = sv.Name.Substring(0, sv.Name.Length - 5);
                        if (controls.ContainsKey(key))
                        {
                            var con = controls[key];

                            _left.Remove(con);
                            _right.Remove(con);

                            if (sv.Value == "Right") _right.Add(con);
                            else _left.Add(con);
                        }
                    }
                    if (sv.Name.EndsWith(":Expanded"))
                    {
                        var key = sv.Name.Substring(0, sv.Name.Length - 9);
                        if (controls.ContainsKey(key))
                        {
                            var con = controls[key];
                            con.Panel.Hidden = sv.Value == "False";
                        }
                    }
                    if (sv.Name.EndsWith(":Order"))
                    {

                    }
                }
            });
        }

        public IEnumerable<SettingValue> GetValues()
        {
            for (var i = 0; i < _left.Count; i++)
            {
                var sc = _left[i];
                yield return new SettingValue($"{sc.ID}:Side", "Left");
                yield return new SettingValue($"{sc.ID}:Order", Convert.ToString(i, CultureInfo.InvariantCulture));
                yield return new SettingValue($"{sc.ID}:Expanded", Convert.ToString(!sc.Panel.Hidden, CultureInfo.InvariantCulture));
            }
            for (var i = 0; i < _right.Count; i++)
            {
                var sc = _right[i];
                yield return new SettingValue($"{sc.ID}:Side", "Right");
                yield return new SettingValue($"{sc.ID}:Order", Convert.ToString(i, CultureInfo.InvariantCulture));
                yield return new SettingValue($"{sc.ID}:Expanded", Convert.ToString(!sc.Panel.Hidden, CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// A container for a sidebar component.
        /// </summary>
        private class SidebarComponent
        {
            /// <summary>
            /// The source component
            /// </summary>
            public ISidebarComponent Component { get; private set; }

            /// <summary>
            /// The container panel
            /// </summary>
            public SidebarPanel Panel { get; private set; }

            /// <summary>
            /// The component ID
            /// </summary>
            public string ID => Component.GetType().FullName;

            public SidebarComponent(ISidebarComponent component)
            {
                Component = component;
                Panel = new SidebarPanel
                {
                    Text = component.Title,
                    Name = component.Title,
                    Dock = DockStyle.Fill,
                    Hidden = false
                };
                Panel.AddControl((Control) component.Control);
            }

            /// <summary>
            /// Update the component visibility based on the current context
            /// </summary>
            /// <param name="context">The current context</param>
            public void ContextChanged(IContext context)
            {
                Panel.Visible = Component.IsInContext(context);
            }
        }
    }
}
