using DG.Tweening;
using System;

public partial class AsyncOperationJoin
{
    Action onJoin;
    int count = 0;
    bool endSpecified = false;
    public bool IsOver { get; private set; } = false;

    public AsyncOperationJoin(Action onJoin)
    {
        this.onJoin = onJoin;
    }
    public Action RegisterOperation()
    {
        count++;
        return OnCompleteAnyOperation;
    }
    public void MarkEnd()
    {
        endSpecified = true;
        CheckCompletion();
    }

    void OnCompleteAnyOperation()
    {
        count--;
        CheckCompletion();
    }
    void CheckCompletion()
    {
        if (count <= 0 && endSpecified)
            Complete();
    }
    void Complete()
    {
        IsOver = true;
        if (onJoin != null)
        {
            onJoin();
            onJoin = null;
        }
    }
}