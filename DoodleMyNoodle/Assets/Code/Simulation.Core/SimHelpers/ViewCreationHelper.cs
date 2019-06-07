using UnityEngine;

namespace SimHelpers
{
    public static class ViewCreationHelper
    {
        public static SimEntityView CreateViewForEntity(SimEntity entity, ISimBlueprintBank blueprintBank)
        {
            if (entity.blueprintId.isValid)
            {
                SimBlueprint blueprint = blueprintBank.GetBlueprint(entity.blueprintId);
                // todo

                SimEntity discardEntity;
                SimEntityView entityView;
                blueprint.InstantiateEntityAndView(out discardEntity, out entityView);

                ViewAttachingHelper.DetachEntityAndComponents(entityView);

                return entityView;
            }
            else
            {
                // Manual view creation
                GameObject viewGameObject = new GameObject(entity.name + "-view");
                SimEntityView entityView = viewGameObject.AddComponent<SimEntityView>();

                foreach (SimComponent component in entity.components)
                {
                    CreateViewForComponent(viewGameObject, component);
                }

                return entityView;
            }
        }

        public static SimComponentView CreateViewForComponent(SimEntity entity, SimComponent component)
        {
            return CreateViewForComponent(entity.view.gameObject, component);
        }

        private static SimComponentView CreateViewForComponent(GameObject viewGameObject, SimComponent component)
        {
            return (SimComponentView)viewGameObject.AddComponent(typeof(SimComponentView).MakeGenericType(component.GetType()));
        }

        public static void DestroyViewForEntityAndComponents(SimEntityView view)
        {
            if (view != null)
            {
                GameObject gameobject = view.gameObject;
                bool destroyGameObject = view.destroyOnDetach;
                float destroyDelay = view.destroyDelay;
                SimComponentView[] componentViews = view.GetComponentViews();

                // Destroy entity view
                GameObject.Destroy(view);

                // Destroy component views
                for (int i = 0; i < componentViews.Length; i++)
                {
                    GameObject.Destroy(componentViews[i]);
                }

                // Destroy gameobject
                if (destroyGameObject)
                {
                    if (destroyDelay > 0)
                        GameObject.Destroy(gameobject);
                    else
                        GameObject.Destroy(gameobject, destroyDelay);
                }
            }
        }

        public static void DestroyViewForComponent(SimComponentView view)
        {
            GameObject.Destroy(view);
        }
    }
}