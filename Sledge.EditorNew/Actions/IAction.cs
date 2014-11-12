using System;
using Sledge.EditorNew.Documents;

namespace Sledge.EditorNew.Actions
{
    public interface IAction : IDisposable
    {
        bool SkipInStack { get; }
        bool ModifiesState { get; }
        void Reverse(Document document);
        void Perform(Document document);
    }
}
