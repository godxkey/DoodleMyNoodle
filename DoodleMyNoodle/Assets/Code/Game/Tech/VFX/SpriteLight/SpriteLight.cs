using System;
using UnityEngine;
using UnityEngineX;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class SpriteLight : MonoBehaviour
{
    [Range(0, 1)]
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateRenderer();
    }
#endif

    public void UpdateRenderer()
    {
        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();
        
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        // load 
        _spriteRenderer.GetPropertyBlock(_propertyBlock);

        // update
        _propertyBlock.SetFloat("_Intensity", _intensity);

        // send
        _spriteRenderer.SetPropertyBlock(_propertyBlock);
    }
}