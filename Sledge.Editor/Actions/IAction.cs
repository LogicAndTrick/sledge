using System;
using System.Collections.Generic;
using System.Text;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions
{
    public interface IAction : IDisposable
    {
        void Reverse(Document document);
        void Perform(Document document);
    }
}
