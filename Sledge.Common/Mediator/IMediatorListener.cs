using System;

namespace Sledge.Common.Mediator
{
    public interface IMediatorListener
    {
        void Notify(string message, object data);
    }
}