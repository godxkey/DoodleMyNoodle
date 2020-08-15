using System;
using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

[CustomEditor(typeof(SpriteLight))]
public class SpriteLightEditor : Editor
{
    public enum BlendMode
    {
        Add
    }

    const string ASSET_PATH_ADD = "Assets/Materials/Generated/MAT_SpriteLight.mat";
    const string SHADER_NAME_ADD = "CCC/Sprite-Light";

    private Shader _shaderAdd;


    private void OnEnable()
    {
        _shaderAdd = Shader.Find(SHADER_NAME_ADD);

        if (_shaderAdd == null)
            throw new Exception($"Shader {SHADER_NAME_ADD} could not be found.");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BlendMode? currentblendMode = GetCurrentBlendMode(); // uneditable at runtime for now ...

        if (currentblendMode == null)
        {
            SetBlendMode(BlendMode.Add);
            currentblendMode = BlendMode.Add;
        }

        var newBlendMode = BlendMode.Add;

        if (newBlendMode != currentblendMode)
        {
            SetBlendMode(newBlendMode);
        }
    }

    void SetBlendMode(BlendMode blendMode)
    {
        Shader shader;
        string assetPath;

        switch (blendMode)
        {
            default:
            case BlendMode.Add:
                shader = _shaderAdd;
                assetPath = ASSET_PATH_ADD;
                break;
        }

        Material material = AssetDatabaseX.LoadOrCreateAsset(assetPath, () =>
        {
            var newMat = new Material(shader);
            newMat.enableInstancing = true;
            return newMat;
        });

        ((SpriteLight)target).GetComponent<SpriteRenderer>().sharedMaterial = material;
    }

    BlendMode? GetCurrentBlendMode()
    {
        SpriteRenderer spriteRenderer = ((SpriteLight)target).GetComponent<SpriteRenderer>();

        if (spriteRenderer.sharedMaterial?.shader == _shaderAdd)
            return BlendMode.Add;

        return null;
    }
}