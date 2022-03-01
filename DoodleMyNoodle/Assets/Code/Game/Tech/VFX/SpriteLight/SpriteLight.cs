using System;
using UnityEngine;
using UnityEngineX;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteLight : MonoBehaviour
{
    [Range(0, 1)]
    [Tooltip("0: Additive white - brightens everything equally. \n1: Multiplicative white - brightens bright colors and ignores dark colors (more realistic).")]
    [SerializeField] private float _saturationMode = 1;
    [Range(0, 3)]
    [SerializeField] private float _intensity = 1;

    private MaterialPropertyBlock _propertyBlock;
    private SpriteRenderer _spriteRenderer;

    public float Intensity
    {
        get => _intensity;
        set
        {
            if (value != _intensity)
            {
                _intensity = value;
                UpdateRenderer();
            }
        }
    }

    public float SaturationMode
    {
        get => _saturationMode;
        set
        {
            if (value != _saturationMode)
            {
                _saturationMode = value;
                UpdateRenderer();
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateRenderer();
    }
#endif

    void OnDidApplyAnimationProperties()
    {
        UpdateRenderer();
    }

    public void UpdateRenderer()
    {
        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();
        
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        // load 
        _spriteRenderer.GetPropertyBlock(_propertyBlock);

        // update
        _propertyBlock.SetFloat("_Intensity", _intensity * Mathf.Lerp(0.334f, 1, _saturationMode));
        _propertyBlock.SetFloat("_Saturation", _saturationMode);

        // send
        _spriteRenderer.SetPropertyBlock(_propertyBlock);
    }
}