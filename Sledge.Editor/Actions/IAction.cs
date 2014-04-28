using System;
using System.Collections.Generic;
using System.Text;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions
{
    public interface IAction : IDisposable
    {
        bool SkipInStack { get; }
        bool ModifiesState { get; }
        void Reverse(Document document);
        void Perform(Document document);
    }
}
