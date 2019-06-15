using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using UnityEngine;


[Serializable]
public class SimWorld : IDisposable
{
    [NonSerialized]
    public ISimBlueprintBank blueprintBank;

    [NonSerialized]
    ReadOnlyCollection<SimEntity> m_entitiesReadOnly;
    public ReadOnlyCollection<SimEntity> entities => m_entitiesReadOnly ?? (m_entitiesReadOnly = m_entities.AsReadOnly());

    List<SimEntity> m_entities = new List<SimEntity>();

    public SimEntity InstantiateEntity(string name)
    {
        // Create entity
        SimEntity entity = new SimEntity(name)
        {
            world = this,
            blueprintId = SimBlueprintId.invalid
        };

        // add entity to list
        m_entities.Add(entity);

        // Create view
        SimEntityView view = SimHelpers.ViewCreationHelper.CreateViewForEntity(entity, blueprintBank);

        // Bind view with sim
        SimHelpers.ViewAttachingHelper.AttachEntityAndComponents(entity, view);

        // Fire events
        SimHelpers.EventHelper.FireEventOnEntityAndComponents_OnAwake(entity);

        return entity;
    }

    public SimEntity InstantiateEntity(SimBlueprintId blueprintId)
    {
        if (blueprintBank == null)
        {
            DebugService.LogError("Cannot spawn blueprint: Blueprint bank is null");
            return null;
        }

        if (blueprintId.isValid == false)
        {
            DebugService.LogError("Cannot spawn blueprint: Blueprint id is invalid");
            return null;
        }

        SimBlueprint blueprint = blueprintBank.GetBlueprint(blueprintId);

        // Create entity & View
        SimEntity entity;
        SimEntityView view;
        blueprint.InstantiateEntityAndView(out entity, out view);

        // fill basic info
        entity.world = this;
        entity.blueprintId = blueprintId;

        // add entity to list
        m_entities.Add(entity);

        // bind view with sim
        SimHelpers.ViewAttachingHelper.AttachEntityAndComponents(entity, view);

        // Fire 'OnAwake' event
        SimHelpers.EventHelper.FireEventOnEntityAndComponents_OnAwake(entity);

        return entity;
    }

    public void DestroyEntity(SimEntity entity)
    {
        int i = m_entities.IndexOf(entity);
        if (i >= 0)
            DestroyEntityAt(i);
    }

    void DestroyEntityAt(int index)
    {
        SimEntity entity = m_entities[index];

        // Fire 'OnAwake' event
        SimHelpers.EventHelper.FireEventOnEntityAndComponents_OnDestroy(entity);

        if(entity.attachedToView)
        {
            SimEntityView entityView = (SimEntityView)entity.view;

            // detach view from sim
            SimHelpers.ViewAttachingHelper.DetachEntityAndComponents(entity);

            // destroy view
            SimHelpers.ViewCreationHelper.DestroyViewForEntityAndComponents(entityView);
        }

        // remove references (marking them as destroyed)
        entity.world = null;
        for (int i = 0; i < entity.components.Count; i++)
        {
            entity.components[i].entity = null;
        }

        // remove from lists
        entity.components.Clear();
        m_entities.RemoveAt(index);
    }

    public virtual void Tick_PreInput() { }
    public virtual void Tick_PostInput() { }

    public void Dispose()
    {
        // reverse loop to save cpu
        for (int i = m_entities.Count - 1; i >= 0; i--)
        {
            DestroyEntityAt(i);
        }
    }
}
