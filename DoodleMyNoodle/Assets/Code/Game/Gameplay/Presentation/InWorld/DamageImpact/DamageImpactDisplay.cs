using UnityEngine;
using DG.Tweening;

public class DamageImpactDisplay : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float AnimDuration = 2;

    private void Start()
    {
        SpriteRenderer.DOFade(0, AnimDuration);
    }
}