using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextRule : ScriptableObject
{
    bool lastEvaluationResult;
    bool loopsBroken;
    List<ContextRule> parentRules = new List<ContextRule>();
    protected List<ContextRule> childrenRules = new List<ContextRule>();

    protected abstract bool SpecificEvaluate(ContextRuleUpdater ruleUpdater);
    protected abstract List<ContextRule> ProvideChildRulesArray();

    public void SetRuleDirty()
    {
        Dirty = true;
    }
    public bool Dirty { get; private set; }

    void Init()
    {
        Dirty = true;
        loopsBroken = false;
        childrenRules = ProvideChildRulesArray();
        AddReferenceToChildren();

        if (Debug.isDebugBuild) // we shoudnt need to break loops in complete build as they are considered data errors
        {
            BreakLoops(new HashSet<ContextRule>() { this });
        }
    }

    protected virtual void OnEnable()
    {
        if (Application.isPlaying)
        {
            Init();
        }
    }

    protected virtual void OnDisable()
    {
        RemoveReferenceToChildren();
        childrenRules = null;
    }

    protected virtual void OnValidate()
    {
        RemoveReferenceToChildren();
        Init();
    }

    void AddReferenceToChildren()
    {
        // add ourselves to our children
        foreach (ContextRule rule in childrenRules)
        {
            rule?.parentRules.Add(this);
        }
    }

    void RemoveReferenceToChildren()
    {
        // add ourselves to our children
        foreach (ContextRule rule in childrenRules)
        {
            rule?.parentRules.Add(this);
        }
    }

    void BreakLoops(HashSet<ContextRule> visitedRules)
    {
        if (loopsBroken)
        {
            return;
        }

        foreach (ContextRule rule in childrenRules)
        {
            if (rule == null)
                continue;

            if (visitedRules.Remove(rule))
            {
                Debug.Assert(false, "Loop discovered and broken in (ContextRule)" + name + ".");
            }
            else
            {
                visitedRules.Add(rule);
                rule.BreakLoops(new HashSet<ContextRule>(visitedRules));
            }
        }

        loopsBroken = true;
    }



    public bool Evaluate()
    {
        return Evaluate(null);
    }

    public bool Evaluate(ContextRuleUpdater ruleUpdater)
    {
        if (Dirty)
        {
            bool previousResult = lastEvaluationResult;
            lastEvaluationResult = SpecificEvaluate(ruleUpdater);

            if (ruleUpdater != null && previousResult != lastEvaluationResult)
            {
                ruleUpdater.DirtyRules(parentRules);
            }

            Dirty = false;
        }

        return lastEvaluationResult;
    }
}
