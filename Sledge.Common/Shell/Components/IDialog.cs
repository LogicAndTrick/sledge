using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    public interface IDialog : IContextAware
    {
        void SetVisible(IContext context, bool visible);
    }
}