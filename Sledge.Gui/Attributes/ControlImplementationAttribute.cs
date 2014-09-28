using System;

namespace Sledge.Gui.Attributes
{
    public class ControlImplementationAttribute : Attribute
    {
        public string UIImplementationName { get; set; }

        public ControlImplementationAttribute(string uiImplementationName)
        {
            UIImplementationName = uiImplementationName;
        }
    }
}