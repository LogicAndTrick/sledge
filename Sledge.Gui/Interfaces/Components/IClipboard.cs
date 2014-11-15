using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Components
{
    [ControlInterface]
    public interface IClipboard
    {
        void SetText(string text);
        string GetText();
        bool HasText();
    }
}
