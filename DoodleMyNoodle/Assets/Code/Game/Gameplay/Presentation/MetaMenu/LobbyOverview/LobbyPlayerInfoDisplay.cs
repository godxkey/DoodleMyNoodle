using System;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;
using CCC.Fix2D;

public class LobbyPlayerInfoDisplay : GamePresentationBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _characterName;

    private Entity _currentPlayer;

    public void UpdatePlayerInfoDisplay(FixedString64Bytes playerName, FixedString64Bytes characterName, Entity pawnReference)
    {
        _playerName.text = playerName.ToString();
        _characterName.text = characterName.ToString();

        _currentPlayer = pawnReference;
    }
}