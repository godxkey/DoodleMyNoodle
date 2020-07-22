using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

// CODE TAKEN FROM PIXEL PERFECT OUTLINE PACKAGE

public enum WorldColor {
    CustomColor, PresetOne, PresetTwo, PresetThree, PresetFour
}

[ExecuteInEditMode]
[AddComponentMenu( "Outline/PixelPerfectOutline" )]
[Serializable]
public class PixelPerfectOutline : MonoBehaviour {
    #region ----------------------------------Public Variables ------------------------------------------
    [HideInInspector] public Color32 color = new Color32( 254, 254, 254, 255 );
    [HideInInspector] public int alphaThreshold = 100;
    [HideInInspector] public int thickness = 2;
    [HideInInspector] public bool inline = false;
    [HideInInspector] public bool includeChildren = false;
    [HideInInspector] public bool recursive = false;
    [HideInInspector] public bool start = false;
    [HideInInspector] public float progress = 0;
    [HideInInspector] public int filterMode = 0;
    [HideInInspector] public bool save = false;
    [HideInInspector] public bool includingCharacter = false;
    [HideInInspector] public bool compressFile = false;
    [HideInInspector] public string savePath = "";
    [HideInInspector] public Texture2D header;
    #endregion

    private Color32[] presets = {
        new Color32(255,255,0,255),
        new Color32(255,0,255,255),
        new Color32(255,0,0,255),
        new Color32(0,255,255,255)
    };

    public WorldColor worldColor;

    private SpriteRenderer myRenderer;
    private Image myImage;

    private void Create() {

        if (worldColor != WorldColor.CustomColor) {
            color = presets[ (int)worldColor - 1 ];
        }

        start = true;
        progress = 0f;
        myRenderer = GetComponent<SpriteRenderer>();
        if (!myRenderer) {
            myImage = GetComponent<Image>();
            if (!myImage) {
                Debug.LogWarning( "Gameobject does not have a SpriteRenderer! Script aborted!" );
                return;
            }
        }
        GameObject outline = new GameObject();

        if (!myImage) {
            outline.AddComponent<SpriteRenderer>();
        } else {
            outline.AddComponent<Image>();
        }
        Texture2D newTexture;
        if (!myImage) {
            Texture2D duplicateTexture = DuplicateTexture( myRenderer.sprite.texture );
            newTexture = new Texture2D( (int)myRenderer.sprite.rect.width, (int)myRenderer.sprite.rect.height );
            Color[] pixels = duplicateTexture.GetPixels( (int)myRenderer.sprite.textureRect.x,
                                                    (int)myRenderer.sprite.textureRect.y,
                                                    (int)myRenderer.sprite.textureRect.width,
                                                    (int)myRenderer.sprite.textureRect.height );

            if (pixels.Length < newTexture.width * newTexture.height) {
                newTexture = DuplicateTexture( myRenderer.sprite.texture );
            } else {
                newTexture.SetPixels( pixels );
                newTexture.Apply();
            }



        } else {
            newTexture = DuplicateTexture( (Texture2D)myImage.mainTexture );
        }
        if (!myImage) {
            Vector3 scale = (transform.parent) ? transform.root.localScale : new Vector3( 1, 1, 1 );
            newTexture = ScaleTexture( newTexture, (int)(newTexture.width * scale.x * transform.localScale.x),
                                                  (int)(newTexture.height * scale.y * transform.localScale.y) );
        } else {
            Vector3 scale = (transform.parent) ? transform.root.localScale : new Vector3( 1, 1, 1 );
            newTexture = ScaleTexture( newTexture, (int)(scale.x * GetComponent<RectTransform>().rect.width),
                                                  (int)(scale.y * GetComponent<RectTransform>().rect.height) );
        }

        int localThickness = (newTexture.width > newTexture.height) ? Mathf.RoundToInt( newTexture.height * thickness * 0.005f ) :
            Mathf.RoundToInt( newTexture.width * thickness * 0.005f );

        if (localThickness < 1) localThickness = 1;

        //newTexture = CreateInline(newTexture,2);
        newTexture = CreateOutline( newTexture, localThickness );

        if (!includingCharacter) {
            newTexture = HideRest( newTexture );
        }
        if (save) {
            try {
                File.WriteAllBytes( Application.dataPath + "/" + savePath + gameObject.name + "_outline.png", newTexture.EncodeToPNG() );
            } catch (Exception e) {
                Debug.LogWarning( e.Message );
            }
        }
        if (includingCharacter)
            newTexture = HideRest( newTexture );

        switch (filterMode) {
            case 0:
                newTexture.filterMode = FilterMode.Bilinear;
                break;
            case 1:
                newTexture.filterMode = FilterMode.Point;
                break;
            case 2:
                newTexture.filterMode = FilterMode.Trilinear;
                break;
        }

        if (compressFile)
            newTexture.Compress( false );


        if (!newTexture.isReadable) {
            Debug.LogWarning( "Texture is not readable! Please activate Read/Write enabled in the Inspector of the Texture!" );
            return;
        }
        if (color.Equals( new Color32( 255, 255, 255, 255 ) )) {
            color = new Color32( 254, 254, 254, 255 );
        }

        for (int i = transform.childCount - 1; i >= 0; i--) {
            if (transform.GetChild( i ).gameObject.name == gameObject.name + "_outline")
                DestroyImmediate( transform.GetChild( i ).gameObject );
        }
        if (!myImage) {
            outline.GetComponent<SpriteRenderer>().sprite = Sprite.Create( newTexture, new Rect( 0, 0, newTexture.width, newTexture.height ), new Vector2( 0.5f, 0.5f ) );
            outline.name = gameObject.name + "_outline";
            outline.transform.SetParent( transform );
            outline.transform.localPosition = new Vector3( 0, 0, -0.001f );
            outline.transform.localRotation = new Quaternion();
            outline.transform.localScale = new Vector3( 1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z );
        } else {
            outline.GetComponent<Image>().sprite = Sprite.Create( newTexture, new Rect( 0, 0, newTexture.width, newTexture.height ), new Vector2( 0.5f, 0.5f ) );
            outline.name = gameObject.name + "_outline";
            outline.transform.SetParent( transform );

            //outline.GetComponent<Image>().rectTransform.pivot = myImage.rectTransform.pivot;
            outline.GetComponent<Image>().rectTransform.rotation = myImage.rectTransform.rotation;
            outline.GetComponent<Image>().rectTransform.anchorMin = myImage.rectTransform.anchorMin;
            outline.GetComponent<Image>().rectTransform.anchorMax = myImage.rectTransform.anchorMax;
            outline.GetComponent<Image>().rectTransform.anchoredPosition = myImage.rectTransform.anchoredPosition;
            outline.GetComponent<Image>().rectTransform.sizeDelta = myImage.rectTransform.rect.size;

            outline.GetComponent<RectTransform>().offsetMin = new Vector2( 0, 0 );
            outline.GetComponent<RectTransform>().offsetMax = new Vector2( 0, 0 );

        }


        start = false;
    }

