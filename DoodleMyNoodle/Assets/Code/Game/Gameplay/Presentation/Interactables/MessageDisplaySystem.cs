using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class MessageDisplaySystem : GamePresentationBehaviour
{
    [SerializeField] private GameObject _messageBubble;
    [SerializeField] private TextMeshPro _messageText;

    [SerializeField] private Vector3 _displacement;

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorldCache.LocalPawn != Entity.Null)
        {
            int2 tilePos = Helpers.GetTile(SimWorld.GetComponentData<FixTranslation>(SimWorldCache.LocalPawn).Value);
            Entity tileEntity = CommonReads.GetTileEntity(SimWorld, tilePos);

            Entity messageEntity = CommonReads.GetFirstTileAddonWithComponent<Message>(SimWorld, tileEntity);

            if (messageEntity != Entity.Null)
            {
                Message message = SimWorld.GetComponentData<Message>(messageEntity);

                _messageText.text = message.ToString();

                _messageBubble.transform.position = new Vector3(tilePos.x + _displacement.x, tilePos.y + _displacement.y, _displacement.z);
                _messageBubble.SetActive(true);
            }
            else
            {
                _messageBubble.SetActive(false);
            }
        }
    }
}