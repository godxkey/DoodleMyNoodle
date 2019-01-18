using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNOTRule", menuName = "CCC/Context Rules/Rule Types/NOT")]
public class ContextRuleNOT : ContextRule
{
    [SerializeField] ContextRule ruleToInvert = null;

    protected override bool SpecificEvaluate(ContextRuleUpdater ruleUpdater)
    {
        return !ruleToInvert.Evaluate(ruleUpdater);
    }

    protected override List<ContextRule> ProvideChildRulesArray() => new List<ContextRule>() { ruleToInvert };
}
