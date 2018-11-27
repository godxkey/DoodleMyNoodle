using DG.Tweening;

public static class SpriteGroupTweenExtension
{
    public static Tweener DOFade(this SpriteGroup value, float to, float duration)
    {
        return DOTween.To(() => value.Alpha, (x) => value.Alpha = x, to, duration);
    }
}
