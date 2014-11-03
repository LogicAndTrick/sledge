using System.Drawing;

namespace Sledge.EditorNew.UI.Menus
{
    public interface IMenuItem
    {
        string TextKey { get; }
        string Text { get; }
        Image Image { get; }
        bool IsActive { get; }
        bool ShowInMenu { get; }
        bool ShowInToolstrip { get; }
        void Execute();
    }
}