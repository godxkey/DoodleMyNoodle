using System;
using TMPro;
using UnityEngine;
using UnityEngineX;

public class PromptDisplay : GameSystem<PromptDisplay>
{
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private PromptAnswerDisplay _answerButtonPrefab;
    [SerializeField] private Transform _answersContainer;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void Ask(string question, Action<int> onAnswerSelected, params string[] answers)
    {
        ClearAnswers();

        gameObject.SetActive(true);

        _questionText.text = question;

        for (int i = 0; i < answers.Length; i++)
        {
            string answer = answers[i];
            PromptAnswerDisplay promptAnswerDisplay = Instantiate(_answerButtonPrefab, _answersContainer);

            int choiceIndex = i;
            promptAnswerDisplay.Init(answer, () =>
            {
                onAnswerSelected(choiceIndex);
                gameObject.SetActive(false);
            });
        }
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