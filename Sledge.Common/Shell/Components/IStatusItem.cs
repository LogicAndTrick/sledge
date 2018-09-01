using System;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    public interface IStatusItem : IContextAware
    {
        string ID { get; }
        int Width { get; }
        bool HasBorder { get; }
        string Text { get; }

        event EventHandler<string> TextChanged;
    }
}
