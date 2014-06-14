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

        // Layout messages
        CreateNewLayoutWindow,
        OpenLayoutSettings,
        ViewportCreated,

        // Document messages
        DocumentOpened,
        DocumentSaved,
        DocumentDeactivated,
        DocumentActivated,
        DocumentClosed,
        DocumentAllClosed,

        // Document manager messages

        // Editing messages
        TextureSelected,
        ToolSelected,
        ContextualHelpChanged,
        ViewportRightClick,
        WorldspawnProperties,
        ResetSelectedBrushType,
        SetZoomValue,
        IgnoreGroupingChanged,
        SelectMatchingTextures,

        VisgroupToggled,
        VisgroupsChanged,
        VisgroupVisibilityChanged,
        VisgroupShowEditor,
        VisgroupSelect,
        VisgroupShowAll,

        // Action messages

        DocumentTreeStructureChanged,
        DocumentTreeSelectedObjectsChanged,
        DocumentTreeObjectsChanged,
        DocumentTreeSelectedFacesChanged,
        DocumentTreeFacesChanged,

        EntityDataChanged,

        SelectionTypeChanged,
        SelectionChanged,

        HistoryChanged,
        ClipboardChanged,

        CompileStarted,
        CompileFinished,
        CompileFailed,

        // Status bar messages
        MouseCoordinatesChanged,
        SelectionBoxChanged,
        ViewZoomChanged,
        ViewFocused,
        ViewUnfocused,
        DocumentGridSpacingChanged,

        // File system messages
        FileOpened,
        FileSaved,
        FileAutosaved,

        // Message logging
        OutputMessage,

        // Editor messages
        LoadFile,
        UpdateMenu,
        UpdateToolstrip,
        CheckForUpdates,
        OpenWebsite,
        About,
        Exit,

        // Single instance

    }
}
