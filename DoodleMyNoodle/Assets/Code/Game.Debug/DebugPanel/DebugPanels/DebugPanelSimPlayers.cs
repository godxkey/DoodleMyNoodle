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

    List<SimPlayerComponent> _cachedPlayers;

    void UpdateCachedPlayers()
    {
        _cachedPlayers.Clear();
        foreach (SimPlayerComponent simPlayer in SimulationView.EntitiesWithComponent<SimPlayerComponent>())
        {
            _cachedPlayers.Add(simPlayer);
        }
    }

    public override void OnGUI()
    {
        UpdateCachedPlayers();
        var simPlayers = _cachedPlayers;

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Player Name", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(SimPlayerHelpers.GetPlayerName(simPlayers[i]));
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

    static void ApplyPlayerTextColor(SimPlayerComponent simPlayer)
    {
        PlayerInfo p = PlayerIdHelpers.GetPlayerFromSimPlayer(simPlayer);
        GUI.color = p == null ? Color.red : Color.white;
    }
    static void ResetTextColor()
    {
        GUI.color = Color.white;
    }
}
