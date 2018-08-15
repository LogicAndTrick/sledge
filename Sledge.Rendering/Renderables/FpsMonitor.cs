using System;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Renderables
{
    public class FpsMonitor : IUpdateable
    {
        private long _lastSecond = -1;
        private long _framesSinceLastSecond;
        private float _averageFps = -1;
        private int _nextReport = 5;

        public void Update(long frame)
        {
            _framesSinceLastSecond++;
            var diff = frame - _lastSecond;
            if (_lastSecond < 0 || diff > 1000)
            {
                _averageFps = (_averageFps * 0.5f + _framesSinceLastSecond / (float) diff * 1000 * 1.5f) / 2;
                _lastSecond = frame;
                _framesSinceLastSecond = 0;
                if (--_nextReport == 0)
                {
                    Console.WriteLine($"FPS: {_averageFps:F}");
                    _nextReport = 5;
                }
            }
        }
    }
}