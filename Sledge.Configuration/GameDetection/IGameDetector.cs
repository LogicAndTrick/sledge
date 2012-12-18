using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Configuration.GameDetection
{
    public interface IGameDetector
    {
        string Name { get; }

        void Detect();
    }
}
