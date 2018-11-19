using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewORRule", menuName = "CCC/Context Rules/Rule Types/OR")]
public class ContextRuleOR : ContextRuleArrayBased
{
    protected override bool SpecificEvaluate(ContextRuleUpdater ruleUpdater)
    {
        foreach (ContextRule rule in childrenRules)
        {
            if (rule.Evaluate(ruleUpdater))
            {
                return true;
            }
        }

        return false;
    }
}
