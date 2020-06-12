﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using sisbase.Utils;

#nullable enable
namespace sisbase.Interactivity {
    public class EventWaiter<T> : IDisposable where T : AsyncEventArgs {
        private readonly Func<T, bool> pred;
        private readonly TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly CancellationTokenSource token;

        internal Task<T> Task => taskSource.Task;

        internal EventWaiter(Func<T, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            this.token = ConcurrencyUtils.PrepareTimeoutToken(timeout, token);
            this.pred = pred;
        }

        internal bool Offer(T args) {
            if (token.IsCancellationRequested) {
                taskSource.SetCanceled();
                return true;
            }
            if (pred(args))
                return taskSource.TrySetResult(args);
            return false;
        }

        internal static EventWaitHandler<T>? Handler;

        public static AsyncEventHandler<T> Listener {
            get {
                Handler ??= new EventWaitHandler<T>();
                return Handler.Offer;
            }
        }

        public static async Task<T> Wait(Func<T, bool> pred, TimeSpan timeout = default, CancellationToken token = default) {
            if (Handler == null) throw new InvalidOperationException($"The listener for EventWaiter<{typeof(T).Name}> has not been registered");
            var waiter = new EventWaiter<T>(pred, timeout, token);
            Handler.Register(waiter);
            return await waiter.Task;
        }

        public void Dispose() {
            token?.Dispose();
        }
    }

    internal class EventWaitHandler<T> where T : AsyncEventArgs {
        private List<EventWaiter<T>> waiters = new List<EventWaiter<T>>();

        public void Register(EventWaiter<T> waiter) {
            waiters.Add(waiter);
        }

        public async Task Offer(T args) {
            var toRemove = waiters.Where((waiter => !waiter.Offer(args))).ToList();
            waiters = waiters.Except(toRemove).ToList();
            toRemove.ForEach(waiter => waiter.Dispose());
        }
    }
}