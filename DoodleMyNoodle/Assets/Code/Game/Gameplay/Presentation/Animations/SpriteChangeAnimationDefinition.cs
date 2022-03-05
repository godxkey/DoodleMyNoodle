using DG.Tweening;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Animations/Sprite Change Animation")]
public class SpriteChangeAnimationDefinition : AnimationDefinition
{
    [FormerlySerializedAs("NewSprite")]
    [SerializeField] private Sprite _newSprite;
    [SerializeField] private float _duration = 0.5f;

    private Dictionary<int, Sprite> _initialSprites = new Dictionary<int, Sprite>();

    public override void TriggerAnimation(TriggerInput input, ref TriggerOuput ouput)
    {
        // remember the old sprite
        _initialSprites[input.TriggerId] = input.PresentationTarget.SpriteRenderer.sprite;

        // set new sprite
        input.PresentationTarget.SpriteRenderer.sprite = _newSprite;

        ouput.Duration = _duration;
    }

    public override void StopAnimation(StopInput input)
    {
        input.PresentationTarget.SpriteRenderer.sprite = _initialSprites[input.TriggerId];
        _initialSprites.Remove(input.TriggerId);
    }
}