using System.Windows.Forms;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Components.Properties
{
    public interface IObjectPropertyEditor
    {
        Control Control { get; }
        bool SupportsType(VariableType type);
    }
}