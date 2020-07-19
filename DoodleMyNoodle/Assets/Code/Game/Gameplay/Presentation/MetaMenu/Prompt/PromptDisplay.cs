using System;
using UnityEngine;
using UnityEngineX;

public class PromptDisplay : GamePresentationSystem<PromptDisplay>
{
    [SerializeField] private GameObject _answerButton;

    public override bool SystemReady => true;

    protected override void OnGamePresentationUpdate() { }

    public void Ask(string question, Action<int> onAnswer, params string[] answers)
    {
        foreach (string answer in answers)
        {

        }
    }
}