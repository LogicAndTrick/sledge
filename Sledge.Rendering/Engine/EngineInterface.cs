using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Rendering.Engine
{
    [Export]
    public class EngineInterface
    {
        public Renderables.Buffer CreateBuffer()
        {
            return new Renderables.Buffer(Engine.Instance.Device);
        }
    }
}
