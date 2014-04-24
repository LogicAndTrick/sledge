using System;
using Sledge.DataStructures.GameData;

namespace Sledge.Editor.UI.ObjectProperties.SmartEdit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class SmartEditAttribute : Attribute
    {
        public VariableType VariableType { get; set; }

        public SmartEditAttribute(VariableType variableType)
        {
            VariableType = variableType;
        }
    }
}