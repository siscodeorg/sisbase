using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace sisbase.Utils {
    public static class ConcurrencyUtils {
        /// <summary>
        /// Produce a new CancellationTokenSource that will become cancelled when either the timeout expires or the
        /// passed in token becomes cancelled. This function is intended for use to unify timeout and token based
        /// cancellation APIs. 
        ///
        /// Keep in mind that the returned CancellationTokenSource implements IDisposable, and should therefore be
        /// disposed.
        /// </summary>
        /// <param name="timeout">The timeout to expire after. Never expires if set to null.</param>
        /// <param name="token">The token to link. If not specified, an empty token is created.</param>
        /// <returns>A token source for the merged timeout/token.</returns>
        internal static CancellationTokenSource PrepareTimeoutToken(TimeSpan? timeout = null, CancellationToken token = default) {
            var timeoutSource = new CancellationTokenSource();
            if (timeout != TimeSpan.Zero) {
                timeoutSource.CancelAfter(timeout ?? TimeSpan.MaxValue);
            }
            var linked = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, token);
            linked.Token.Register(() => timeoutSource.Dispose());
            return linked;
        }

        // The following two methods are shamelessly borrowed from kjbartel on StackExchange. 
        // https://stackoverflow.com/a/53393575
        /// <summary>
        /// Stop waiting for this Task when the cancellation token fires. Intended to be used to semi-support
        /// cancellation for non-cancellable Tasks. This does not cancel the detached Task, but does propagate
        /// cancellation upwards from the call. UNSAFE if the detached task owns any resources. Use with care.
        /// Catch OperationCancelledException to clean up the detached task if possible.
        /// </summary>
        /// <param name="task">The task to detach from.</param>
        /// <param name="token">The cancellation token for the task.</param>
        /// <typeparam name="T">The return type of the wrapped and wrapper tasks.</typeparam>
        /// <returns>A wrapper task that will be cancelled according to token</returns>
        internal static async Task<T> DetachOnCancel<T>(this Task<T> task, CancellationToken token) {
            token.ThrowIfCancellationRequested();
            await Task.WhenAny(task, token.WhenCanceled());
            token.ThrowIfCancellationRequested();

            return await task;
        }

        /// <summary>
        /// Return a task that will complete when the cancellation token fires. Used to await cancellation of a token.
        /// Only use this method if you know that the cancellation token will fire, or that you will wait on multiple
        /// tasks, at least one of which will eventually complete.
        /// </summary>
        /// <param name="cancellationToken">The token to produce a Task from.</param>
        /// <returns>A task that will mirror the cancellation token.</returns>
        internal static Task WhenCanceled(this CancellationToken cancellationToken) {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s)!.SetResult(true), tcs);
            return tcs.Task;
        }
    }
}
