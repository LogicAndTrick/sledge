using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System;
using System.Globalization;
using System.Linq;
using Sledge.Common.Shell.Settings;

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
            yield break;
        }

        public void SetValues(ISettingsStore store)
        {
            _shell.Invoke((MethodInvoker) delegate
            {
                _shell.WindowState = store.Get("WindowState", _shell.WindowState);

                foreach (var dp in _shell.GetDockPanels())
                {
                    var dph = $"DockPanel:{dp.Name}:Hidden";
                    var dps = $"DockPanel:{dp.Name}:Size";
                    dp.Hidden = store.Get(dph, dp.Hidden);
                    dp.DockDimension = store.Get(dps, dp.DockDimension);
                }
            });
        }

        public IEnumerable<SettingValue> GetValues()
        {
            yield return new SettingValue("WindowState", Convert.ToString(_shell.WindowState, CultureInfo.InvariantCulture));
            foreach (var dp in _shell.GetDockPanels())
            {
                yield return new SettingValue($"DockPanel:{dp.Name}:Hidden", dp.Hidden);
                yield return new SettingValue($"DockPanel:{dp.Name}:Size", dp.DockDimension);
            }
        }
    }
}