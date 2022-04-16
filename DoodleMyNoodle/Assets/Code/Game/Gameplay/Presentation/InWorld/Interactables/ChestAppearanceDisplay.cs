using CCC.Fix2D;
using DG.Tweening;
using Unity.Entities;
using UnityEngine;

public class ChestAppearanceDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private float _yDisplacement = 0.1f;
    [SerializeField] private float _animationDuration = 1;
    [SerializeField] private float _scaleChange = 0.25f;

    [SerializeField] private Transform _spriteContainer;
    [SerializeField] private SpriteRenderer _chestRenderer;
    [SerializeField] private SpriteRenderer _bgRenderer;

    private Tween _currentAnimation;
    private bool _appearanceChanged = false;

    public override void PresentationUpdate()
    {
        if (SimWorld.TryGetBufferReadOnly(SimEntity, out DynamicBuffer<InventoryItemReference> items))
        {
            if ((items.Length == 1))
            {
                if (!_appearanceChanged)
                {
                    _appearanceChanged = true;

                    _spriteContainer.localScale = new Vector3(_scaleChange, _scaleChange, 1);

                    InventoryItemReference item = items[0];
                    if (SimWorld.TryGetComponent(item.ItemEntity, out SimAssetId itemIDComponent))
                    {
                        ItemAuth itemAuth = PresentationHelpers.FindItemAuth(itemIDComponent);
                        _chestRenderer.sprite = itemAuth.Icon;
                    }

                    _bgRenderer.gameObject.SetActive(true);

                    _currentAnimation = _spriteContainer.DOMoveY(transform.position.y + _yDisplacement, _animationDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                }
            }
            else
            {
                _appearanceChanged = false;

                _bgRenderer.gameObject.SetActive(false);

                _spriteContainer.localScale = new Vector3(1, 1, 1);

                _currentAnimation?.Kill();
            }
        }
    }
}