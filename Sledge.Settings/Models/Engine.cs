using Sledge.Providers;

namespace Sledge.Settings.Models
{
    public class Engine
    {
        public virtual int ID { get { return -1; } }
        public string Name { get; set; }

        public int MapMins { get; set; }
        public int MapMaxs { get; set; }

    }
}
