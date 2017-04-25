using System;

namespace Sledge.Common.Shell.Components
{
    public class SidebarComponentAttribute : Attribute
    {
        public string OrderHint { get; set; }
    }
}