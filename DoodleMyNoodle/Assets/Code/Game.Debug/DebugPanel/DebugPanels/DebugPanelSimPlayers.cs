using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DebugPanelSimPlayers : DebugPanel
{
    public override string title => "Sim Players";

    public override bool canBeDisplayed => GameMonoBehaviourHelpers.GetSimulationWorld() != null;

    List<Entity> _cachedPlayers = new List<Entity>();

    void UpdateCachedPlayers()
    {
        _cachedPlayers.Clear();

        GameMonoBehaviourHelpers.GetSimulationWorld().Entities
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
        var simWorldAccessor = GameMonoBehaviourHelpers.GetSimulationWorld();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Player Name", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(simWorldAccessor.GetComponentData<Name>(simPlayers[i]).Value.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("Sim Player Id", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(simWorldAccessor.GetComponentData<PersistentId>(simPlayers[i]).Value.ToString());
        }
        ResetTextColor();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Label("isBeingPlayed", DebugPanelStyles.boldText);
        for (int i = 0; i < simPlayers.Count; i++)
        {
            ApplyPlayerTextColor(simPlayers[i]);
            GUILayout.Label(PlayerHelpers.GetPlayerFromSimPlayer(simPlayers[i], GameMonoBehaviourHelpers.GetSimulationWorld()) == null ? "false" : "true");
        }
        ResetTextColor();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    static void ApplyPlayerTextColor(Entity simPlayer)
    {
        PlayerInfo p = PlayerHelpers.GetPlayerFromSimPlayer(simPlayer, GameMonoBehaviourHelpers.GetSimulationWorld());
        GUI.color = p == null ? Color.red : Color.white;
    }
    static void ResetTextColor()
    {
        GUI.color = Color.white;
    }
}
