using UnityEngine;
using DG.Tweening;

public class FloatingObjectDisplay : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float AnimDuration = 2;
    public float DisplacementAnimOffset = 2;
    public bool DestroyOnComplete = true;
    public bool AlsoFadeChildSpriteRenderer = true;
    public float DelayBeforeFade = 0;

    private void Start()
    {
        Sequence animation = DOTween.Sequence();

        animation.Join(transform.DOMoveY(transform.position.y + DisplacementAnimOffset, AnimDuration));
        if (DestroyOnComplete)
        {
            animation.OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        animation.Join(SpriteRenderer.DOFade(0, AnimDuration).SetDelay(DelayBeforeFade));
        if (AlsoFadeChildSpriteRenderer)
        {
            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                animation.Join(renderer.DOFade(0, AnimDuration));
            }
        }
    }
}