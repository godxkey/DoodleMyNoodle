using System;
using TMPro;
using UnityEngine;
using UnityEngineX;

public class PromptDisplay : GameSystem<PromptDisplay>
{
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private PromptAnswerDisplay _answerButtonPrefab;
    [SerializeField] private PromptAnswerFieldDisplay _answerFieldPrefab;
    [SerializeField] private Transform _answersContainer;

    private bool _waitingForAnswer = false;
    public bool IsWaitingForAnswer
    {
        get
        {
            return _waitingForAnswer;
        }
        set
        {
            _waitingForAnswer = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void Ask(string question, Action<int> onAnswerSelected, params string[] answers)
    {
        ClearAnswers();

        _waitingForAnswer = true;
        UIStateMachine.Instance.TransitionTo(BlockedGameplayState.StateTypes.BlockedGameplay);

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

                _waitingForAnswer = false;
            });
        }
    }

    public void AskString(string question, Action<string> onAnswerSelected)
    {
        ClearAnswers();

        _waitingForAnswer = true;
        UIStateMachine.Instance.TransitionTo(BlockedGameplayState.StateTypes.BlockedGameplay);

        gameObject.SetActive(true);

        _questionText.text = question;

        PromptAnswerFieldDisplay promptAnswerFieldDisplay = Instantiate(_answerFieldPrefab, _answersContainer);
        promptAnswerFieldDisplay.Init((string answer) =>
        {
            onAnswerSelected(answer);
            gameObject.SetActive(false);

            _waitingForAnswer = false;
        });
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