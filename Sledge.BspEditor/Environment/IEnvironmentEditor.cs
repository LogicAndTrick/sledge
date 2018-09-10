using System;
using System.Windows.Forms;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Environment
{
    public interface IEnvironmentEditor : IManualTranslate
    {
        event EventHandler EnvironmentChanged;
        Control Control { get; }
        IEnvironment Environment { get; set; }
    }
}