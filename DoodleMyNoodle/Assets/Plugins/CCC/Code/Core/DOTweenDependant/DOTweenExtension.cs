using DG.Tweening;

public static class DOTweenExtension
{
    public static void KillIfActive(this Tween tween)
    {
        if (tween.IsActive())
        {
            tween.Kill();
        }
    }
}
