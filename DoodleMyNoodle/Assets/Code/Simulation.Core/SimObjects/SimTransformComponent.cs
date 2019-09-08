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


    public FixVector3 localScale { get => _localScale; set { _localScale = value; DirtyCachedRelativeToSelfValues(); } }
    public FixVector3 localPosition { get => _localPosition; set { _localPosition = value; DirtyCachedRelativeToSelfValues(); } }
    public FixQuaternion localRotation { get => _localRotation; set { _localRotation = value; DirtyCachedRelativeToSelfValues(); } }


    public FixMatrix localToWorldMatrix
    {
        get
        {
            UpdateLocalToWorldMatrix();
            return _localToWorldMatrix.val;
        }
    }
    public FixMatrix worldToLocalMatrix
    {
        get
        {
            UpdateWorldToLocalMatrix();
            return _worldToLocalMatrix.val;
        }
    }
    public FixVector3 worldPosition
    {
        get
        {
            if(parent == null)
            {
                return _localPosition;
            }
            else
            {
                UpdateLocalToWorldMatrix();
                return FixMatrix.TransformPoint(_localPosition, _localToWorldMatrix.val);
            }
        }
        set
        {
            if (parent == null)
            {
                _localPosition = value;
            }
            else
            {
                UpdateWorldToLocalMatrix();
                _localPosition =  FixMatrix.TransformPoint(value, _worldToLocalMatrix.val);
            }
        }
    }

    public SimTransformComponent parent => UnityTransform.parent?.GetComponent<SimTransformComponent>();

    void Update()
    {
        // VISUAL UPDATE

        // could be optimized later
        UnityTransform.localPosition = localPosition.ToUnityVec();
        UnityTransform.localRotation = localRotation.ToUnityQuat();
        UnityTransform.localScale = localScale.ToUnityVec();
    }

    void UpdateMatrix()
    {
        if (_matrix.upToDate)
            return;

        Debug.Log("UpdateMatrix");

        FixMatrix.CreateTRS(_localPosition, _localRotation, _localScale, out _matrix.val);

        // mark as up-to-date
        _matrix.upToDate = true;
    }

    void UpdateLocalToWorldMatrix()
    {
        if (_localToWorldMatrix.upToDate)
            return;
        Debug.Log("UpdateLocalToWorldMatrix");

        UpdateMatrix(); // needed for calculations

        SimTransformComponent parentTr = parent;
        if (parentTr != null)
        {
            _localToWorldMatrix.val = _matrix.val * parentTr.localToWorldMatrix;
        }
        else
        {
            _localToWorldMatrix = _matrix;
        }

        _localToWorldMatrix.upToDate = true;
    }

    void UpdateWorldToLocalMatrix()
    {
        if (_worldToLocalMatrix.upToDate)
            return;
        Debug.Log("UpdateWorldToLocalMatrix");

        UpdateLocalToWorldMatrix(); // needed for calculations
        FixMatrix.Invert(_localToWorldMatrix.val, out _worldToLocalMatrix.val);

        _worldToLocalMatrix.upToDate = true;
    }

    struct LazyMatrix
    {
        public FixMatrix val;
        public bool upToDate;
    }

    [NonSerialized]
    LazyMatrix _matrix;
    [NonSerialized]
    LazyMatrix _localToWorldMatrix;
    [NonSerialized]
    LazyMatrix _worldToLocalMatrix;

    void DirtyCachedRelativeToSelfValues()
    {
        _matrix.upToDate = false;
        _localToWorldMatrix.upToDate = false;
        _worldToLocalMatrix.upToDate = false;
        DirtyChildren();
    }
    void DirtyCachedRelativeToParentValues()
    {
        _localToWorldMatrix.upToDate = false;
        _worldToLocalMatrix.upToDate = false;
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
