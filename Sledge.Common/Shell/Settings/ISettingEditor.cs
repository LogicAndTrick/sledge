using System;

namespace Sledge.Common.Shell.Settings
{
    public interface ISettingEditor : IDisposable
    {
        event EventHandler OnValueChanged;

        string Label { get; set; }
        object Value { get; set; }
        object Control { get; }
    }
}