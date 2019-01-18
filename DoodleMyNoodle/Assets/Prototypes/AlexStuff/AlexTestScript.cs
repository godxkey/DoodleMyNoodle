using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlexTextScript : MonoBehaviour {

    public LocalisedText text = "";
    Text currentText;

    public void Start()
    {
        string temp = text;


        currentText.text = text;
    }


}
