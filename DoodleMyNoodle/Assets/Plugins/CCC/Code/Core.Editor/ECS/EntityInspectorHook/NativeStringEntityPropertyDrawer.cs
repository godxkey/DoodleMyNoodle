using Unity.Properties;
using UnityEditor;
using Unity.Collections;

namespace CCC.Editor
{
    [CustomEntityPropertyDrawer]
    public class NativeStringEntityPropertyDrawer : IMGUIAdapter,
        IVisitAdapter<NativeString32>,
        IVisitAdapter<NativeString64>,
        IVisitAdapter<NativeString128>,
        IVisitAdapter<NativeString512>
    {

        #region Native String
        const int NATIVE_STRING_32_MAX_CHAR_LENGTH = NativeString32.MaxLength / sizeof(char);
        const int NATIVE_STRING_64_MAX_CHAR_LENGTH = NativeString64.MaxLength / sizeof(char);
        const int NATIVE_STRING_128_MAX_CHAR_LENGTH = NativeString128.MaxLength / sizeof(char);
        const int NATIVE_STRING_512_MAX_CHAR_LENGTH = NativeString512.MaxLength / sizeof(char);

        VisitStatus IVisitAdapter<NativeString32>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref NativeString32 value, ref ChangeTracker changeTracker)
        {
            DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
            {
                var currentValue = val.ToString();
                var newValue = EditorGUILayout.TextField(label, currentValue);

                if (!newValue.Equals(currentValue))
                {
                    if (newValue.Length > NATIVE_STRING_32_MAX_CHAR_LENGTH)
                    {
                        newValue = newValue.Substring(0, NATIVE_STRING_32_MAX_CHAR_LENGTH);
                    }
                    return new NativeString32(newValue);
                }

                return val;
            });

            return VisitStatus.Handled;
        }
        VisitStatus IVisitAdapter<NativeString64>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref NativeString64 value, ref ChangeTracker changeTracker)
        {
            DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
            {
                var currentValue = val.ToString();
                var newValue = EditorGUILayout.TextField(label, currentValue);

                if (!newValue.Equals(currentValue))
                {
                    if (newValue.Length > NATIVE_STRING_64_MAX_CHAR_LENGTH)
                    {
                        newValue = newValue.Substring(0, NATIVE_STRING_64_MAX_CHAR_LENGTH);
                    }
                    return new NativeString64(newValue);
                }

                return val;
            });

            return VisitStatus.Handled;
        }
        VisitStatus IVisitAdapter<NativeString128>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref NativeString128 value, ref ChangeTracker changeTracker)
        {
            DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
            {
                var currentValue = val.ToString();
                var newValue = EditorGUILayout.TextField(label, currentValue);

                if (!newValue.Equals(currentValue))
                {
                    if (newValue.Length > NATIVE_STRING_128_MAX_CHAR_LENGTH)
                    {
                        newValue = newValue.Substring(0, NATIVE_STRING_128_MAX_CHAR_LENGTH);
                    }
                    return new NativeString128(newValue);
                }

                return val;
            });

            return VisitStatus.Handled;
        }
        VisitStatus IVisitAdapter<NativeString512>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref NativeString512 value, ref ChangeTracker changeTracker)
        {
            DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
            {
                var currentValue = val.ToString();
                var newValue = EditorGUILayout.TextField(label, currentValue);

                if (!newValue.Equals(currentValue))
                {
                    if (newValue.Length > NATIVE_STRING_512_MAX_CHAR_LENGTH)
                    {
                        newValue = newValue.Substring(0, NATIVE_STRING_512_MAX_CHAR_LENGTH);
                    }
                    return new NativeString512(newValue);
                }

                return val;
            });

            return VisitStatus.Handled;
        }
        #endregion
    }
}