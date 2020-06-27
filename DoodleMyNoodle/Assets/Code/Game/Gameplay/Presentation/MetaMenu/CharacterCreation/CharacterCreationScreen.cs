using System;
using UnityEngine;
using UnityEngineX;

public class CharacterCreationScreen : GamePresentationBehaviour
{
    public GameObject ScreenContainer;

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.HasSingleton<GameReadyToStart>())
        {
            ScreenContainer.SetActive(false);
        }
    }
}