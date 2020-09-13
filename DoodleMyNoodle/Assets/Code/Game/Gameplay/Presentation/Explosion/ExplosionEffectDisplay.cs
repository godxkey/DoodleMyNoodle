using System;
using UnityEngine;
using UnityEngineX;
using DG.Tweening;

public class ExplosionEffectDisplay : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float FadeAnimDuration = 2;

    private void Start()
    {
        SpriteRenderer.DOFade(0, FadeAnimDuration).OnComplete(()=> 
        {
            Destroy(gameObject);
        });
    }
}