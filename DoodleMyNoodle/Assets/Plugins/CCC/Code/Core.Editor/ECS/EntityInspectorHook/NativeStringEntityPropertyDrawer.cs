using Unity.Properties;
using UnityEditor;
using Unity.Collections;
using Unity.Properties.Adapters;
using System;

namespace CCC.Editor
{
    [CustomEntityPropertyDrawer]
    public class NativeStringEntityPropertyDrawer : IMGUIAdapter,
        IVisit<FixedString32>,
        IVisit<FixedString64>,
        IVisit<FixedString128>,
        IVisit<FixedString512>
    {
        static int NATIVE_STRING_32_MAX_CHAR_LENGTH => FixedString32.UTF8MaxLengthInBytes / sizeof(char);
        static int NATIVE_STRING_64_MAX_CHAR_LENGTH => FixedString64.UTF8MaxLengthInBytes / sizeof(char);
        static int NATIVE_STRING_128_MAX_CHAR_LENGTH => FixedString128.UTF8MaxLengthInBytes / sizeof(char);
        static int NATIVE_STRING_512_MAX_CHAR_LENGTH => FixedString512.UTF8MaxLengthInBytes / sizeof(char);


        VisitStatus IVisit<FixedString32>.Visit<TContainer>(Property<TContainer, FixedString32> property, ref TContainer container, ref FixedString32 value)
        {
            return VisitString(property, ref value, NATIVE_STRING_32_MAX_CHAR_LENGTH);
        }
        VisitStatus IVisit<FixedString64>.Visit<TContainer>(Property<TContainer, FixedString64> property, ref TContainer container, ref FixedString64 value)
        {
            return VisitString(property, ref value, NATIVE_STRING_64_MAX_CHAR_LENGTH);
        }
        VisitStatus IVisit<FixedString128>.Visit<TContainer>(Property<TContainer, FixedString128> property, ref TContainer container, ref FixedString128 value)
        {
            return VisitString(property, ref value, NATIVE_STRING_128_MAX_CHAR_LENGTH);
        }
        VisitStatus IVisit<FixedString512>.Visit<TContainer>(Property<TContainer, FixedString512> property, ref TContainer container, ref FixedString512 value)
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