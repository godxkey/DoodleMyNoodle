using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngineX;

public class LobbyPlayerInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _characterName;

    public void UpdatePlayerInfoDisplay(NativeString64 playerName, NativeString64 characterName)
    {
        _playerName.text = playerName.ToString();
        _characterName.text = characterName.ToString();
    }
}