    public void BeginOutline() {

        Create();
        if (includeChildren) {
            for (int i = 0; i < transform.childCount; i++) {

                if (transform.GetChild( i ).gameObject.tag == "BACKGROUNDOBJECT") {
                    continue;
                }

                if (!transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>() &&
                    !transform.GetChild( i ).gameObject.name.Equals( gameObject.name + "_outline" )) {
                    transform.GetChild( i ).gameObject.AddComponent<PixelPerfectOutline>();
                }
                if (!transform.GetChild( i ).gameObject.name.Equals( gameObject.name + "_outline" )) {
                    transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>().color = color;
                    transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>().alphaThreshold = alphaThreshold;
                    transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>().thickness = thickness;
                    transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>().inline = inline;
                    transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>().includeChildren = recursive;
                    transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>().recursive = recursive;
                    transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>().worldColor = worldColor;
                    transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>().BeginOutline();
                    DestroyImmediate( transform.GetChild( i ).gameObject.GetComponent<PixelPerfectOutline>() );
                }
            }
        }
    }

    #region ------------------------------Helper Functions---------------------------------------------
    private Texture2D AddTexture( Texture2D source, Texture2D addition, int x, int y ) {
        int width = (source.width > addition.width) ? source.width : addition.width;
        int height = (source.height > addition.height) ? source.height : addition.height;
        Texture2D result = new Texture2D( width, height );

        Color32[] additionPixels = addition.GetPixels32();
        Color32[] sourcePixels = source.GetPixels32();
        Color32[] resultPixels = result.GetPixels32();

        for (int h = 0; h < addition.height; h++) {
            for (int w = 0; w < addition.width; w++) {
                resultPixels[ result.width / 2 - addition.width / 2 + x + w + result.width * (result.height / 2 - addition.height / 2 + h + y) ]
                    = additionPixels[ w + h * addition.width ];
            }
        }

        result.SetPixels32( resultPixels );
        result.Apply();

        return result;
    }

    Texture2D DuplicateTexture( Texture2D source ) {
        RenderTexture renderTex = RenderTexture.GetTemporary(
source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear );

        Graphics.Blit( source, renderTex );
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D( source.width, source.height );
        readableText.ReadPixels( new Rect( 0, 0, renderTex.width, renderTex.height ), 0, 0 );
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary( renderTex );
        return readableText;
    }

    private Texture2D DecompressTexture( Texture2D source ) {
        RenderTexture renderTex = RenderTexture.GetTemporary( source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear );
        Graphics.Blit( source, renderTex );
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D( source.width, source.height );
        readableText.ReadPixels( new Rect( 0, 0, renderTex.width, renderTex.height ), 0, 0 );
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary( renderTex );
        return readableText;
    }

