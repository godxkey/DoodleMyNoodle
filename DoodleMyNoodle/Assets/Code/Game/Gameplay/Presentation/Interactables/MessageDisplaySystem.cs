using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;
using static Unity.Mathematics.math;


public class MessageDisplaySystem : GamePresentationSystem<MessageDisplaySystem>
{
    [SerializeField] private GameObject _messageBubble;
    [SerializeField] private TextMeshPro _messageText;
    [SerializeField] private float _readRange = 2;

    [SerializeField] private Vector3 _displacement;

    private DirtyValue<Message?> _displayedMessage;
    private DirtyValue<int2> _messagePosition;

    public override bool SystemReady => true;

    protected override void OnGamePresentationUpdate()
    {
        UpdateData();
        UpdateView();
    }

    private void UpdateData()
    {
        _displayedMessage.Set(null);
        if (SimWorldCache.LocalPawn != Entity.Null)
        {
            if (distance(SimWorldCache.LocalPawnTile, SimWorldCache.TileUnderCursor) <= _readRange)
            {
                foreach (var item in SimWorldCache.TileActorsUnderCursor)
                {
                    if (SimWorld.TryGetComponentData(item, out Message message))
                    {
                        _displayedMessage.Set(message);
                        _messagePosition.Set(SimWorldCache.TileUnderCursor);
                        break;
                    }
                }
            }
        }
    }

    private void UpdateView()
    {
        if (_messagePosition.ClearDirty())
        {
             var p = (Vector3)Helpers.GetTileCenter(_messagePosition.Get());
            _messageBubble.transform.position = p + _displacement;
        }

        if (_displayedMessage.ClearDirty())
        {
            if (_displayedMessage.Get().HasValue)
            {
                _messageBubble.SetActive(true);
                _messageText.text = _displayedMessage.Get().Value.ToString();
            }
            else
            {
                _messageBubble.SetActive(false);

            }
        }
    }
}