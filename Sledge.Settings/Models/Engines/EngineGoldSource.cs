using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Settings.Models.Engines
{
    public class EngineGoldSource : Engine
    {
        public static int EngineID { get { return 1; } }
        public override int ID { get { return EngineID; } }

        public EngineGoldSource() : base()
        {
            this.Name = "GoldSource";

            this.MapMins = -4096;
            this.MapMaxs = 4096;
        }
    }
}
