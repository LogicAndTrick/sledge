using System;

namespace Sledge.Gui.Attributes
{
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
}