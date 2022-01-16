using System;
using UnityEngine;
using UnityEngineX;

public class YourTurnDisplaySystem : GamePresentationSystem<YourTurnDisplaySystem>
{
    [SerializeField] private TextData _yourTurnMessage;
    [SerializeField] private AudioPlayable _yourTurnSFX;

    protected override void OnGamePresentationUpdate()
    {
        if (Cache.IsNewTurn && Cache.CanLocalPlayerPlay)
        {
            DebugScreenMessage.DisplayMessage(_yourTurnMessage.ToString());
            DefaultAudioSourceService.Instance.PlayStaticSFX(_yourTurnSFX);
        }
    }
}