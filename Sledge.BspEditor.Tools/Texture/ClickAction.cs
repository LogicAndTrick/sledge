using System;

namespace Sledge.BspEditor.Tools.Texture
{
    [Flags]
    public enum ClickAction
    {
        Lift = 1 << 1,
        Select = 1 << 2,

        Apply = 1 << 3,
        Values = 1 << 4,
        AlignToSample = 1 << 5,
        AlignToView = 1 << 6
    }
}
