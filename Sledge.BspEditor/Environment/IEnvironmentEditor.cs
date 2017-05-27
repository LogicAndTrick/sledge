using System;
using System.Windows.Forms;

namespace Sledge.BspEditor.Environment
{
    public interface IEnvironmentEditor
    {
        event EventHandler EnvironmentChanged;
        Control Control { get; }
        IEnvironment Environment { get; set; }
    }
}