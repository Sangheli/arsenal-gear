using System;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskExtensions
{
    public static async Task LogExceptions(this Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
