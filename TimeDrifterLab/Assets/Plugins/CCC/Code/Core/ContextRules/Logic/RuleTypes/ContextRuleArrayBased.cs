using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextRuleArrayBased : ContextRule
{
    [SerializeField] List<ContextRule> subRules;

    protected override List<ContextRule> ProvideChildRulesArray()
    {
        return new List<ContextRule>(subRules);
    }
}
