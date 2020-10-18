/*using CCC.InspectorDisplay.ExposeInEditorInternals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngineX;


namespace CCC.InspectorDisplay
{
    namespace ExposeInEditorInternals
    {
        public abstract class MemberDrawer
        {
            public readonly ExposeInEditorAttribute attribute;

            // if display format is a button-with-parameter, we need a dummy value to put in the field for the user
            private object m_buttonParameterValue;
            private string m_displayName;

            public MemberDrawer(ExposeInEditorAttribute attribute)
            {
                this.attribute = attribute;
            }

            public void Initialize()
            {
                // Create a 'default' value that we can use for the display of the 'button-with-parameter'
                if (MemberType != typeof(void))
                {
                    m_buttonParameterValue = MemberDrawerHelpers.InstantiateDefaultValue(MemberType);
                }
                m_displayName = ObjectNames.NicifyVariableName(MemberName);
            }

            protected enum DisplayFormat
            {
                None,
                Property,
                PlainButton,
                ButtonWithParameter
            }

            protected virtual DisplayFormat GetCurrentDisplayFormat(out bool readOnly)
            {
                bool canRead = (CanMemberRead && attribute.CanReadNow);
                bool canWrite = (CanMemberWrite && attribute.CanWriteNow);

                readOnly = canRead && !canWrite;
                bool writeOnly = !canRead && canWrite;

                if (writeOnly)
                {
                    return DisplayFormat.ButtonWithParameter;
                }
                else if (canRead || canWrite)
                {
                    return DisplayFormat.Property;
                }
                else
                {
                    return DisplayFormat.None;
                }
            }

            public virtual bool Draw(object target)
            {
                DisplayFormat displayFormat = GetCurrentDisplayFormat(out bool readOnly);

                bool wasGUIEnabled = GUI.enabled;
                GUI.enabled = !readOnly;

                bool valueChange = false;
                switch (displayFormat)
                {
                    default:
                    case DisplayFormat.None:
                        break;

                    case DisplayFormat.Property:
                    {
                        object val = ReadMember(target);

                        // Draw GUI Field
                        object newVal = MemberDrawerHelpers.DrawGUIField(MemberType, m_displayName, val);

                        // take note of value (might be used later)
                        m_buttonParameterValue = newVal;

                        if (!readOnly && !Equals(val, newVal))
                        {
                            WriteMember(target, newVal);
                            valueChange = true;
                        }
                    }
                    break;

                    case DisplayFormat.PlainButton:
                        if (GUILayout.Button(m_displayName))
                        {
                            InvokeMember(target);
                            valueChange = true;
                        }
                        break;

                    case DisplayFormat.ButtonWithParameter:
                    {
                        EditorGUILayout.BeginHorizontal();

                        // Draw GUI Button
                        if (MemberDrawerHelpers.DrawGUIButtonWithParameter(MemberType, m_displayName, m_buttonParameterValue, out m_buttonParameterValue))
                        {
                            WriteMember(target, m_buttonParameterValue);
                            valueChange = true;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    break;
                }

                GUI.enabled = wasGUIEnabled;

                return valueChange;
            }

            protected abstract string MemberName { get; }
            protected abstract Type MemberType { get; }
            protected abstract bool CanMemberRead { get; }
            protected abstract bool CanMemberWrite { get; }

            protected abstract object ReadMember(object target);
            protected abstract void WriteMember(object target, object obj);
            protected abstract void InvokeMember(object target);
        }

        public class PropertyDrawer : MemberDrawer
        {
            private readonly PropertyInfo m_propertyInfo;

            public PropertyDrawer(PropertyInfo propertyInfo, ExposeInEditorAttribute attribute)
                : base(attribute)
            {
                m_propertyInfo = propertyInfo;
            }

            protected override string MemberName => m_propertyInfo.Name;
            protected override Type MemberType => m_propertyInfo.PropertyType;
            protected override bool CanMemberRead => m_propertyInfo.CanRead;
            protected override bool CanMemberWrite => m_propertyInfo.CanWrite;

            protected override object ReadMember(object target)
            {
                return m_propertyInfo.GetValue(target);
            }

            protected override void WriteMember(object target, object obj)
            {
                m_propertyInfo.SetValue(target, obj);
            }

            public static bool IsValidMemberToDraw(PropertyInfo propertyInfo)
            {
                return (propertyInfo.CanRead || propertyInfo.CanWrite)
                    && MemberDrawerHelpers.IsSupportedType(propertyInfo.PropertyType);
            }

            protected override void InvokeMember(object target) => throw new NotImplementedException();
        }

        public class FieldDrawer : MemberDrawer
        {
            private readonly FieldInfo m_fieldInfo;

            public FieldDrawer(FieldInfo fieldInfo, ExposeInEditorAttribute attribute)
                : base(attribute)
            {
                m_fieldInfo = fieldInfo;
            }

            protected override string MemberName => m_fieldInfo.Name;
            protected override Type MemberType => m_fieldInfo.FieldType;
            protected override bool CanMemberRead => true;
            protected override bool CanMemberWrite => true;

            protected override object ReadMember(object target)
            {
                return m_fieldInfo.GetValue(target);
            }

            protected override void WriteMember(object target, object obj)
            {
                m_fieldInfo.SetValue(target, obj);
            }

            public static bool IsValidMemberToDraw(FieldInfo fieldInfo)
            {
                return MemberDrawerHelpers.IsSupportedType(fieldInfo.FieldType);
            }

            protected override void InvokeMember(object target) => throw new NotImplementedException();
        }

        public class MethodDrawer : MemberDrawer
        {
            private readonly MethodInfo m_methodInfo;
            private readonly ParameterInfo[] m_parameters;
            private readonly Type m_type;
            private static readonly object[] s_parameterArray = new object[1] { null };

            public MethodDrawer(MethodInfo methodInfo, ExposeInEditorAttribute attribute)
                : base(attribute)
            {
                m_methodInfo = methodInfo;
                m_parameters = m_methodInfo.GetParameters();
                m_type = GetTypeOfMethod(m_methodInfo, m_parameters);
            }

            protected override string MemberName => m_methodInfo.Name;
            protected override Type MemberType => m_type;
            protected override bool CanMemberRead => m_methodInfo.ReturnType != typeof(void) && m_parameters.Length == 0;
            protected override bool CanMemberWrite => m_parameters.Length != 0;

            protected override DisplayFormat GetCurrentDisplayFormat(out bool readOnly)
            {
                if (!CanMemberRead && (attribute.CanReadNow || attribute.CanWriteNow))
                {
                    readOnly = !attribute.CanWriteNow;
                    if (CanMemberWrite)
                    {
                        return DisplayFormat.ButtonWithParameter;
                    }
                    else
                    {
                        return DisplayFormat.PlainButton;
                    }
                }

                return base.GetCurrentDisplayFormat(out readOnly);
            }

            protected override object ReadMember(object target)
            {
                return m_methodInfo.Invoke(target, null);
            }

            protected override void WriteMember(object target, object obj)
            {
                s_parameterArray[0] = obj;
                m_methodInfo.Invoke(target, s_parameterArray);
            }

            protected override void InvokeMember(object target)
            {
                m_methodInfo.Invoke(target, null);
            }

            public static bool IsValidMemberToDraw(MethodInfo methodInfo)
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length > 1)
                {
                    return false;
                }

                Type type = GetTypeOfMethod(methodInfo, parameters);
                return type == typeof(void) || MemberDrawerHelpers.IsSupportedType(type);
            }

            private static Type GetTypeOfMethod(MethodInfo methodInfo, ParameterInfo[] parameters)
            {
                if (parameters.Length == 1)
                {
                    return parameters[0].ParameterType;
                }
                else
                {
                    return methodInfo.ReturnType;
                }
            }
        }
    }

    public static class ExposeInEditorStaticEditor
    {
        static readonly Dictionary<object, List<MemberDrawer>> s_cachedTargetMembers = new Dictionary<object, List<MemberDrawer>>();
        static readonly Dictionary<Type, MethodInfo> s_cachedOnValidateMethods = new Dictionary<Type, MethodInfo>();

        public static void DrawElementsExposedInEditor(UnityEditor.Editor editor)
        {
            DrawElementsExposedInEditor(editor.target, out bool shouldRepaint);
            if (shouldRepaint)
            {
                editor.Repaint();
            }
        }

        public static void DrawElementsExposedInEditor(object target, out bool shouldRepaint)
        {
            if (s_cachedTargetMembers.Count > 200) // make sure the cache doesn't expand forever
            {
                s_cachedTargetMembers.Clear();
            }

            if (!s_cachedTargetMembers.ContainsKey(target))
            {
                s_cachedTargetMembers.Add(target, CreateMemberDrawers(target.GetType()));
            }

            shouldRepaint = false;

            var memberDrawers = s_cachedTargetMembers[target];
            if (memberDrawers.Count == 0)
            {
                return;
            }

            // Draw header
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Exposed Members", EditorStyles.boldLabel);

            bool shouldCallOnValidate = false;
            foreach (var member in memberDrawers)
            {
                // draw property
                bool change = member.Draw(target);
                if (change && member.attribute.CallOnValidate)
                {
                    shouldCallOnValidate = true;
                }

                shouldRepaint |= member.attribute.ForceRepaint;
            }

            if (shouldCallOnValidate)
            {
                GetOnValidateMethod(target.GetType())?.Invoke(target, null);
            }
        }

        static MethodInfo GetOnValidateMethod(Type type)
        {
            if (!s_cachedOnValidateMethods.TryGetValue(type, out MethodInfo method))
            {
                method = type.GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                s_cachedOnValidateMethods.Add(type, method);
            }

            return method;
        }

        static List<MemberDrawer> CreateMemberDrawers(Type targetType)
        {
            var memberDrawers = new List<MemberDrawer>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = targetType.GetFields(bindingFlags)
                .Where(f =>
                    f.GetCustomAttribute<ExposeInEditorAttribute>() != null
                    && FieldDrawer.IsValidMemberToDraw(f));

            var properties = targetType.GetProperties(bindingFlags)
                .Where(p =>
                    p.GetCustomAttribute<ExposeInEditorAttribute>() != null
                    && ExposeInEditorInternals.PropertyDrawer.IsValidMemberToDraw(p));

            var methods = targetType.GetMethods(bindingFlags)
                .Where(m =>
                    m.GetCustomAttribute<ExposeInEditorAttribute>() != null
                    && MethodDrawer.IsValidMemberToDraw(m));

            foreach (var field in fields)
            {
                memberDrawers.Add(new FieldDrawer(field, field.GetCustomAttribute<ExposeInEditorAttribute>()));
            }
            foreach (var prop in properties)
            {
                memberDrawers.Add(new ExposeInEditorInternals.PropertyDrawer(prop, prop.GetCustomAttribute<ExposeInEditorAttribute>()));
            }
            foreach (var method in methods)
            {
                memberDrawers.Add(new MethodDrawer(method, method.GetCustomAttribute<ExposeInEditorAttribute>()));
            }

            foreach (var item in memberDrawers)
            {
                item.Initialize();
            }

            return memberDrawers;
        }
    }
}*/