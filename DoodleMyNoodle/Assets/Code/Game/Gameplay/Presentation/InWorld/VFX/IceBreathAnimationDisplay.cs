using System;
using UnityEngine;
using UnityEngineX;
using DG.Tweening;

public class IceBreathAnimationDisplay : GamePresentationBehaviour
{
    public float MoveToDestinationX = 1;
    public float AnimationDuration = 1;
    public GameObject SpriteMaskToMove;

    protected override void OnGamePresentationUpdate() { }

    private void Start()
    {
        if (SpriteMaskToMove != null)
        {
            SpriteMaskToMove.SetActive(true);
            SpriteMaskToMove.transform.DOLocalMoveX(MoveToDestinationX, AnimationDuration);
        }
    }
}