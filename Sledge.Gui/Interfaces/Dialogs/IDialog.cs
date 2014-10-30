using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Dialogs
{
    public interface IDialog : IDisposable
    {
        bool Prompt();
    }

    [ControlInterface]
    public interface IFileOpenDialog : IDialog
    {
        bool Multiple { get; set; }
        string Filter { get; set; }
        string File { get; set; }
        IEnumerable<string> Files { get; }
    }

    [ControlInterface]
    public interface IFileSaveDialog : IDialog
    {
        string Filter { get; set; }
        string File { get; set; }
    }

    [ControlInterface]
    public interface IFolderOpenDialog : IDialog
    {
        string Folder { get; set; }
        IEnumerable<string> Folders { get; }
    }

    [ControlInterface]
    public interface IColourChooserDialog : IDialog
    {
        Color Colour { get; set; }
    }
}
