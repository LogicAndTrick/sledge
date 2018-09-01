using System;

namespace Sledge.Common.Shell.Menu
{
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