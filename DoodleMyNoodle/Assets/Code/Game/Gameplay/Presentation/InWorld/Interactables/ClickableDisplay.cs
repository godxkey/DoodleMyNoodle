using CCC.Fix2D;
using UnityEngine;

public class ClickableDisplay : BindedPresentationEntityComponent, IWorldUIPointerEnterHandler, IWorldUIPointerExitHandler, IWorldUIPointerClickHandler
{
    public enum ClickAction // for now, since our use cases are very simple, we can use this. If they get more complex, we could use child classes
    {
        OpenChest,
        TriggerSignal,
    }

    [SerializeField] private ClickAction _clickAction;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool _highlighted = false;

    protected override void OnGamePresentationUpdate() { }

    void IWorldUIPointerEnterHandler.OnPointerEnter()
    {
        SetHighlighted(true);
    }

    void IWorldUIPointerExitHandler.OnPointerExit()
    {
        SetHighlighted(false);
    }

    void IWorldUIPointerClickHandler.OnPointerClick()
    {
        if (!Cache.LocalPawnAlive)
            return;

        fix2 entityPosition = SimWorld.GetComponentData<FixTranslation>(SimEntity);
        fix2 pawnPosition = Cache.LocalPawnPosition;

        if (fixMath.distancemanhattan(entityPosition, pawnPosition) > SimulationGameConstants.InteractibleMaxDistanceManhattan)
        {
            PresentationHelpers.RequestFloatingText((Vector2)entityPosition, "Trop loin", Color.white);
            return;
        }

        switch (_clickAction)
        {
            case ClickAction.OpenChest:
                if (InteractableInventoryDisplaySystem.Instance != null)
                    InteractableInventoryDisplaySystem.Instance.SetupDisplayForInventory(SimEntity);
                break;

            case ClickAction.TriggerSignal:
                SimWorld.SubmitInput(new SimPlayerInputClickSignalEmitter(entityPosition));
                break;
        }
    }

    public void SetSprite(SpriteRenderer newSpriteRenderer)
    {
        if (newSpriteRenderer == _spriteRenderer)
        {
            return;
        }

        bool wasHighlighted = _highlighted;
        SetHighlighted(false);
        if (wasHighlighted)
            SetHighlighted(true);
        _spriteRenderer = newSpriteRenderer;
    }

    private void SetHighlighted(bool highlighted)
    {
        if (_highlighted == highlighted)
            return;

        _highlighted = highlighted;

        if (highlighted)
        {
            HighlightService.Params args = HighlightService.Params.Default;

            args.Duration = HighlightService.Duration.UntilManuallyStopped;
            args.FlickerSpeed = HighlightService.FlickerSpeed.Slow;
            args.Intensity = HighlightService.Intensity.Normal;
            args.AnimStart = HighlightService.AnimStart.MidHighlight;

            HighlightService.HighlightSprite(_spriteRenderer, args);
        }
        else
        {
            HighlightService.StopHighlight(_spriteRenderer);
        }
    }
}