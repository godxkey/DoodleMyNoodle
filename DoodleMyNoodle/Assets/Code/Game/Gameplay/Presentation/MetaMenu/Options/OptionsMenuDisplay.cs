using System;
using UnityEngine;
using UnityEngineX;

public class OptionsMenuDisplay : GamePresentationBehaviour
{
    [SerializeField] private GameObject _optionMenu;

    protected override void OnGamePresentationUpdate() { }

    public void OpenMenu()
    {
        _optionMenu.SetActive(true);
    }

    public void LeaveGame()
    {
        PromptDisplay.Instance.Ask("Voulez vous quittez la partie?", (int choiceIndex) =>
         {
             switch (choiceIndex)
             {
                 case 0:
                     break;
                 case 1:
                     GameStateManager.TransitionToState(QuickStartAssets.instance.rootMenu);
                     break;
                 default:
                     break;
             }
         }, "Non", "Oui");
    }

    public void ReturnToGame()
    {
        _optionMenu.SetActive(false);
    }
}