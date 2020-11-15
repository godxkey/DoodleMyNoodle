using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;

public class LobbyOverviewDisplay : GamePresentationSystem<LobbyOverviewDisplay>
{
    [SerializeField] private LobbyPlayerInfoDisplay _playerInfoPrefab;
    [SerializeField] private Transform _playerInfoContainer;
    [SerializeField] private RectTransform _container;

    [SerializeField] private TextMeshProUGUI _serverName;

    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private Vector3 _endPosition;
    [SerializeField] private float _animationDuration = 2;

    private List<LobbyPlayerInfoDisplay> _playerInfos = new List<LobbyPlayerInfoDisplay>();
    private List<(Entity player, Entity pawn)> _data = new List<(Entity player, Entity pawn)>();

    private bool _isVisible = false;
    private Tweener _tween;

    protected override void Awake()
    {
        base.Awake();

        _playerInfoContainer.GetComponentsInChildren(_playerInfos);
        _container.gameObject.SetActive(false);
        SetVisible(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _tween?.Kill();
    }

    protected override void OnGamePresentationUpdate()
    {
        bool shouldBeVisible = Input.GetKey(KeyCode.Tab) && !GameConsole.IsOpen();

        if (shouldBeVisible != _isVisible)
        {
            SetVisible(shouldBeVisible);
        }

        if (OnlineService.OnlineInterface?.SessionInterface != null)
        {
            _serverName.text = OnlineService.OnlineInterface.SessionInterface.HostName;
        }
        else
        {
            _serverName.text = "Local Game";
        }

        _data.Clear();

        SimWorld.Entities
            .WithAll<PlayerTag>()
            .ForEach((Entity pawnController, ref ControlledEntity pawn) =>
            {
                if (SimWorld.Exists(pawn))
                {
                    _data.Add((pawnController, pawn));
                }
            });

        UIUtility.UpdateGameObjectList(_playerInfos, _data, _playerInfoPrefab, _playerInfoContainer, onUpdate: UpdatePlayerInfo);
    }

    private void UpdatePlayerInfo(LobbyPlayerInfoDisplay display, (Entity player, Entity pawn) data)
    {
        FixedString64 playerName = "N/A";
        FixedString64 characterName = "N/A";

        if (SimWorld.TryGetComponentData(data.player, out Name playerNameComponent))
        {
            playerName = playerNameComponent.Value;
        }

        if (SimWorld.TryGetComponentData(data.pawn, out Name characterNameComponent))
        {
            characterName = characterNameComponent.Value;
        }

        display.UpdatePlayerInfoDisplay(playerName, characterName, data.pawn);
    }

    private void SetVisible(bool visible)
    {
        _tween?.Kill();

        if (visible)
        {
            _container.gameObject.SetActive(true);
            _tween = _container.DOAnchorPos(_endPosition, _animationDuration);
        }
        else
        {
            _tween = _container.DOAnchorPos(_startPosition, _animationDuration).OnComplete(() => _container.gameObject.SetActive(false));
        }

        _isVisible = visible;
    }
}