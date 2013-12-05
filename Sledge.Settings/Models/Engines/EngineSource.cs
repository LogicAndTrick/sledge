using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Settings.Models.Engines
{
    public class EngineSource : Engine
    {
        public static int EngineID { get { return 2; } }
        public override int ID { get { return EngineID; } }


        public EngineSource() : base()
        {
            this.Name = "Source";

            this.MapMins = -16384;
            this.MapMaxs = 16384;
        }
    }
}
