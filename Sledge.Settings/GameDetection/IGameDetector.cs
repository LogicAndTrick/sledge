using System.Collections.Generic;
using Sledge.Settings.Models;

namespace Sledge.Settings.GameDetection
{
    public interface IGameDetector
    {
        string Name { get; }

        IEnumerable<Game> Detect();
    }
}
