namespace Rayer.IPSHost.Extensions;

internal static class TaskExtensions
{
    public static async Task WithTimeout(this Task task, TimeSpan timeoutDelay, string message)
    {
        if (task == await Task.WhenAny(task, Task.Delay(timeoutDelay)))
        {
            task.Wait();
        }
        else
        {
            throw new TimeoutException(message);
        }
    }

    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeoutDelay, string message)
    {
        return task == await Task.WhenAny(task, Task.Delay(timeoutDelay)) ? task.Result : throw new TimeoutException(message);
    }
}