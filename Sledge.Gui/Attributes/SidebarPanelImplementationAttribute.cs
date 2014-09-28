using System;

namespace Sledge.Gui.Attributes
{
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