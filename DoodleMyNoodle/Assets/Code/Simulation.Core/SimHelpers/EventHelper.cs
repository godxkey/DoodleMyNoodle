using System.Collections.Generic;

namespace SimHelpers
{
    public static class EventHelper
    {
        public static void FireEventOnEntityAndComponents_OnAwake(SimEntity entity)
        {
            entity.OnAwake();
            foreach (SimComponent component in entity.components)
            {
                component.OnAwake();
            }
        }
        public static void FireEventOnEntityAndComponents_OnDestroy(SimEntity entity)
        {
            entity.OnDestroy();
            foreach (SimComponent component in entity.components)
            {
                component.OnDestroy();
            }
        }

        public static void FireEventOnComponent_OnAwake(SimComponent component)
        {
            component.OnAwake();
        }
        public static void FireEventOnComponent_OnDestroy(SimComponent component)
        {
            component.OnDestroy();
        }
    }
}