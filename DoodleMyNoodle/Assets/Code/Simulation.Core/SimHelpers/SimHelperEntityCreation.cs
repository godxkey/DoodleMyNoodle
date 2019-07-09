using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RecursiveMode
{
    Recursive,
    RootOnly
}

namespace SimHelper
{
    public static class EntityCreation
    {
        public static SimEntity CreateEntityTreeFromView(SimWorld world, GameObject gameObject, SimCompTransform2D parent, RecursiveMode recursive)
        { return null; }

        /// <summary>
        /// This: 
        /// <para/>Creates SimEntity
        /// <para/>Assigns SimComponents
        /// <para/>Assigns transform parenthood
        /// <para/>(optional) Recursive create children
        /// <para/>Binds sim with view
        /// </summary>
        public static SimEntity CreateEntityTreeFromView(SimWorld world, SimBlueprintId blueprintId, GameObject gameObject, SimCompTransform2D parent, RecursiveMode recursive)
        {
            return null;

            //SimEntityView entityView = gameObject.GetComponent<SimEntityView>();
            //if (!entityView)
            //{
            //    return null;
            //}

            //// create entity
            //SimEntity entity = world.InternalAddEntity(gameObject.name);
            //entity.blueprintId = blueprintId;

            //// add every component serialized the gameobject to the entity
            //SimComponentView[] componentViews = gameObject.GetComponents<SimComponentView>();
            //for (int i = 0; i < componentViews.Length; i++)
            //{
            //    entity.InternalAddComponent(componentViews[i].GetComponentFromSerializedData());
            //}

            //// Assign transform parent if needed
            //SimCompTransform2D simTransform = entity.transform;
            //if (simTransform != null && parent != null)
            //{
            //    simTransform.parent = parent;
            //}

            //// recursively create children
            //if (recursive == RecursiveMode.Recursive && simTransform != null)
            //{
            //    Transform tr = gameObject.transform;
            //    for (int i = 0; i < tr.childCount; i++)
            //    {
            //        CreateEntityTreeFromView(world, tr.GetChild(i).gameObject, simTransform, RecursiveMode.Recursive);
            //    }
            //}

            //// Bind sim with view
            //SimHelper.ViewAttach.AttachEntityAndComponents(entity, entityView);

            //return entity;
        }
    }
}