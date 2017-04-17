using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Common.Tasks
{
    // Borrowed from: https://github.com/Gentlee/SerialQueue
    // MIT license
    // Modified a bit (a lot really)
    public class TaskQueue
    {
        private readonly object _locker = new object();
        private WeakReference<Task> _lastTask;

        public Task Head
        {
            get
            {
                Task t;
                if (_lastTask != null && _lastTask.TryGetTarget(out t))
                {
                    return t;
                }
                return Task.FromResult(0);
            }
        }

        public Task Enqueue(Task task)
        {
            return Enqueue(() => task);
        }

        public Task Enqueue(Func<Task> action)
        {
            lock (_locker)
            {
                Task lastTask;
                Task resultTask;

                if (_lastTask != null && _lastTask.TryGetTarget(out lastTask))
                {
                    resultTask = lastTask.ContinueWith(async _ => await action());
                }
                else
                {
                    resultTask = action();
                }

                _lastTask = new WeakReference<Task>(resultTask);
                return resultTask;
            }
        }
    }
}
