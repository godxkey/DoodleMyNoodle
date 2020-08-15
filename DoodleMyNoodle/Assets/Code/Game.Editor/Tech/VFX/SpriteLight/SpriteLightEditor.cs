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
        Add,
        Multiply
    }

    const string ASSET_PATH_MULT = "Assets/Materials/Generated/MAT_SpriteLight_Mult.mat";
    const string ASSET_PATH_ADD = "Assets/Materials/Generated/MAT_SpriteLight_Add.mat";
    const string SHADER_NAME_MULT = "CCC/Sprite-Light-Multiply";
    const string SHADER_NAME_ADD = "CCC/Sprite-Light-Add";

    private Shader _shaderMult;
    private Shader _shaderAdd;


    private void OnEnable()
    {
        _shaderMult = Shader.Find(SHADER_NAME_MULT);
        _shaderAdd = Shader.Find(SHADER_NAME_ADD);

        if (_shaderMult == null)
            throw new Exception($"Shader {SHADER_NAME_MULT} could not be found.");

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

        // Multiply not supported in 2D renderer yet :(
        //var newBlendMode = (BlendMode)EditorGUILayout.EnumPopup("Mode", currentblendMode.Value); 
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
            case BlendMode.Multiply:
                shader = _shaderMult;
                assetPath = ASSET_PATH_MULT;
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
        else if (spriteRenderer.sharedMaterial?.shader == _shaderMult)
            return BlendMode.Multiply;

        return null;
    }
}