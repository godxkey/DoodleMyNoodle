using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SimTransformComponent : SimComponent
{
    [System.Serializable]
    public struct SerializedData
    {
        public fix3 LocalPosition;
        public fixQuaternion LocalRotation;
        public fix3 LocalScale;
        public SimTransformComponent Parent;
        public int SiblingIndex;
    }

    public fix3 LocalScale { get => _data.LocalScale; set { _data.LocalScale = value; DirtyCachedRelativeToSelfValues(); } }
    public fix3 LocalPosition { get => _data.LocalPosition; set { _data.LocalPosition = value; DirtyCachedRelativeToSelfValues(); } }
    public fixQuaternion LocalRotation { get => _data.LocalRotation; set { _data.LocalRotation = value; DirtyCachedRelativeToSelfValues(); } }


    public fix4x4 LocalToWorldMatrix
    {
        get
        {
            UpdateLocalToWorldMatrix();
            return _localToWorldMatrix.Val;
        }
    }
    public fix4x4 WorldToLocalMatrix
    {
        get
        {
            UpdateWorldToLocalMatrix();
            return _worldToLocalMatrix.Val;
        }
    }
    public fix3 WorldPosition
    {
        get
        {
            if (Parent == null)
            {
                return _data.LocalPosition;
            }
            else
            {
                UpdateLocalToWorldMatrix();
                return fix4x4.TransformPoint(_data.LocalPosition, _localToWorldMatrix.Val);
            }
        }
        set
        {
            if (Parent == null)
            {
                _data.LocalPosition = value;
            }
            else
            {
                UpdateWorldToLocalMatrix();
                _data.LocalPosition = fix4x4.TransformPoint(value, _worldToLocalMatrix.Val);
            }
        }
    }

    /// <summary>
    /// To set the parent, simply use the normal Unity
    /// </summary>
    public SimTransformComponent Parent => UnityTransform.parent?.GetComponent<SimTransformComponent>();

    void Update()
    {
        // VISUAL UPDATE

        // could be optimized later
        UnityTransform.localPosition = LocalPosition.ToUnityVec();
        UnityTransform.localRotation = LocalRotation.ToUnityQuat();
        UnityTransform.localScale = LocalScale.ToUnityVec();
    }

    public void SetParent(SimTransformComponent parent)
    {
        UnityTransform.SetParent(parent?.UnityTransform);
        _data.Parent = parent;
    }

    public void SetSiblingIndex(int siblingIndex)
    {
        UnityTransform.SetSiblingIndex(siblingIndex);
        _data.SiblingIndex = siblingIndex;
    }

    void UpdateMatrix()
    {
        if (_matrix.UpToDate)
            return;

        Debug.Log("UpdateMatrix");

        fix4x4.CreateTRS(_data.LocalPosition, _data.LocalRotation, _data.LocalScale, out _matrix.Val);

        // mark as up-to-date
        _matrix.UpToDate = true;
    }

    void UpdateLocalToWorldMatrix()
    {
        if (_localToWorldMatrix.UpToDate)
            return;
        Debug.Log("UpdateLocalToWorldMatrix");

        UpdateMatrix(); // needed for calculations

        SimTransformComponent parentTr = Parent;
        if (parentTr != null)
        {
            _localToWorldMatrix.Val = _matrix.Val * parentTr.LocalToWorldMatrix;
        }
        else
        {
            _localToWorldMatrix = _matrix;
        }

        _localToWorldMatrix.UpToDate = true;
    }

    void UpdateWorldToLocalMatrix()
    {
        if (_worldToLocalMatrix.UpToDate)
            return;
        Debug.Log("UpdateWorldToLocalMatrix");

        UpdateLocalToWorldMatrix(); // needed for calculations
        fix4x4.Invert(_localToWorldMatrix.Val, out _worldToLocalMatrix.Val);

        _worldToLocalMatrix.UpToDate = true;
    }

    struct LazyMatrix
    {
        public fix4x4 Val;
        public bool UpToDate;
    }

    [NonSerialized]
    LazyMatrix _matrix;
    [NonSerialized]
    LazyMatrix _localToWorldMatrix;
    [NonSerialized]
    LazyMatrix _worldToLocalMatrix;

    void DirtyCachedRelativeToSelfValues()
    {
        _matrix.UpToDate = false;
        _localToWorldMatrix.UpToDate = false;
        _worldToLocalMatrix.UpToDate = false;
        DirtyChildren();
    }
    void DirtyCachedRelativeToParentValues()
    {
        _localToWorldMatrix.UpToDate = false;
        _worldToLocalMatrix.UpToDate = false;
    }
    void DirtyChildren()
    {
        for (int i = 0; i < UnityTransform.childCount; i++)
        {
            SimTransformComponent childSimTr = UnityTransform.GetChild(i).GetComponent<SimTransformComponent>();
            if (childSimTr)
            {
                childSimTr.DirtyCachedRelativeToParentValues();
            }
        }
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    public SerializedData _data = new SerializedData() // needs to be public for Editor access
    {
        LocalScale = new fix3(1, 1, 1)
    };
    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);

        VerifyIntegrity();
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        SetParent(_data.Parent);
        if (_data.Parent)
            SetSiblingIndex(_data.SiblingIndex);

        base.PopFromDataStack(dataStack);
    }
    #endregion

    void VerifyIntegrity()
    {
        if (UnityTransform.parent?.gameObject != _data.Parent?.gameObject)
        {
            DebugService.LogError("Mismatch between the UnityTransform's parent and the SimTransform's parent. " +
                "Make sure you use the simTransform's methods like simTransform.SetParent()");
        }
    }

#if UNITY_EDITOR
    public void Editor_DirtyCachedAllValues()
    {
        DirtyCachedRelativeToSelfValues();
    }
#endif
}
