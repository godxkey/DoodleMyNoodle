using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CCC/Context Rules/Registers/Player Class")]
public class ContextRulePlayerClassRegister : ScriptableObject
{
    [SerializeField] ContextRuleUpdater ruleUpdater;
    [SerializeField] List<ContextRulePlayerClass> playerClassRules;

    DirtyValue<PlayerClassType> playerClassType = new DirtyValue<PlayerClassType>(PlayerClassType.Assassin);

    public void UpdateRules(PlayerClassType currentPlayerClass)
    {
        playerClassType.Value = currentPlayerClass;

        if (playerClassType.IsDirty)
        {
            foreach (ContextRulePlayerClass rule in playerClassRules)
            {
                rule.CurrentPlayerClass = currentPlayerClass;
                ruleUpdater.DirtyRule(rule);
            }
            playerClassType.Reset();
        }
    }
}
