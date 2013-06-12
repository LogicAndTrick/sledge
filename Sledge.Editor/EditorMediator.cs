using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Editor
{
    public enum EditorMediator
    {
        // Settings messages
        SettingsChanged,
        OpenSettings,

        // Document messages
        DocumentOpened,
        DocumentActivated,
        DocumentClosed,

        // Editing messages
        TextureSelected,

        ToolSelected,

        ViewportRightClick,

        WorldspawnProperties,

        VisgroupToggled,
        VisgroupsChanged,
        VisgroupShowEditor,
        VisgroupSelect,
        VisgroupShowAll,

        // Action messages

        DocumentTreeStructureChanged,
        DocumentTreeObjectsChanged,
        DocumentTreeFacesChanged,

        EntityDataChanged,

        SelectionTypeChanged,
        SelectionChanged,

        // File system messages
        FileOpened,
        FileSaved,

        // Editor messages
        UpdateMenu,
        CheckForUpdates,
        OpenWebsite,
        About,
        Exit
    }
}
