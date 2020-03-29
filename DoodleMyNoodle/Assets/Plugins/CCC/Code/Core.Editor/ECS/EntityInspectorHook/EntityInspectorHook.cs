using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace CCC.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomEntityPropertyDrawer : Attribute
    {

    }

    [InitializeOnLoad] // to make sure the static constructor is called
    public static class EntityInspectorHook
    {
        private static IPropertyVisitorAdapter[] s_editors;

        static EntityInspectorHook()
        {
            TypeCache.TypeCollection typeCollection = TypeCache.GetTypesWithAttribute<CustomEntityPropertyDrawer>();

            s_editors = typeCollection.Where((type) =>
            {
                if (!typeof(IPropertyVisitorAdapter).IsAssignableFrom(type))
                {
                    Debug.LogError($"Custom entity inspector {type.Name} will be ignored because it doesn't implement" +
                        $" the {nameof(IPropertyVisitorAdapter)} interface.");
                    return false;
                }
                return true;
            }).Select((type) => (IPropertyVisitorAdapter)Activator.CreateInstance(type))
            .ToArray();

            // The property adapter will take care of drawing in the inspector
            Unity.Entities.Editor.InspectorUserHook.s_AdapterBuilder = () => s_editors;
        }
    }

    public class IMGUIAdapter : IPropertyVisitorAdapter
    {
        protected static void DoField<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker, Func<GUIContent, TValue, TValue> drawer)
            where TProperty : IProperty<TContainer, TValue>
        {
            EditorGUI.BeginChangeCheck();

            value = drawer(new GUIContent(property.GetName()), value);

            if (EditorGUI.EndChangeCheck())
            {
                changeTracker.MarkChanged();
            }
        }
    }
}