    private Texture2D ScaleTexture( Texture2D source, float width, float height ) {
        Texture2D result = new Texture2D( Mathf.RoundToInt( width ), Mathf.RoundToInt( height ), source.format, false );
        float incX = (1.0f / (float)width);
        float incY = (1.0f / (float)height);
        for (int i = 0; i < result.height; ++i) {
            for (int j = 0; j < result.width; ++j) {
                Color newColor = source.GetPixelBilinear( (float)j / (float)result.width, (float)i / (float)result.height );
                result.SetPixel( j, i, newColor );
            }
        }
        result.Apply();
        return result;
    }
    #endregion

    #region ----------------------------------Create Inline -------------------------------------------

    private Texture2D CreateInline( Texture2D source, int thickness ) {
        Color32[] colorArray = source.GetPixels32();
        int column = 0;
        int row = 0;
        int paintedPixels = 0;

        for (int i = 0; i < colorArray.Length; i++) {
            if (i % source.width == 0) {
                column++;
                row = 0;
            }
            if (colorArray[ i ].a >= alphaThreshold && !colorArray[ i ].Equals( color )) {
                if (column - 1 <= thickness || column - 1 >= source.height - thickness) {
                    colorArray[ i ] = color;
                } else {
                    if (paintedPixels < thickness) {
                        paintedPixels++;
                        colorArray[ i ] = color;
                    }
                }
                if (row <= thickness || row >= source.width - (thickness + 1)) {
                    colorArray[ i ] = color;
                }
                if (i > source.width * thickness) {

                    for (int m = 1; m < thickness + 1; m++) {
                        if (colorArray[ i - m * source.width ].a < alphaThreshold) {
                            colorArray[ i ] = color;
                        }
                    }
                }
                if (i < colorArray.Length - source.width * thickness - 1) {
                    for (int n = 1; n < thickness + 1; n++) {
                        if (colorArray[ i + source.width * n ].a < alphaThreshold) {
                            colorArray[ i ] = color;
                        }
                    }
                }
            } else {
                if (paintedPixels > 0) {
                    for (int k = 0; k < thickness; k++) {
                        colorArray[ i - k ] = color;
                    }
                    paintedPixels = 0;
                }
            }
            row++;
        }
        source.SetPixels32( colorArray );
        source.Apply();
        return source;
    }
    #endregion

    private Texture2D HideRest( Texture2D source ) {
        Color32[] colorArray = source.GetPixels32();
        for (int s = 0; s < colorArray.Length; s++) {
            if (!colorArray[ s ].Equals( color )) {
                colorArray[ s ] = new Color32( 0, 0, 0, 0 );
            }
        }
        source.SetPixels32( colorArray );
        source.Apply();
        return source;
    }

    #region ----------------------------------Create Outline ------------------------------------------

    private Texture2D CreateOutline( Texture2D source, int thickness ) {
        Color32[] colorArray = source.GetPixels32();
        int column = 0;
        int row = 0;
        int paintedPixels = 0;

        for (int i = 0; i < colorArray.Length; i++) {

            progress = i / colorArray.Length;

            if (i % source.width == 0) {
                column++;
                row = 0;
            }
            if (colorArray[ i ].a >= alphaThreshold && !colorArray[ i ].Equals( color )) {
                if (column <= thickness + 1 || column >= source.height - thickness) {
                    colorArray[ i ] = color;
                } else {
                    for (int m = 1; m < thickness + 1; m++) {
                        if (paintedPixels < thickness && row + m < source.width) {
                            colorArray[ i - paintedPixels ] = color;
                            paintedPixels++;
                        }
                    }
                }
                if (row <= thickness || row >= source.width - (thickness + 1)) {
                    colorArray[ i ] = color;
                }
                if (i > source.width * thickness) {
                    for (int m = 1; m < thickness + 1; m++) {
                        if (colorArray[ i - m * source.width ].a < alphaThreshold) {
                            colorArray[ i - m * source.width ] = color;
                        }
                    }
                }
                if (i < colorArray.Length - source.width * thickness - 1) {
                    for (int n = 1; n < thickness + 1; n++) {
                        if (colorArray[ i + source.width * n ].a < alphaThreshold) {
                            colorArray[ i + source.width * n ] = color;
                        }
                    }
                }

                if (i < colorArray.Length - source.width * thickness - thickness && i > source.width * thickness + thickness) {
                    for (int n = 1; n < thickness + 1; n++) {
                        if (colorArray[ i + source.width * n - n ].a < alphaThreshold && row > n) {
                            colorArray[ i + source.width * n - n ] = color;
                        } else if (colorArray[ i + source.width * n + n ].a < alphaThreshold && row + n < source.width) {
                            colorArray[ i + source.width * n + n ] = color;
                        }
                    }
                }
            } else {
                if (paintedPixels > 0) {
                    for (int k = 0; k < thickness; k++) {
                        if (row + k < source.width) {
                            colorArray[ i + k ] = color;
                        }
                    }
                    paintedPixels = 0;
                }
            }
            row++;
        }
        source.SetPixels32( colorArray );
        source.Apply();
        return source;
    }
    #endregion
}