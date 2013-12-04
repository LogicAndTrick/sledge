using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Settings.Models.Engines
{
    public class EngineSvencoop : EngineGoldSource
    {
        public static int EngineID { get { return 3; } }
        public override int ID { get { return EngineID; } }


        public EngineSvencoop() : base()
        {
            this.Name = "Sven Co-op";

            this.MapMins = -16384;
            this.MapMaxs = 16384;
        }

    }
}
