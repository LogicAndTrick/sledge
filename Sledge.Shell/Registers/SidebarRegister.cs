using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Shell.Settings;
using Sledge.Shell.Controls;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The sidebar register controls sidebar components, positioning, and visibility
    /// </summary>
    [Export(typeof(IStartupHook))]
    [Export(typeof(ISettingsContainer))]
    public class SidebarRegister : IStartupHook, ISettingsContainer
    {
        // The sidebar register needs direct access to the shell
        [Import] private Forms.Shell _shell;
        [ImportMany] private IEnumerable<Lazy<ISidebarComponent>> _sidebarComponents;

        public async Task OnStartup()
        {
            // Register the exported sidebar components
            foreach (var export in _sidebarComponents)
            {
                var ty = export.Value.GetType();
                var hint = OrderHintAttribute.GetOrderHint(ty);
                Add(export.Value, hint);
                Log.Debug("Sidebar", "Loaded: " + export.Value.GetType().FullName);
            }

            // Subscribe to context changes
            Oy.Subscribe<IContext>("Context:Changed", ContextChanged);
        }

        private List<SidebarComponent> _left;
        private List<SidebarComponent> _right;   

        public SidebarRegister()
        {
            _left = new List<SidebarComponent>();
            _right = new List<SidebarComponent>();
        }

        /// <summary>
        /// Add a sidebar component to the right sidebar
        /// </summary>
        /// <param name="component">The component to add</param>
        /// <param name="orderHint"></param>
        private void Add(ISidebarComponent component, string orderHint)
        {
            var sc = new SidebarComponent(component, orderHint);
            _right.Add(sc);
            _right = _right.OrderBy(x => x.OrderHint).ToList();
            _shell.RightSidebarContainer.Insert(sc.Panel, _right.IndexOf(sc));
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
            yield break;
        }

        public void LoadValues(ISettingsStore store)
        {
            _shell.Invoke((MethodInvoker) delegate
            {
                var controls = _left.Union(_right).ToDictionary(x => x.ID, x => x);
                foreach (var sv in store.GetKeys())
                {
                    if (sv.EndsWith(":Side"))
                    {
                        var key = sv.Substring(0, sv.Length - 5);
                        if (controls.ContainsKey(key))
                        {
                            var con = controls[key];

                            _left.Remove(con);
                            _right.Remove(con);

                            if (store.Get(sv, "Left") == "Right") _right.Add(con);
                            else _left.Add(con);
                        }
                    }
                    if (sv.EndsWith(":Expanded"))
                    {
                        var key = sv.Substring(0, sv.Length - 9);
                        if (controls.ContainsKey(key))
                        {
                            var con = controls[key];
                            con.Panel.Hidden = !store.Get(sv, true);
                        }
                    }
                    if (sv.EndsWith(":Order"))
                    {

                    }
                }
            });
        }

        public void StoreValues(ISettingsStore store)
        {
            for (var i = 0; i < _left.Count; i++)
            {
                var sc = _left[i];
                store.Set($"{sc.ID}:Side", "Left");
                store.Set($"{sc.ID}:Order", i);
                store.Set($"{sc.ID}:Expanded", !sc.Panel.Hidden);
            }
            for (var i = 0; i < _right.Count; i++)
            {
                var sc = _right[i];
                store.Set($"{sc.ID}:Side", "Right");
                store.Set($"{sc.ID}:Order", i);
                store.Set($"{sc.ID}:Expanded", !sc.Panel.Hidden);
            }
        }

        /// <summary>
        /// A container for a sidebar component.
        /// </summary>
        private class SidebarComponent
        {
            public string OrderHint { get; }

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

            public SidebarComponent(ISidebarComponent component, string orderHint)
            {
                OrderHint = orderHint ?? "T";
                Component = component;
                Panel = new SidebarPanel
                {
                    Text = component.Title,
                    Name = component.Title,
                    Dock = DockStyle.Fill,
                    Hidden = false,
                    Visible = false,
                    Tag = this
                };
                Panel.AddControl((Control) component.Control);
            }

            /// <summary>
            /// Update the component visibility based on the current context
            /// </summary>
            /// <param name="context">The current context</param>
            public void ContextChanged(IContext context)
            {
                Panel.Invoke(() => {
                    var iic = Component.IsInContext(context);
                    if (iic != Panel.Visible) Panel.Visible = iic;
                });
            }
        }
    }
}
