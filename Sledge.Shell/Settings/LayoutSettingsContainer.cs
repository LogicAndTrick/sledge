using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Sledge.Common.Settings;
using System;
using System.Globalization;
using System.Linq;

namespace Sledge.Shell.Settings
{
    /// <summary>
    /// Manages layout settings for the shell in general.
    /// Remembers the window state and dockpanel sizes.
    /// </summary>
    [Export(typeof(ISettingsContainer))]
    public class LayoutSettingsContainer : ISettingsContainer
    {
        [Import] private Forms.Shell _shell;
        
        // Settings provider

        public string Name => "Sledge.Shell.Layout";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("WindowState", "", typeof(FormWindowState));

            foreach (var dp in _shell.GetDockPanels())
            {
                yield return new SettingKey($"DockPanel:{dp.Name}:Hidden", "", typeof(bool));
                yield return new SettingKey($"DockPanel:{dp.Name}:Size", "", typeof(int));
            }
        }

        public void SetValues(IEnumerable<SettingValue> values)
        {
            var vals = values.ToDictionary(x => x.Name, x => x.Value);
            _shell.Invoke((MethodInvoker) delegate
            {
                if (vals.ContainsKey("WindowState"))
                {
                    _shell.WindowState = (FormWindowState) Enum.Parse(typeof(FormWindowState), vals["WindowState"]);
                }

                foreach (var dp in _shell.GetDockPanels())
                {
                    var dph = $"DockPanel:{dp.Name}:Hidden";
                    var dps = $"DockPanel:{dp.Name}:Size";
                    if (vals.ContainsKey(dph)) dp.Hidden = Convert.ToBoolean(vals[dph], CultureInfo.InvariantCulture);
                    if (vals.ContainsKey(dps)) dp.DockDimension = Convert.ToInt32(vals[dps], CultureInfo.InvariantCulture);
                }
            });
        }

        public IEnumerable<SettingValue> GetValues()
        {
            yield return new SettingValue("WindowState", Convert.ToString(_shell.WindowState, CultureInfo.InvariantCulture));
            foreach (var dp in _shell.GetDockPanels())
            {
                yield return new SettingValue($"DockPanel:{dp.Name}:Hidden", Convert.ToString(dp.Hidden, CultureInfo.InvariantCulture));
                yield return new SettingValue($"DockPanel:{dp.Name}:Size", Convert.ToString(dp.DockDimension, CultureInfo.InvariantCulture));
            }
        }
    }
}