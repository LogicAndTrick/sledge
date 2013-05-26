using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Editor
{
    public enum EditorMediator
    {
        SettingsChanged,
        OpenSettings,

        DocumentOpened,
        DocumentActivated,
        DocumentClosed,

        TextureSelected,

        SelectionTypeChanged,
        SelectionChanged,

        ViewportRightClick,

        FileOpened,
        FileSaved,

        Exit,

        About
    }
}
