using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInGameEscape : MonoBehaviour
{
    [SerializeField] SceneInfo _sessionCreationMenu;
    [SerializeField] SceneInfo _sessionSelectionMenu;
    [SerializeField] SceneInfo _onlineRoleChoiceMenu;
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
        OnlineService.SetTargetRole(OnlineRole.None);
        SceneService.Load(_onlineRoleChoiceMenu);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
