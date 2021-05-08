using System;
using UnityEngine;
using UnityEngineX;
using DG.Tweening;

public class ExplosionEffectDisplay : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float FadeAnimDuration = 2;
    public AudioPlayable Sfx;
    public AnimationCurve SfxVolumeCurve;
    public AudioSource AudioSource;

    private void Start()
    {
        Sfx.PlayOn(AudioSource, SfxVolumeCurve.Evaluate(transform.localScale.x));

        SpriteRenderer.DOFade(0, FadeAnimDuration).OnComplete(()=> 
        {
            Destroy(gameObject, 2f);
        });
    }
}