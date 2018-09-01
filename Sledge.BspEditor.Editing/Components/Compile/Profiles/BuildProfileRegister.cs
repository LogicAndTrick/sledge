using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.Common.Shell.Settings;

namespace Sledge.BspEditor.Editing.Components.Compile.Profiles
{
    [Export]
    [Export(typeof(ISettingsContainer))]
    public class BuildProfileRegister : ISettingsContainer
    {
        private List<BuildProfile> _profiles = new List<BuildProfile>();

        public string Name => "Sledge.BspEditor.Editing.BuildProfileRegister";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield break;
        }

        public void LoadValues(ISettingsStore store)
        {
            if (store.Contains("Profiles"))
            {
                _profiles = store.Get("Profiles", new List<BuildProfile>());
            }
        }

        public void StoreValues(ISettingsStore store)
        {
            store.Set("Profiles", _profiles);
        }

        /// <summary>
        /// Get all the registered profiles.
        /// </summary>
        public List<BuildProfile> GetProfiles()
        {
            return _profiles.ToList();
        }

        /// <summary>
        /// Get all the profiles for a given specification name.
        /// </summary>
        public List<BuildProfile> GetProfiles(string specificationName)
        {
            return _profiles.Where(x => x.SpecificationName == specificationName).ToList();
        }

        /// <summary>
        /// Add a profile to the register. Any profile with the same name will be replaced with this one.
        /// </summary>
        public void Add(BuildProfile profile)
        {
            _profiles.RemoveAll(x => string.Equals(x.Name, profile.Name, StringComparison.InvariantCultureIgnoreCase) && x.SpecificationName == profile.SpecificationName);
            _profiles.Add(profile);
        }

        /// <summary>
        /// Remove a profile from the register.
        /// </summary>
        /// <param name="profile">The profile to remove</param>
        public void Remove(BuildProfile profile)
        {
            _profiles.RemoveAll(x => string.Equals(x.Name, profile.Name, StringComparison.InvariantCultureIgnoreCase) && x.SpecificationName == profile.SpecificationName);
        }
    }
}