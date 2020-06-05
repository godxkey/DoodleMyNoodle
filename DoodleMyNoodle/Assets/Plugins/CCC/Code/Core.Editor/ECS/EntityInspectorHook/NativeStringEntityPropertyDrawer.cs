using Unity.Properties;
using UnityEditor;
using Unity.Collections;
using Unity.Properties.Adapters;
using System;

namespace CCC.Editor
{
    [CustomEntityPropertyDrawer]
    public class NativeStringEntityPropertyDrawer : IMGUIAdapter,
        IVisit<NativeString32>,
        IVisit<NativeString64>,
        IVisit<NativeString128>,
        IVisit<NativeString512>
    {
        const int NATIVE_STRING_32_MAX_CHAR_LENGTH = NativeString32.MaxLength / sizeof(char);
        const int NATIVE_STRING_64_MAX_CHAR_LENGTH = NativeString64.MaxLength / sizeof(char);
        const int NATIVE_STRING_128_MAX_CHAR_LENGTH = NativeString128.MaxLength / sizeof(char);
        const int NATIVE_STRING_512_MAX_CHAR_LENGTH = NativeString512.MaxLength / sizeof(char);


        VisitStatus IVisit<NativeString32>.Visit<TContainer>(Property<TContainer, NativeString32> property, ref TContainer container, ref NativeString32 value)
        {
            return VisitString(property, ref value, NATIVE_STRING_32_MAX_CHAR_LENGTH);
        }
        VisitStatus IVisit<NativeString64>.Visit<TContainer>(Property<TContainer, NativeString64> property, ref TContainer container, ref NativeString64 value)
        {
            return VisitString(property, ref value, NATIVE_STRING_64_MAX_CHAR_LENGTH);
        }
        VisitStatus IVisit<NativeString128>.Visit<TContainer>(Property<TContainer, NativeString128> property, ref TContainer container, ref NativeString128 value)
        {
            return VisitString(property, ref value, NATIVE_STRING_128_MAX_CHAR_LENGTH);
        }
        VisitStatus IVisit<NativeString512>.Visit<TContainer>(Property<TContainer, NativeString512> property, ref TContainer container, ref NativeString512 value)
        {
            return VisitString(property, ref value, NATIVE_STRING_512_MAX_CHAR_LENGTH);
        }

        private VisitStatus VisitString<TContainer, TStringType>(Property<TContainer, TStringType> property, ref TStringType value, int maxChar)
        {
            string currentValue = value.ToString();
            string newValue = EditorGUILayout.TextField(GetDisplayName(property), currentValue);

            if (!newValue.Equals(currentValue))
            {
                if (newValue.Length > maxChar)
                {
                    newValue = newValue.Substring(0, maxChar);
                }
                value = (TStringType)Activator.CreateInstance(typeof(TStringType), newValue);
            }

            return VisitStatus.Stop;
        }
    }
}