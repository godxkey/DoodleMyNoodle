using UnityEngine;
using System;

[Serializable]
public class RandomTriColoredSprite
{
    [SerializeField] Sprite sprite;
    [SerializeField] RandomHSVColor rColor = new RandomHSVColor(Color.red, Color.red);
    [SerializeField] RandomHSVColor gColor = new RandomHSVColor(Color.green, Color.green);
    [SerializeField] RandomHSVColor bColor = new RandomHSVColor(Color.blue, Color.blue);

    Sprite Sprite         { get { return sprite; } set { sprite = value; } }
    RandomHSVColor RColor { get { return rColor; } set { rColor = value; } }
    RandomHSVColor GColor { get { return gColor; } set { gColor = value; } }
    RandomHSVColor BColor { get { return bColor; } set { bColor = value; } }

    public TriColoredSprite GetRandomTriColoredSprite()
    {
        return new TriColoredSprite(sprite,
            rColor.GetRandomColor(),
            gColor.GetRandomColor(),
            bColor.GetRandomColor());
    }
}
