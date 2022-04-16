using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;
using static Unity.Mathematics.math;
using CCC.Fix2D;


public class MessageDisplaySystem : GamePresentationSystem<MessageDisplaySystem>
{
    [SerializeField] private GameObject _container;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _readRange = 2;

    private DirtyValue<Message?> _displayedMessage;
    private DirtyValue<Vector3> _messagePosition;

    public override void PresentationUpdate()
    {
        UpdateData();
        UpdateView();
    }

    private void UpdateData()
    {
        _displayedMessage.Set(null);
        if (Cache.PointerInWorld &&  Cache.LocalPawn != Entity.Null)
        {
            if (distance(Cache.DEPRECATED_LocalPawnTile, Cache.PointedTile) <= _readRange)
            {
                foreach (var tileActor in Cache.PointedBodies)
                {
                    if (SimWorld.TryGetComponent(tileActor, out Message message))
                    {
                        _displayedMessage.Set(message);
                        _messagePosition.Set((Vector2)(fix2)SimWorld.GetComponent<FixTranslation>(tileActor));
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
            _container.transform.position = (Vector3)_messagePosition.Get();
        }

        if (_displayedMessage.ClearDirty())
        {
            if (_displayedMessage.Get().HasValue)
            {
                _container.SetActive(true);
                _text.text = _displayedMessage.Get().Value.ToString();
            }
            else
            {
                _container.SetActive(false);

            }
        }
    }
}