using DG.Tweening;

public partial class AsyncOperationJoin
{
    public TweenCallback RegisterTweenOperation()
    {
        count++;
        return OnCompleteAnyInit;
    }
}

public partial class AsyncOperationJoin<T>
{
    public TweenCallback<T> RegisterTweenOperation()
    {
        count++;
        return OnCompleteAnyInit;
    }
}
