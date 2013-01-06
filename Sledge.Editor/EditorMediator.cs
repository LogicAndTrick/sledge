using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Editor
{
    public static class EditorMediator
    {
        public const string SettingsChanged = "SettingsChanged";

        public const string SelectionTypeChanged = "SelectionTypeChanged";
        public const string SelectionChanged = "SelectionChanged";

        public const string ViewportRightClick = "ViewportRightClick";

        public const string FileOpened = "FileOpened";
        public const string FileSaved = "FileSaved";
    }
}
