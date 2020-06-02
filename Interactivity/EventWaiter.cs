using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using sisbase.Utils;

namespace sisbase.Interactivity {
    public class EventWaiter<T> {
        private readonly Func<T, bool> pred;
        private readonly TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();
        private readonly CancellationTokenSource token;

        public Task<T> Task => taskSource.Task;

        public EventWaiter(Func<T, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            this.token = ConcurrencyUtils.PrepareTimeoutToken(timeout, token);
            this.pred = pred;
        }

        public bool Offer(T args) {
            if (token.IsCancellationRequested) {
                taskSource.SetCanceled();
                return true;
            }
            if (pred(args))
                return taskSource.TrySetResult(args);
            return false;
        }
    }

    public class EventWaitHandler<T> {
        private List<EventWaiter<T>> waiters = new List<EventWaiter<T>>();

        public void Register(EventWaiter<T> waiter) {
            waiters.Add(waiter);
        }

        public void Offer(T args) {
            waiters = waiters.Where((waiter => !waiter.Offer(args))).ToList();
        }
    }
}