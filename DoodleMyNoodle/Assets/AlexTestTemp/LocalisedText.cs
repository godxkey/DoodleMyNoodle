using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalisedText
{
    string editorText;
    string applicationText;

    LocalisedText(string text)
    {
        editorText = text;
    }

    public static implicit operator LocalisedText(string input)
    {
        LocalisedText output = new LocalisedText(input);
        return output;
    }

    public static implicit operator string(LocalisedText input)
    {
#if UNITY_EDITOR
        return input.editorText;
#endif
        return input.applicationText;
    }
}
