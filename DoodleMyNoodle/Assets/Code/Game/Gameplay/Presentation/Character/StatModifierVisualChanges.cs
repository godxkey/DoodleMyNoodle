using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;

public class StatModifierVisualChanges : BindedPresentationEntityComponent
{
    [Serializable]
    private class StatModifierPresentationGameObject
    {
        [FormerlySerializedAs("_gameObject")]
        public GameObject GameObject;
        [FormerlySerializedAs("_statModifierType")]
        public StatModifierType StatModifierType;
    }

    [SerializeField]
    private List<StatModifierPresentationGameObject> _presentationGameObjects = new List<StatModifierPresentationGameObject>();

    protected override void Awake()
    {
        base.Awake();

        foreach (StatModifierPresentationGameObject presentationGameObject in _presentationGameObjects)
        {
            presentationGameObject.GameObject.SetActive(false);
        }
    }

    public override void PresentationUpdate()
    {
        if (SimWorld.TryGetBufferReadOnly(SimEntity, out DynamicBuffer<StatModifier> statModifiers))
        {
            foreach (StatModifierPresentationGameObject presentationGameObject in _presentationGameObjects)
            {
                bool shouldActive = false;
                foreach (var statModifier in statModifiers)
                {
                    if (presentationGameObject.StatModifierType == statModifier.Type)
                    {
                        shouldActive = true;
                        break;
                    }
                }

                presentationGameObject.GameObject.SetActive(shouldActive);
            }
        }
    }
}