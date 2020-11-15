using System;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class LobbyPlayerInfoDisplay : GamePresentationBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _characterName;

    private Entity _currentPlayer;

    public void UpdatePlayerInfoDisplay(FixedString64 playerName, FixedString64 characterName, Entity pawnReference)
    {
        _playerName.text = playerName.ToString();
        _characterName.text = characterName.ToString();

        _currentPlayer = pawnReference;
    }

    public void OnPlayerInfoClicked()
    {
        if (SimWorld.TryGetComponentData(_currentPlayer, out FixTranslation translation))
        {
            CameraMovementController.Instance.TeleportCameraToPosition(translation.Value.ToUnityVec());
        }
    }

    protected override void OnGamePresentationUpdate() { }
}