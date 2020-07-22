using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInGameEscape : MonoBehaviour
{
    [SerializeField] Button _exitApplicationButton;
    [SerializeField] Button _exitSessionButton;
    [SerializeField] Button _returnButton;

    void Awake()
    {
        Close();
    }

    void Start()
    {
        _returnButton.onClick.AddListener(Close);
        _exitSessionButton.onClick.AddListener(ExitSession);
        _exitApplicationButton.onClick.AddListener(ExitApplication);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ExitSession()
    {
        PromptDisplay.Instance.Ask("Are you sure you want to quit ?", (int choice) =>
        {
            if (choice == 1)
            {
                ((GameStateInGameBase)GameStateManager.currentGameState).ReturnToMenu();
            }
        }, "No", "Yes");
    }

    public void ExitApplication()
    {
        PromptDisplay.Instance.Ask("Are you sure you want to quit ?", (int choice) =>
        {
            if (choice == 1)
            {
                ((GameStateInGameBase)GameStateManager.currentGameState).ExitApplication();
            }
        }, "No", "Yes");
    }
}
