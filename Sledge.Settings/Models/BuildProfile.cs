using System;
using System.Linq;
using Sledge.Providers;

namespace Sledge.Settings.Models
{
    public class BuildProfile
    {
        public int ID { get; set; }
        public int BuildID { get; set; }
        public string Name { get; set; }

        public bool RunCsg { get; set; }
        public bool RunBsp { get; set; }
        public bool RunVis { get; set; }
        public bool RunRad { get; set; }

        public string GeneratedCsgParameters { get; set; }
        public string GeneratedBspParameters { get; set; }
        public string GeneratedVisParameters { get; set; }
        public string GeneratedRadParameters { get; set; }
        public string GeneratedSharedParameters { get; set; }

        public string AdditionalCsgParameters { get; set; }
        public string AdditionalBspParameters { get; set; }
        public string AdditionalVisParameters { get; set; }
        public string AdditionalRadParameters { get; set; }
        public string AdditionalSharedParameters { get; set; }

        public string FullCsgParameters
        {
            get
            {
                return (GeneratedCsgParameters + ' ' +
                        AdditionalCsgParameters + ' ' +
                        GeneratedSharedParameters + ' ' +
                        AdditionalSharedParameters).Trim();
            }
        }

        public string FullBspParameters
        {
            get
            {
                return (GeneratedBspParameters + ' ' +
                        AdditionalBspParameters + ' ' +
                        GeneratedSharedParameters + ' ' +
                        AdditionalSharedParameters).Trim();
            }
        }

        public string FullVisParameters
        {
            get
            {
                return (GeneratedVisParameters + ' ' +
                        AdditionalVisParameters + ' ' +
                        GeneratedSharedParameters + ' ' +
                        AdditionalSharedParameters).Trim();
            }
        }

        public string FullRadParameters
        {
            get
            {
                return (GeneratedRadParameters + ' ' +
                        AdditionalRadParameters + ' ' +
                        GeneratedSharedParameters + ' ' +
                        AdditionalSharedParameters).Trim();
            }
        }

        public void Read(GenericStructure gs)
        {
            foreach (var pi in GetType().GetProperties().Where(x => x.CanWrite))
            {
                var val = gs[pi.Name] ?? "";
                if (pi.PropertyType == typeof(int))
                {
                    int i;
                    pi.SetValue(this, Int32.TryParse(val, out i) ? i : 0, null);
                }
                else if (pi.PropertyType == typeof(bool))
                {
                    bool b;
                    if (!bool.TryParse(val, out b)) b = true;
                    pi.SetValue(this, b, null);
                }
                else
                {
                    pi.SetValue(this, val, null);
                }
            }
        }

        public void Write(GenericStructure gs)
        {
            foreach (var pi in GetType().GetProperties())
            {
                var val = pi.GetValue(this, null);
                gs.AddProperty(pi.Name, val == null ? "" : val.ToString());
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}