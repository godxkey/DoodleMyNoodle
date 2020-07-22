using System;
using TMPro;
using UnityEngine;
using UnityEngineX;

public class PromptDisplay : GameSystem<PromptDisplay>
{
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private GameObject _answerButtonPrefab;
    [SerializeField] private Transform _answersContainer;
    [SerializeField] private GameObject _blackScreen;

    public override bool SystemReady => true;

    public void Ask(string question, Action<int> onAnswerSelected, params string[] answers)
    {
        ClearAnswers();

        _questionText.text = question;

        for (int i = 0; i < answers.Length; i++)
        {
            string answer = answers[i];
            PromptAnswerDisplay promptAnswerDisplay = Instantiate(_answerButtonPrefab, _answersContainer).GetComponent<PromptAnswerDisplay>();
            if(promptAnswerDisplay != null)
            {
                int choiceIndex = i;
                promptAnswerDisplay.Init(answer, () => 
                {
                    onAnswerSelected(choiceIndex);
                    _blackScreen?.SetActive(false);
                });
            }
        }

        _blackScreen?.SetActive(true);
    }
    
    private void ClearAnswers()
    {
        PromptAnswerDisplay[] answers = _answersContainer.GetComponentsInChildren<PromptAnswerDisplay>();
        for (int i = 0; i < answers.Length; i++)
        {
            Destroy(answers[i].gameObject);
        }
    }
}