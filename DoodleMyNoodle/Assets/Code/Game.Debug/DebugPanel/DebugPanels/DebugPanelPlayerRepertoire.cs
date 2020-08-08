using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelPlayerRepertoire : DebugPanel
{
    public override string Title => "Player Repertoire";
    public override bool CanBeDisplayed => PlayerRepertoireSystem.Instance != null;

    public override void OnGUI()
    {
        PlayerRepertoireSystem repertoire = PlayerRepertoireSystem.Instance;
        var players = repertoire.Players;

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Player Name", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(players[i].PlayerName);
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("Id", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(players[i].PlayerId.Value.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("isServer", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(players[i].IsMaster.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("isLocal", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(repertoire.IsLocalPlayer(players[i].PlayerId).ToString());
        }
        GUILayout.EndVertical();
        ResetTextColor();


        GUILayout.BeginVertical();
        GUILayout.Label("SimPlayerId", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(players[i].SimPlayerId.ToString());
        }
        GUILayout.EndVertical();
        ResetTextColor();


        GUILayout.EndHorizontal();
    }

    static void ApplyPlayerTextColor(PlayerInfo playerInfo)
    {
        GUI.color = PlayerRepertoireSystem.Instance.IsLocalPlayer(playerInfo.PlayerId) ? Color.yellow : Color.white;
    }
    static void ResetTextColor()
    {
        GUI.color = Color.white;
    }
}
