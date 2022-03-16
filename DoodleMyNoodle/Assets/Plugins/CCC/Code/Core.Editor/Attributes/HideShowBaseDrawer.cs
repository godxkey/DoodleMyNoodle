using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

namespace CCC.InspectorDisplay
{
    public abstract class HideShowBaseDrawer : PropertyDrawer
    {
        protected abstract bool DefaultMemberValue { get; }
        protected abstract bool IsShownIfMemberTrue { get; }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Evaluate(property) == IsShownIfMemberTrue)
            {
                bool indentProperty = ((HideShowBaseAttribute)attribute).IndentProperty;
                if (indentProperty)
                    EditorGUI.indentLevel++;
                EditorGUI.PropertyField(position, property, label, true);
                if (indentProperty)
                    EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Evaluate(property) == IsShownIfMemberTrue ? EditorGUI.GetPropertyHeight(property, label) : -2;
        }

        private bool Evaluate(SerializedProperty property)
        {
            if (property.serializedObject.isEditingMultipleObjects)
                return DefaultMemberValue;

            object containerInstance = GetContainerInstanceFromSerializedProperty(property);

            if (containerInstance == null)
                return DefaultMemberValue;

            return EvaluateMember(containerInstance, ((HideShowBaseAttribute)attribute).ConditionalMemberName);
        }

        private object GetContainerInstanceFromSerializedProperty(SerializedProperty property)
        {
            string parentPath = GetPropertyParentPath(property.propertyPath);

            if (parentPath == "")
            {
                // we're already at the root-level
                return property.serializedObject.targetObject;
            }
            else
            {
                // we need to dig in deeper in the serialized data
                return GetObjectInstanceFromPath(property.serializedObject.targetObject, parentPath);
            }
        }

        string GetPropertyParentPath(string propertyPath)
        {
            // the serialized property path will look like this:
            // theRootObject.aSubProperty.Array.data[16].ourProperty
            string path = propertyPath;
            if (path.Contains("."))
            {
                path = path.Remove(path.LastIndexOf('.'));

                if (path.EndsWith(".Array")) // if the path end with .Array, we'll want to strip that away and restart
                {
                    path = path.Remove(path.LastIndexOf('.'));
                    return GetPropertyParentPath(path);
                }
            }
            else
            {
                path = "";
            }

            return path;
        }

        object GetObjectInstanceFromPath(object parentObject, string objectPath)
        {
            string[] pathSerializedNames = objectPath.Split('.');
            try
            {
                for (int i = 0; i < pathSerializedNames.Length; i++)
                {
                    if (pathSerializedNames[i] == "Array")
                    {
                        // the serialized name will be like this 'Array.data[15].TheThingAfter'

                        ++i; // skip 'Array'

                        // we want to extract the '15' out of 'data[15]'
                        string dataIndex = pathSerializedNames[i].Substring("data".Length + 1, pathSerializedNames[i].Length - "data".Length - "[]".Length);
                        int index = int.Parse(dataIndex);
                        if (parentObject is IList list)
                        {
                            if (list.Count <= index)
                                return null;
                            parentObject = list[index];
                        }
                    }
                    else
                    {
                        FieldInfo fieldInfo = parentObject.GetType().GetField(pathSerializedNames[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        parentObject = fieldInfo.GetValue(parentObject);
                    }
                }
            }
            catch (Exception e)
            {
                LogWarning("Error in code: " + e.Message);
                return null;
            }

            return parentObject;
        }

        private bool EvaluateMember(
            object containerInstance,
            string conditionalMemberName)
        {
            Type containerType = containerInstance.GetType();
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            while (containerType != null)
            {
                MemberInfo[] memberInfos = containerType.GetMember(conditionalMemberName, bindingFlags);

                if (memberInfos.Length > 0)
                {
                    MemberInfo memberInfo = memberInfos[0]; // we should not find more than 1 member

                    if (CanMemberBeEvaluatedToBoolean(memberInfo))
                    {
                        if (IsMemberStatic(memberInfo))
                        {
                            return EvaluateMember(null, memberInfo);
                        }
                        else
                        {
                            return EvaluateMember(containerInstance, memberInfo);
                        }
                    }
                    else
                    {
                        LogWarning($"The {memberInfo.MemberType} named \"{conditionalMemberName}\" is not of type bool.");
                        return DefaultMemberValue;
                    }
                }
                else
                {
                    containerType = containerType.BaseType;
                }
            }

            LogWarning($"Failed to get the member named \"{conditionalMemberName}\".");

            return DefaultMemberValue;
        }

        private bool CanMemberBeEvaluatedToBoolean(MemberInfo memberInfo)
        {
            Type type;
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    type = fieldInfo.FieldType;
                    break;

                case MethodInfo methodInfo:
                    type = methodInfo.ReturnType;
                    break;

                case PropertyInfo propertyInfo:
                    type = propertyInfo.PropertyType;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return type == typeof(bool);
        }

        private bool IsMemberStatic(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.IsStatic;

                case MethodInfo methodInfo:
                    return methodInfo.IsStatic;

                case PropertyInfo propertyInfo:
                    return propertyInfo.GetMethod.IsStatic;

                default:
                    throw new NotImplementedException();
            }
        }

        private bool EvaluateMember(object containerInstance, MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    return (bool)fieldInfo.GetValue(containerInstance);

                case MethodInfo methodInfo:
                    return (bool)methodInfo.Invoke(containerInstance, null);

                case PropertyInfo propertyInfo:
                    return (bool)propertyInfo.GetValue(containerInstance);

                default:
                    throw new NotImplementedException();
            }
        }

        private void LogWarning(string message)
        {
            Log.Warning($"[ShowIf/HideIf] {message}");
        }
    }
}