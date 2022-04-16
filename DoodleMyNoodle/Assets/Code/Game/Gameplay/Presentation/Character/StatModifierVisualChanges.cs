using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class StatModifierVisualChanges : BindedPresentationEntityComponent
{
    [Serializable]
    private class StatModifierPresentationGameObject 
    {
        public GameObject _gameObject;
        public StatModifierType _statModifierType;
    }

    [SerializeField]
    private List<StatModifierPresentationGameObject> _presentationGameObjects = new List<StatModifierPresentationGameObject>();

    protected override void Awake()
    {
        base.Awake();

        foreach (StatModifierPresentationGameObject presentationGameObject in _presentationGameObjects)
        {
            presentationGameObject._gameObject.SetActive(false);
        }
    }

    public override void PresentationUpdate()
    {
        if(SimWorld.TryGetBufferReadOnly(SimEntity, out DynamicBuffer<StatModifier> StatModifiers)) 
        {
            foreach (StatModifierPresentationGameObject presentationGameObject in _presentationGameObjects)
            {
                bool shouldActive = false;
                foreach (var statModifier in StatModifiers)
                {
                    if (presentationGameObject._statModifierType == statModifier.Type)
                    {
                        shouldActive = true;
                        break;
                    }
                }

                presentationGameObject._gameObject.SetActive(shouldActive);
            }
        }
    }
}