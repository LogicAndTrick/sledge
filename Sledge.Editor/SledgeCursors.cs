using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Sledge.Editor.Properties;
using System.IO;

namespace Sledge.Editor
{
    public static class SledgeCursors
    {
        static SledgeCursors()
        {
            RotateCursor = new Cursor(new MemoryStream(Resources.Cursor_Rotate));
        }

        public static Cursor RotateCursor;
    }
}
