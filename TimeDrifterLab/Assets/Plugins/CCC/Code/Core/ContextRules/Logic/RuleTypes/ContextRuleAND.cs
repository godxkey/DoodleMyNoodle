using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewANDRule", menuName = "CCC/Context Rules/Rule Types/AND")]
public class ContextRuleAND : ContextRuleArrayBased
{
    protected override bool SpecificEvaluate(ContextRuleUpdater ruleUpdater)
    {
        foreach (ContextRule child in childrenRules)
        {
            if(child.Evaluate(ruleUpdater) == false)
            {
                return false;
            }
        }

        return true;
    }
}
