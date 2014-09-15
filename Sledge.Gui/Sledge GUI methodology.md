Sledge GUI Methodology
----------------------

Sledge abstracts away the UI layer to allow for multiple UI implementations for the editor.

Example:

- Windows gets Winforms or GTK
- Linux gets GTK
- OSX gets GTK or MonoMac

Current target is WinForms, with GTK to follow shortly afterwards. In the future, a native OSX UI would be nice too.

To do this, key components are represented as interfaces instead of widgets.
The most important of these are:

- Shell - the bootstrapper window, which contains the following elements:
    - Menu - the menu bar (File, Tools, etc.)
    - Toolbar - the toolbar (icon buttons for key actions)
    - Sidebar - sidebars (left, bottom, right), these may be expandable, resizable, and so on, based on the individual implementation
        - Sidebar panels - panels within sidebars, these may be floatable, dockable, and so on, based on the implementation
    - StatusBar - the status bar at the bottom of the application window
    - TabStrip - the tab strip containing the list of currently open documents
    - TableLayoutPanel - the main control grid. Usually contains viewports, but potentially can contain other things too
    - Viewport - an interactive OpenGL viewport
- Window - an additional tool window containing a single TableLayoutPanel

The following components also add important and required functionality:

- QuickForms - the lazy tool programmer's best friend. Allows quick creation of basic prompt/input forms without needing to design every one individually.
- Clipboard - clipboard access
- Compiler - proxies execution of compile tools. platform-specific instead of UI-specific
- GameScanner - scans the system for installed game configurations. platform-specific instead of UI-specific
- HotkeyProvider - provides hotkey logic for a UI framework

The following general-purpose dialog windows (or platform equivalent) need to be implemented:

- ExceptionDialog - error reporting dialog
- SettingsDialog - settings window
- ObjectPropertiesDialog - object properties
- AboutDialog - about window
- CheckForProblemsDialog
- EntityReportDialog
- MapInformationDialog
- MapTreeDialog
- PasteSpecialDialog
- TextureBrowserDialog / AssetBrowserDialog
- TextureReplaceDialog
- TransformDialog
- CheckForUpdatesDialog
- FileSystemBrowserDialog

And these sidebar panels:

- Tool-specific (select, entity, brush, texture, VM, displacement)
- Visgroup panel
- Contextual help panel
- Texture panel
- Compile output panel

The following components are generic enough to only require one implementation, to be translated to the correct UI components as needed:

- MenuManager - manages the menu bar and the toolbar
- ToolManager - manages the list of available tools and switches them appropriately

For a future version, it would be good to expose a simple XML format that parses into an advanced version of a QuickForm - sort
of a combination of XWT (https://github.com/mono/xwt) and GTK's Glade, except for WinForms and my other target platforms (WPF does
not have a reasonable OpenGL component and so it is not a good target platform)
