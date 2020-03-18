using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DebugPanelSimPlayers : DebugPanel
{
    public override string title => "Sim Players";

    public override bool canBeDisplayed => GameMonoBehaviourHelpers.SimulationWorld != null;

    List<Entity> _cachedPlayers = new List<Entity>();

    void UpdateCachedPlayers()
    {
        _cachedPlayers.Clear();

        using (EntityQuery query = GameMonoBehaviourHelpers.SimulationWorld.CreateEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadOnly<PersistentId>()))
        {
            using (NativeArray<Entity> simPlayers = query.ToEntityArray(Allocator.TempJob))
            {
                _cachedPlayers.AddRange(simPlayers);
            }
        }
    }

    public override void OnGUI()
    {
        UpdateCachedPlayers();
        var simPlayers = _cachedPlayers;
        var simWorldAccessor = GameMonoBehaviourHelpers.SimulationWorld;

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
            GUILayout.Label(PlayerIdHelpers.GetPlayerFromSimPlayer(simPlayers[i], GameMonoBehaviourHelpers.SimulationWorld) == null ? "false" : "true");
        }
        ResetTextColor();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    static void ApplyPlayerTextColor(Entity simPlayer)
    {
        PlayerInfo p = PlayerIdHelpers.GetPlayerFromSimPlayer(simPlayer, GameMonoBehaviourHelpers.SimulationWorld);
        GUI.color = p == null ? Color.red : Color.white;
    }
    static void ResetTextColor()
    {
        GUI.color = Color.white;
    }
}
