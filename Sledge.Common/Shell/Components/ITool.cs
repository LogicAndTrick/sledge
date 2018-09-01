using System.Drawing;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    public interface ITool : IContextAware
    {
        Image Icon { get; }
        string Name { get; }
    }
}
