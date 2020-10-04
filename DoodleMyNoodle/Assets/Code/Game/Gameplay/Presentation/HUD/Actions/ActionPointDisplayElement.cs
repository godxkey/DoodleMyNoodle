using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPointDisplayElement : MonoBehaviour
{
    public Image ActionPointImageComponent;

    public Sprite FilledSprite;
    public Sprite EmptySprite;

    public void SetAsAvailable(bool isAvailable)
    {
        ActionPointImageComponent.sprite = isAvailable ? FilledSprite : EmptySprite;
    }
}
