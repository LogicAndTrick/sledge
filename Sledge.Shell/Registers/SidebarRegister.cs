using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Components;
using Sledge.Common.Hooks;
using Sledge.Common.Settings;
using Sledge.Shell.Controls;

namespace Sledge.Shell.Registers
{
    [Export(typeof(IShellStartupHook))]
    [Export(typeof(ISettingsContainer))]
    public class SidebarRegister : IShellStartupHook, ISettingsContainer
    {
        private Forms.Shell _shell;

        public async Task OnStartup(Forms.Shell shell, CompositionContainer container)
        {
            _shell = shell;

            foreach (var export in container.GetExports<ISidebarComponent>())
            {
                Add(export.Value);
            }
        }

        private readonly List<SidebarComponent> _left;
        private readonly List<SidebarComponent> _right;   

        public SidebarRegister()
        {
            _left = new List<SidebarComponent>();
            _right = new List<SidebarComponent>();
        }

        private void Add(ISidebarComponent component)
        {
            var sc = new SidebarComponent(component);
            _right.Add(sc);
            _shell.RightSidebarContainer.Add(sc.Panel);
        }

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

        private class SidebarComponent
        {
            public ISidebarComponent Component { get; private set; }
            public SidebarPanel Panel { get; private set; }

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
        }
    }
}
