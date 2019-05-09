using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Generic Utility Class for the Editor to popup a prompt
public class UnityPromptPopup : EditorWindow
{
    /// <summary>
    /// Call this to open a prompt in the Unity Editor
    /// </summary>
    /// <param name="info">You need to pass the info about the prompt. It should fit a certain type</param>
    /// <param name="promptCompleted">When the prompt closes, return an event containing info about the interaction with the prompt</param>
    public static void EditorPrompt(PromptInfo info , Action<PromptInteractionResult> promptCompleted)
    {
        currentInfo = info;
        currentEvent = promptCompleted;

        UnityPromptPopup window = ScriptableObject.CreateInstance<UnityPromptPopup>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 250);
        window.ShowPopup();
    }






    #region DATA

    private static PromptInfo currentInfo;
    private static Action<PromptInteractionResult> currentEvent;

    private static string stringToEdit = "";

    public struct PromptInfo
    {
        public bool needTextEntered;
        public int buttonAmount;
        public string[] listOfChoice;
        public string title;
    }

    public struct PromptInteractionResult
    {
        public int choiceSelected;
        public string textEntered;
        public bool hasExit;
    }

    #endregion

    #region RENDERING

    void OnGUI()
    {
        EditorGUILayout.LabelField(currentInfo.title, EditorStyles.wordWrappedLabel);
        GUILayout.Space(70);

        if (currentInfo.needTextEntered)
        {
            stringToEdit = GUILayout.TextField(stringToEdit, 25);
        }

        for (int i = 0; i < currentInfo.buttonAmount; i++)
        {
            if (GUILayout.Button(currentInfo.listOfChoice[i]))
            {
                OnComplete(i, stringToEdit);
            }
        }
    }

    void OnComplete(int buttonChoice, string textEntered)
    {
        PromptInteractionResult result = new PromptInteractionResult();
        result.choiceSelected = buttonChoice;
        result.hasExit = true;
        result.textEntered = textEntered;
        currentEvent.Invoke(result);
        Close();
    }

    #endregion
}
