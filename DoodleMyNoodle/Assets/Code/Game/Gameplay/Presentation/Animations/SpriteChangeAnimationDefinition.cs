using DG.Tweening;
using System.Collections;
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
    [SerializeField] private bool _loop = false;

    private Dictionary<int, Sprite> _initialSprites = new Dictionary<int, Sprite>();

    public override void TriggerAnimation(TriggerInput input, ref TriggerOuput ouput)
    {
        // remember the old sprite
        _initialSprites[input.TriggerId] = input.PresentationTarget.SpriteRenderer.sprite;

        ouput.Duration = _loop ? -1 : _duration;

        input.PresentationTarget.SpriteRenderer.sprite = _newSprite;

        if (_loop)
        {
            input.PresentationTarget.Root.GetComponent<MonoBehaviour>().StartCoroutine(SpriteLoop(input));
        }
    }

    IEnumerator SpriteLoop(TriggerInput input)
    {
        while (true)
        {
            yield return new WaitForSeconds(_duration);

            input.PresentationTarget.SpriteRenderer.sprite = _initialSprites[input.TriggerId];

            yield return new WaitForSeconds(_duration);

            input.PresentationTarget.SpriteRenderer.sprite = _newSprite;
        }
    }

    public override void StopAnimation(StopInput input)
    {
        input.PresentationTarget.Root.GetComponent<MonoBehaviour>().StopAllCoroutines();
        input.PresentationTarget.SpriteRenderer.sprite = _initialSprites[input.TriggerId];
        _initialSprites.Remove(input.TriggerId);
    }
}