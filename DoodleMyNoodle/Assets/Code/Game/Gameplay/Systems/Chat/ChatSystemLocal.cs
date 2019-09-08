using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSystemLocal : ChatSystem
{
    public override void SubmitMessage(string message)
    {
        PlayerInfo localPlayer = PlayerRepertoireSystem.Instance.GetLocalPlayerInfo();


        AddNewLine(new ChatLine()
        {
            ChatterName = localPlayer.PlayerName,
            Message = message
        });
    }
}
