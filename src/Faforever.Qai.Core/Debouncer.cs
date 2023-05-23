using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Faforever.Qai.Core
{
    public class Debouncer<TKey> where TKey : notnull
    {
        private readonly Dictionary<TKey, CancellationTokenSource> _dictionary
            = new Dictionary<TKey, CancellationTokenSource>();
        private readonly TimeSpan _delay;

        public Debouncer(TimeSpan delay)
        {
            _delay = delay;
        }

        public async Task<T> Debounce<T>(TKey key, Func<Task<T>> action)
        {
            if (_dictionary.ContainsKey(key))
            {
                // Cancel the previous action related to this key
                _dictionary[key].Cancel();
            }

            // Create a new cancellation token source for this key
            var cts = new CancellationTokenSource();
            _dictionary[key] = cts;

            T? result = default;

            try
            {
                var task = Task.Delay(_delay, cts.Token);
                await task;
                if (!task.IsCanceled)
                    result = await action();
            }
            finally
            {
                _dictionary.Remove(key); // Clean up after completion
            }

            return result;
        }
    }
}
