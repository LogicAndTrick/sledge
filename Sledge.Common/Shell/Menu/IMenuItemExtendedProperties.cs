using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Menu
{
    public interface IMenuItemExtendedProperties
    {
        bool IsToggle { get; }
        bool GetToggleState(IContext context);
    }
}