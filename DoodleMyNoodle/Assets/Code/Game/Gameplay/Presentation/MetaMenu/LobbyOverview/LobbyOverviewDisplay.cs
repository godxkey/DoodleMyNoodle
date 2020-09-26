using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;

public class LobbyOverviewDisplay : GamePresentationBehaviour
{
    [SerializeField] private GameObject _playerInfoPrefab;
    [SerializeField] private Transform _playerInfoContainer;

    [SerializeField] private TextMeshProUGUI _serverName;

    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private Vector3 _endPosition;
    [SerializeField] private float _animationDuration = 2;

    private List<LobbyPlayerInfoDisplay> _playerInfos = new List<LobbyPlayerInfoDisplay>();

    private bool _isVisible = false;
    private bool _doingAnimation = false;

    protected override void OnGamePresentationUpdate()
    {
        bool shouldBeVisible = Input.GetKey(KeyCode.Tab) && !GameConsole.IsOpen();

        if (shouldBeVisible != _isVisible)
        {
            ToggleVisibility();
        }

        if (OnlineService.OnlineInterface?.SessionInterface != null)
        {
            _serverName.text = OnlineService.OnlineInterface.SessionInterface.HostName;
        }
        else
        {
            _serverName.text = "Local Game";
        }

        int currentPlayerIndex = 0;

        SimWorld.Entities
            .WithAll<Controllable>()
            .ForEach((Entity pawn) =>
            {
                Entity pawnController = CommonReads.GetPawnController(SimWorld, pawn);

                if (pawnController != Entity.Null && !SimWorld.HasComponent<AITag>(pawnController))
                {
                    while (_playerInfos.Count <= currentPlayerIndex)
                    {
                        _playerInfos.Add(Instantiate(_playerInfoPrefab, _playerInfoContainer).GetComponent<LobbyPlayerInfoDisplay>());
                    }

                    NativeString64 playerName = "N/A";
                    NativeString64 characterName = "N/A";

                    if (SimWorld.TryGetComponentData(pawnController, out Name playerNameComponent))
                    {
                        playerName = playerNameComponent.Value;
                    }

                    if (SimWorld.TryGetComponentData(pawn, out Name characterNameComponent))
                    {
                        characterName = characterNameComponent.Value;
                    }

                    _playerInfos[currentPlayerIndex].UpdatePlayerInfoDisplay(playerName, characterName);

                    currentPlayerIndex++;
                }
            });
    }

    private void ToggleVisibility()
    {
        if (_doingAnimation)
        {
            return;
        }

        _doingAnimation = true;

        if (_isVisible)
        {
            GetComponent<RectTransform>().DOAnchorPos(_startPosition, _animationDuration).OnComplete(()=> 
            { 
                _isVisible = false;
                _doingAnimation = false;
            });
        }
        else
        {
            GetComponent<RectTransform>().DOAnchorPos(_endPosition, _animationDuration).OnComplete(() => 
            { 
                _isVisible = true;
                _doingAnimation = false;
            });
        }
    }
}