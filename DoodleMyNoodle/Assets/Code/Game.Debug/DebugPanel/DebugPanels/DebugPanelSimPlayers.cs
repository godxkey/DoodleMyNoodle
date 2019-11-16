using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelSimPlayers : DebugPanel
{
    public override string title => "Sim Players";

    public override bool canBeDisplayed =>
        SimulationView.IsRunningOrReadyToRun
        && Game.Started
        && SimPlayerManager.Instance;

    public override void OnGUI()
    {
        var simPlayers = SimPlayerManager.Instance.Players;

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Player Name", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(simPlayers[i].Name);
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("Sim Player Id", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(simPlayers[i].SimPlayerId.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("isBeingPlayed", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(PlayerIdHelpers.GetPlayerFromSimPlayer(simPlayers[i]) == null ? "false" : "true");
        }
        ResetTextColor();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    static void ApplyPlayerTextColor(ISimPlayerInfo simPlayerInfo)
    {
        PlayerInfo p = PlayerIdHelpers.GetPlayerFromSimPlayer(simPlayerInfo);
        GUI.color = p == null ? Color.red : Color.white;
    }
    static void ResetTextColor()
    {
        GUI.color = Color.white;
    }
}
