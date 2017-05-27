using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.BspEditor.Environment.Controls;
using Sledge.Common.Shell.Settings;

namespace Sledge.BspEditor.Environment
{
    [Export(typeof(ISettingsContainer))]
    [Export(typeof(ISettingEditorFactory))]
    public class EnvironmentRegister : ISettingsContainer, ISettingEditorFactory
    {
        [ImportMany] private IEnumerable<Lazy<IEnvironmentFactory>> _factories;
        private EnvironmentCollection _environments = new EnvironmentCollection();

        public bool Supports(SettingKey key)
        {
            return key.Type == typeof(EnvironmentCollection);
        }

        public ISettingEditor CreateEditorFor(SettingKey key)
        {
            if (key.Type == typeof(EnvironmentCollection))
            {
                return new EnvironmentCollectionEditor(_factories.Select(x => x.Value));
            }
            return null;
        }

        public string Name => "Sledge.BspEditor.Environment.EnvironmentRegister";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Environments", "Environments", typeof(EnvironmentCollection));
        }

        public void LoadValues(ISettingsStore store)
        {
            if (store.Contains("Environments"))
            {
                _environments = (EnvironmentCollection) store.Get(typeof(EnvironmentCollection), "Environments") ?? new EnvironmentCollection();
            }
        }

        public void StoreValues(ISettingsStore store)
        {
            store.Set("Environments", _environments);
        }
    }
}
