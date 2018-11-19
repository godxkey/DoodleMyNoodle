using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CCC/Context Rules/Context Rule Updater")]
public class ContextRuleUpdater : ScriptableObject
{
    List<ContextRule> dirtyRules;

    public void DirtyRule(ContextRule rule)
    {
        rule.SetRuleDirty();
        dirtyRules.Add(rule);
    }
    public void DirtyRules(List<ContextRule> rules)
    {
        foreach (ContextRule rule in rules)
        {
            rule.SetRuleDirty();
        }
        dirtyRules.AddRange(rules);
    }

    public void UpdateDirtyRules()
    {
        while (dirtyRules.Count > 0)
        {
            int dirtyCount = dirtyRules.Count;
            for (int i = dirtyCount - 1; i >= 0; i--)
            {
                dirtyRules[i].Evaluate(this);
                dirtyRules.RemoveAt(i); // TODO: Remove with last swap
            }
        }
    }
}
