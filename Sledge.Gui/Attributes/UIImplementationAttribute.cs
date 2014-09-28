using System;

namespace Sledge.Gui.Attributes
{
    public class UIImplementationAttribute : Attribute
    {
        public string Name { get; set; }
        public int DefaultOrder { get; set; }
        public UIPlatform[] SupportedPlatforms { get; set; }

        public UIImplementationAttribute(string name, int defaultOrder, params UIPlatform[] supportedPlatforms)
        {
            Name = name;
            DefaultOrder = defaultOrder;
            SupportedPlatforms = supportedPlatforms;
        }
    }
}