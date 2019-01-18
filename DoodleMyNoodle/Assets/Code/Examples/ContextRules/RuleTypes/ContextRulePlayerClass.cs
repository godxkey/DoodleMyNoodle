using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerClassRule", menuName = "CCC/Context Rules/Rule Types/Player Class")]
public class ContextRulePlayerClass : ContextRule
{
    [SerializeField] PlayerClassType playerClass;
    public PlayerClassType CurrentPlayerClass { private get; set; }

    protected override List<ContextRule> ProvideChildRulesArray() { return new List<ContextRule>(0); }

    protected override bool SpecificEvaluate(ContextRuleUpdater ruleUpdater)
    {
        return playerClass == CurrentPlayerClass;
    }
}
