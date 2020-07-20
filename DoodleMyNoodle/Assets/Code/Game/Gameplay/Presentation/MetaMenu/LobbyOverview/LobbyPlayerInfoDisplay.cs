using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngineX;

public class LobbyPlayerInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _characterName;

    public void Init(NativeString64 playerName, NativeString64 characterName)
    {
        _playerName.text = "" + playerName;
        _characterName.text = "" + characterName;
    }
}