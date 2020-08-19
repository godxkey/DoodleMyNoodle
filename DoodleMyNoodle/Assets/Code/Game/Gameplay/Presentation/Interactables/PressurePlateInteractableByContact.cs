using System;
using UnityEngine;
using UnityEngineX;

public class PressurePlateInteractableByContact : ObjectInteractableOnContact
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _activatedSprite;
    [SerializeField] private Sprite _deactivatedSprite;

    private bool _changingToDeactivated = false;

    protected override void OnInteractionTriggeredByInput()
    {
        _spriteRenderer.sprite = _activatedSprite;
    }

    protected override void OnInteractionReset()
    {
        // Dont change the sprite if it has been reactivated again
        if (!_changingToDeactivated)
        {
            _changingToDeactivated = true;

            this.DelayedCall(0.25f, () =>
            {
                if (!_previousInteractedState)
                {
                    _changingToDeactivated = false;
                    _spriteRenderer.sprite = _deactivatedSprite;
                }
            });
        }
    }
}