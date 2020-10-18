using System;
using System.Diagnostics;
using UnityEngine;


/// <summary>
/// Exposes a member in the editor
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public class ExposeInEditorAttribute : Attribute
{
    public enum RWMode
    {
        ReadInEditMode = 1,
        ReadInPlayMode = 1 << 1,

        WriteInEditMode = 1 << 2,
        WriteInPlayMode = 1 << 3,

        ReadWriteInEditMode = ReadInEditMode | WriteInEditMode,
        ReadWriteInPlayMode = ReadInPlayMode | WriteInPlayMode,
        ReadWriteAlways = ReadWriteInEditMode | ReadWriteInPlayMode
    }

    public RWMode Mode = RWMode.ReadWriteInPlayMode | RWMode.ReadInEditMode;

    /// <summary>
    /// Should the OnValidate() method be called on the inspected UnityEngine.Object?
    /// </summary>
    public bool CallOnValidate = true;

    /// <summary>
    /// Should the editor be constantly be repainted ?
    /// </summary>
    public bool ForceRepaint = false;

    public bool CanReadInPlayMode => (Mode & RWMode.ReadInPlayMode) != 0;
    public bool CanReadInEditMode => (Mode & RWMode.ReadInEditMode) != 0;
    public bool CanWriteInPlayMode => (Mode & RWMode.WriteInPlayMode) != 0;
    public bool CanWriteInEditMode => (Mode & RWMode.WriteInEditMode) != 0;

    public bool CanReadNow => (CanReadInEditMode && !Application.isPlaying) || (CanReadInPlayMode && Application.isPlaying);
    public bool CanWriteNow => (CanWriteInEditMode && !Application.isPlaying) || (CanWriteInPlayMode && Application.isPlaying);
}