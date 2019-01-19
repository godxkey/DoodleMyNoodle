using DG.Tweening;
using System;

public abstract class AsyncOperationJoinBase
{
    Action onJoin;
    protected int count = 0;
    bool endSpecified = false;
    public bool IsOver { get; private set; } = false;

    public AsyncOperationJoinBase(Action onJoin)
    {
        this.onJoin = onJoin;
    }
    public void MarkEnd()
    {
        endSpecified = true;
        CheckCompletion();
    }
    protected void CheckCompletion()
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
public partial class AsyncOperationJoin : AsyncOperationJoinBase
{
    public AsyncOperationJoin(Action onJoin) : base(onJoin) { }

    public Action RegisterOperation()
    {
        count++;
        return OnCompleteAnyInit;
    }

    protected virtual void OnCompleteAnyInit()
    {
        count--;
        CheckCompletion();
    }
}

public partial class AsyncOperationJoin<T> : AsyncOperationJoinBase
{
    public AsyncOperationJoin(Action onJoin) : base(onJoin) { }

    public Action<T> RegisterOperation()
    {
        count++;
        return OnCompleteAnyInit;
    }

    protected virtual void OnCompleteAnyInit(T obj)
    {
        count--;
        CheckCompletion();
    }
}