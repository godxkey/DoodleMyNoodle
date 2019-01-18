using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public class DocumentationCreater {

    [MenuItem("Tools/Documentation/Create")]
    private static void CreateNewDocument()
    {
        UnityPromptPopup.PromptInfo prompt = new UnityPromptPopup.PromptInfo();
        prompt.title = "Enter the Documentation File name please.";
        prompt.buttonAmount = 1;
        prompt.listOfChoice = new string[] { "OK" };
        prompt.needTextEntered = true;
        UnityPromptPopup.EditorPrompt(prompt, delegate (UnityPromptPopup.PromptInteractionResult result)
        {
            File.Create(Application.dataPath + "/Documentation/" + result.textEntered + ".md");
            //TextAsset newDocument = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Documentation/result.textEntered.md", typeof(TextAsset));
            //AssetDatabase.OpenAsset(newDocument);
        });
    }
}
