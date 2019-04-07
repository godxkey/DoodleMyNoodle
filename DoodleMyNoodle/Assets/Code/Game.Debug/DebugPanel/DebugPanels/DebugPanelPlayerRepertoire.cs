using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelPlayerRepertoire : DebugPanel
{
    public override string Title => "Player Repertoire";
    public override bool CanBeDisplayed => PlayerRepertoire.instance != null;

    public override void OnGUI()
    {
        PlayerRepertoire repertoire = PlayerRepertoire.instance;
        var players = repertoire.players;

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Player Name", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(players[i].playerName);
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("Id", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(players[i].playerId.value.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("isServer", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(players[i].isServer.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("isLocal", DebugPanelStyles.boldText);
        for (int i = 0; i < players.Count; i++)
        {
            ApplyPlayerTextColor(players[i]);
            GUILayout.Label(repertoire.IsLocalPlayer(players[i].playerId).ToString());
        }
        GUILayout.EndVertical();


        GUILayout.EndHorizontal();
    }

    static void ApplyPlayerTextColor(PlayerInfo playerInfo)
    {
        GUI.color = PlayerRepertoire.instance.IsLocalPlayer(playerInfo.playerId) ? Color.yellow : Color.white;
    }
    static void ResetTextColor()
    {
        GUI.color = Color.white;
    }
}
