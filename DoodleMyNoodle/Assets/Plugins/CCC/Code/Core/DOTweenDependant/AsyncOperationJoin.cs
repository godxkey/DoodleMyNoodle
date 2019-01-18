using DG.Tweening;

public partial class AsyncOperationJoin
{
    public TweenCallback RegisterTweenOperation()
    {
        count++;
        return OnCompleteAnyOperation;
    }
}
