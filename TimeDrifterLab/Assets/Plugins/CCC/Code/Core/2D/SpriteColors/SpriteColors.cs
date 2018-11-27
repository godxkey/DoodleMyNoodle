using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SpriteColors : MonoBehaviour
{
    const string FOLDERNAME = "Materials";
    const string MATERIALNAME = "MAT_SpriteColors";
    const string SHADERNAME = "CCC/HSVShift";

    [Range(0, 1), SerializeField]
    float affectRangeMin = 0;
    [Range(0, 1), SerializeField]
    float affectRangeMax = 1;
    [Range(0, 1), SerializeField]
    float hueShift = 0;
    [Range(-1, 1), SerializeField]
    float saturation = 0;
    [Range(-1, 1), SerializeField]
    float value = 0;
    [Range(-1, 1), SerializeField]
    float alpha = 0;

    float AffectRangeMin { get { return affectRangeMin; } set { affectRangeMin = value; isDirty = true; } }
    float AffectRangeMax { get { return affectRangeMax; } set { affectRangeMax = value; isDirty = true; } }
    float HueShift       { get { return hueShift; }       set { hueShift = value;       isDirty = true; } }
    float Saturation     { get { return saturation; }     set { saturation = value;     isDirty = true; } }
    float Value          { get { return value; }          set { this.value = value;     isDirty = true; } }
    float Alpha          { get { return alpha; }          set { alpha = value;          isDirty = true; } }

    MaterialPropertyBlock propertyBlock;
    Renderer rendererComponent;
    bool isDirty = false;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        rendererComponent = GetComponent<Renderer>();

        if (!Application.isPlaying)
        {
            Material material = Resources.Load(FOLDERNAME + "/" + MATERIALNAME) as Material;

#if UNITY_EDITOR
            if (material == null)
            {
                print("null mat");
                Shader shader = Shader.Find(SHADERNAME);
                if (shader == null)
                {
                    Debug.LogError("Besoin du shader CCC/HSVShift");
                    return;
                }
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                if (!AssetDatabase.IsValidFolder("Assets/Resources/" + FOLDERNAME))
                {
                    AssetDatabase.CreateFolder("Assets/Resources", FOLDERNAME);
                }
                Material newMat = new Material(shader);
                AssetDatabase.CreateAsset(newMat, "Assets/Resources/" + FOLDERNAME + "/" + MATERIALNAME + ".mat");
                material = Resources.Load(FOLDERNAME + "/" + MATERIALNAME) as Material;
            }
#endif

            if (GetComponent<SpriteRenderer>() != null)
                GetComponent<SpriteRenderer>().sharedMaterial = material;
        }

        ForceApply();
    }

    void LateUpdate()
    {
        if (isDirty)
        {
            isDirty = false;
            ForceApply();
        }
    }

    void Verify()
    {
        if (affectRangeMax < affectRangeMin)
            affectRangeMax = affectRangeMin;
    }

    public void ForceApply()
    {
        Verify();
        if (rendererComponent == null)
            return;

        SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();

        rendererComponent.GetPropertyBlock(propertyBlock);

        if (sprRenderer.sprite == null)
            return;

        Material mat = sprRenderer.sharedMaterial;
        Texture texture = sprRenderer.sprite.texture;

        propertyBlock.SetTexture("_MainTex", texture);
        propertyBlock.SetFloat("_HSVRangeMin", affectRangeMin);
        propertyBlock.SetFloat("_HSVRangeMax", affectRangeMax);
        propertyBlock.SetVector("_HSVAAdjust", new Vector4(hueShift, saturation, value, alpha));

        rendererComponent.SetPropertyBlock(propertyBlock);
    }
}