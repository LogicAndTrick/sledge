using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.BspEditor.Environment.Controls;
using Sledge.Common.Shell.Settings;

namespace Sledge.BspEditor.Environment
{
    [Export]
    [Export(typeof(ISettingsContainer))]
    [Export(typeof(ISettingEditorFactory))]
    public class EnvironmentRegister : ISettingsContainer, ISettingEditorFactory
    {
        private readonly IEnumerable<Lazy<IEnvironmentFactory>> _factories;
        private EnvironmentCollection _environments = new EnvironmentCollection();

        [ImportingConstructor]
        public EnvironmentRegister([ImportMany] IEnumerable<Lazy<IEnvironmentFactory>> factories)
        {
            _factories = factories;
        }

        public string OrderHint => "B";

        public IEnumerable<SerialisedEnvironment> GetSerialisedEnvironments()
        {
            return _environments;
        }

        public IEnvironment GetEnvironment(string id)
        {
            var env = _environments.FirstOrDefault(x => x.ID == id);
            if (env == null) return null;

            var fac = _factories.FirstOrDefault(x => x.Value.TypeName == env.Type);
            if (fac == null) return null;

            return fac.Value.Deserialise(env);
        }

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
