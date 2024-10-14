using System;
using System.Threading;
using System.Threading.Tasks;
public static class TaskExtensions
{
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        // Create a TaskCompletionSource that completes when the cancellation token is canceled
        var tcs = new TaskCompletionSource<bool>();

        using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
        {
            // Wait for either the original task or the cancellation token to trigger
            if (task != await Task.WhenAny(task, tcs.Task))
            {
                // The cancellation token was triggered
                throw new OperationCanceledException(cancellationToken);
            }
        }

        // Await the original task to get its result
        return await task;
    }
}