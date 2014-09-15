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

    public class DialogImplementationAttribute : Attribute
    {
        public string UIImplementationName { get; set; }
        public string DialogName { get; set; }

        public DialogImplementationAttribute(string uiImplementationName, string dialogName)
        {
            UIImplementationName = uiImplementationName;
            DialogName = dialogName;
        }
    }

    public class SidebarPanelImplementationAttribute : Attribute
    {
        public string UIImplementationName { get; set; }
        public string SidebarPanelName { get; set; }

        public SidebarPanelImplementationAttribute(string uiImplementationName, string sidebarPanelName)
        {
            UIImplementationName = uiImplementationName;
            SidebarPanelName = sidebarPanelName;
        }
    }
}