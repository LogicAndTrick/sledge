using System;
using System.Threading.Tasks;

namespace Sledge.Common.Threading
{
    /// <summary>
    /// A task queue that guarantees tasks will be executed serially.
    /// </summary>
    /// <remarks> 
    /// Borrowed from: https://github.com/Gentlee/SerialQueue
    /// MIT license
    /// Modified a bit (a lot really)
    /// </remarks>
    public class TaskQueue
    {
        private readonly object _locker = new object();
        private WeakReference<Task> _lastTask;

        /// <summary>
        /// The current head of the task queue.
        /// </summary>
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

        /// <summary>
        /// Add a task to the end of the queue
        /// </summary>
        /// <param name="task">The task to add</param>
        /// <returns>A continuation for after the task completes</returns>
        public Task Enqueue(Task task)
        {
            return Enqueue(() => task);
        }

        /// <summary>
        /// Add an action to the end of the queue
        /// </summary>
        /// <param name="action">The action to add</param>
        /// <returns>A continuation for after the task completes</returns>
        public Task Enqueue(Func<Task> action)
        {
            lock (_locker)
            {
                Task resultTask;

                if (_lastTask != null && _lastTask.TryGetTarget(out var lastTask))
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
