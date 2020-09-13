using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class MessageDisplaySystem : GamePresentationSystem<MessageDisplaySystem>
{
    [SerializeField] private GameObject _messageBubble;
    [SerializeField] private TextMeshPro _messageText;

    [SerializeField] private Vector3 _displacement;

    public override bool SystemReady => true;

    protected override void OnGamePresentationUpdate()
    {
        bool active = false;

        if (SimWorldCache.LocalPawn != Entity.Null)
        {
            Entity messageEntity = CommonReads.FindFirstTileActorWithComponent<Message>(SimWorld, SimWorldCache.LocalPawnTile);

            if (messageEntity != Entity.Null)
            {
                Message message = SimWorld.GetComponentData<Message>(messageEntity);

                _messageText.text = message.ToString();

                int2 tilePos = SimWorldCache.LocalPawnTile;

                _messageBubble.transform.position = new Vector3(tilePos.x + _displacement.x, tilePos.y + _displacement.y, _displacement.z);
                active = true;
            }
        }

        _messageBubble.SetActive(active);
    }
}