using System;
using UnityEngine;
using UnityEngineX;

public class CharacterCreationScreen : GamePresentationBehaviour
{
    public GameObject ScreenContainer;

    protected override void OnGamePresentationUpdate()
    {
        ScreenContainer.SetActive(!SimWorld.HasSingleton<GameStartedTag>());
    }
}