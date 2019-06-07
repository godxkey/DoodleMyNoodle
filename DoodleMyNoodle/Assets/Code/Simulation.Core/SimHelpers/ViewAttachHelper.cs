using System.Collections.Generic;

namespace SimHelpers
{
    public static class ViewAttachingHelper
    {
        public static void AttachEntityAndComponents(SimEntity entity, SimEntityView view)
        {
            if (entity.attachedToView)
            {
                DebugService.LogWarning($"Cannot sim-attach: SimEntity is already attached to a SimEntityView [{entity}].");
                return;
            }
            if (view.attachedToSim)
            {
                DebugService.LogWarning($"Cannot sim-attach: SimEntityView is already attached to a SimEntity [{view}].");
                return;
            }

            // attach entityView to entity
            AttachObjects(entity, view);


            // attach every componentView to a component
            foreach (SimComponentView componentView in view.GetComponentViews())
            {
                foreach (SimComponent component in entity.components)
                {
                    if (component.GetType() == componentView.simComponentType) // matching type ?
                    {
                        // If we have multiple components of the same type, this 'If Not Attached' check will
                        // allow us to attach them all one after the other
                        if (component.attachedToView == false)
                        {
                            AttachObjects(component, componentView);
                            break;
                        }
                    }
                }
            }
        }
        public static void AttachComponent(SimComponent component, SimComponentView view)
        {
            if (component.attachedToView)
            {
                DebugService.LogWarning($"Cannot sim-attach: SimComponent is already attached to a SimComponentView [{component}].");
                return;
            }
            if (view.attachedToSim)
            {
                DebugService.LogWarning($"Cannot sim-attach: SimComponentView is already attached to a SimComponent [{view}].");
                return;
            }

            AttachObjects(component, view);
        }


        public static void DetachEntityAndComponents(SimEntity entity)
        {
            if (!entity.attachedToView)
            {
                DebugService.LogWarning($"Cannot sim-detach: SimEntity is not attached to any SimEntityView [{entity}].");
                return;
            }

            DetachEntityAndComponents(entity, entity.view);
        }
        public static void DetachEntityAndComponents(SimEntityView view)
        {
            if (!view.attachedToSim)
            {
                DebugService.LogWarning($"Cannot sim-detach: SimEntityView is not attached to any SimEntity [{view}].");
                return;
            }

            DetachEntityAndComponents(view.simEntity, view);
        }
        public static void DetachComponent(SimComponent component)
        {
            if (!component.attachedToView)
            {
                DebugService.LogWarning($"Cannot sim-detach: SimComponent is not attached to any SimComponentView [{component}].");
                return;
            }

            DetachObjects(component, component.view);
        }

        private static void DetachEntityAndComponents(SimEntity entity, SimObjectView view)
        {
            // detach entityView from entity
            DetachObjects(entity, view);


            // detach every component
            foreach (SimComponent component in entity.components)
            {
                DetachObjects(component, component.view);
            }
        }

        private static void AttachObjects(SimObject obj, SimObjectView view)
        {
            obj.view = view;
            view.simObject = obj;

            view.OnAttached();
        }
        private static void DetachObjects(SimObject obj, SimObjectView view)
        {
            view.simObject = null;
            obj.view = null;

            view.OnDetached();
        }
    }
}