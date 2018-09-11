using System;

namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// An attribute that can attached to a class to indicate that it's allowed to be shown on the toolbar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AllowToolbarAttribute : Attribute
    {
        public bool Allowed { get; set; }

        public AllowToolbarAttribute(bool allowed)
        {
            Allowed = allowed;
        }
    }
}