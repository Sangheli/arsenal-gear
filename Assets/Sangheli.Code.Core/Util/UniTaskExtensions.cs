using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class UniTaskExtensions
{
    public static async UniTask LogExceptions(this UniTask task)
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
