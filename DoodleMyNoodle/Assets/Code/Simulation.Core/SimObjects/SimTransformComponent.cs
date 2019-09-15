using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SimTransformComponent : SimComponent
{
    [SerializeField]
    FixVector3 _localPosition;
    [SerializeField]
    FixQuaternion _localRotation;
    [SerializeField]
    FixVector3 _localScale = new FixVector3(1, 1, 1);


    public FixVector3 LocalScale { get => _localScale; set { _localScale = value; DirtyCachedRelativeToSelfValues(); } }
    public FixVector3 LocalPosition { get => _localPosition; set { _localPosition = value; DirtyCachedRelativeToSelfValues(); } }
    public FixQuaternion LocalRotation { get => _localRotation; set { _localRotation = value; DirtyCachedRelativeToSelfValues(); } }


    public FixMatrix LocalToWorldMatrix
    {
        get
        {
            UpdateLocalToWorldMatrix();
            return _localToWorldMatrix.Val;
        }
    }
    public FixMatrix WorldToLocalMatrix
    {
        get
        {
            UpdateWorldToLocalMatrix();
            return _worldToLocalMatrix.Val;
        }
    }
    public FixVector3 WorldPosition
    {
        get
        {
            if(Parent == null)
            {
                return _localPosition;
            }
            else
            {
                UpdateLocalToWorldMatrix();
                return FixMatrix.TransformPoint(_localPosition, _localToWorldMatrix.Val);
            }
        }
        set
        {
            if (Parent == null)
            {
                _localPosition = value;
            }
            else
            {
                UpdateWorldToLocalMatrix();
                _localPosition =  FixMatrix.TransformPoint(value, _worldToLocalMatrix.Val);
            }
        }
    }

    public SimTransformComponent Parent => UnityTransform.parent?.GetComponent<SimTransformComponent>();

    void Update()
    {
        // VISUAL UPDATE

        // could be optimized later
        UnityTransform.localPosition = LocalPosition.ToUnityVec();
        UnityTransform.localRotation = LocalRotation.ToUnityQuat();
        UnityTransform.localScale = LocalScale.ToUnityVec();
    }

    void UpdateMatrix()
    {
        if (_matrix.UpToDate)
            return;

        Debug.Log("UpdateMatrix");

        FixMatrix.CreateTRS(_localPosition, _localRotation, _localScale, out _matrix.Val);

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
        FixMatrix.Invert(_localToWorldMatrix.Val, out _worldToLocalMatrix.Val);

        _worldToLocalMatrix.UpToDate = true;
    }

    struct LazyMatrix
    {
        public FixMatrix Val;
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

#if UNITY_EDITOR
    public void Editor_DirtyCachedAllValues()
    {
        DirtyCachedRelativeToSelfValues();
    }
#endif
}
