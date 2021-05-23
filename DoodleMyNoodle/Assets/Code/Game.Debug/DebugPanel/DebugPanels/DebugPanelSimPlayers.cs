using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DebugPanelSimPlayers : DebugPanel
{
    public override string Title => "Sim Players";

    public override bool CanBeDisplayed => PresentationHelpers.GetSimulationWorld() != null;

    List<Entity> _cachedPlayers = new List<Entity>();

    void UpdateCachedPlayers()
    {
        _cachedPlayers.Clear();

        PresentationHelpers.GetSimulationWorld().Entities
            .WithAll<PlayerTag, Name, PersistentId>()
            .ForEach((Entity playerEntity) =>
        {
            _cachedPlayers.Add(playerEntity);
        });
    }

    public override void OnGUI()
    {
        UpdateCachedPlayers();
        var simPlayers = _cachedPlayers;
        var simWorldAccessor = PresentationHelpers.GetSimulationWorld();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Player Name", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(simWorldAccessor.GetComponent<Name>(simPlayers[i]).Value.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("Sim Player Id", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(simWorldAccessor.GetComponent<PersistentId>(simPlayers[i]).Value.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("isBeingPlayed", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(PlayerHelpers.GetPlayerFromSimPlayer(simPlayers[i], PresentationHelpers.GetSimulationWorld()) == null ? "false" : "true");
        }
        ResetTextColor();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    static void ApplyPlayerTextColor(Entity simPlayer)
    {
        PlayerInfo p = PlayerHelpers.GetPlayerFromSimPlayer(simPlayer, PresentationHelpers.GetSimulationWorld());
        GUI.color = p == null ? Color.red : Color.white;
    }
    static void ResetTextColor()
    {
        GUI.color = Color.white;
    }
}
