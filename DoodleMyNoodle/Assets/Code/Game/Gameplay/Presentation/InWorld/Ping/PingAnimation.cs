using System;
using UnityEngine;
using UnityEngineX;
using DG.Tweening;

public class PingAnimation : GamePresentationBehaviour
{
    [SerializeField] private float _endYPosition = 1;
    [SerializeField] private float _animationDuration = 1;
    [SerializeField] private float _duration = 5;

    protected override void Awake()
    {
        base.Awake();

        transform.DOLocalMoveY(_endYPosition, _animationDuration).SetLoops(-1, LoopType.Yoyo);

        this.DelayedCall(_duration, () => { Destroy(gameObject); });
    }

    protected override void OnGamePresentationUpdate() { }
}