using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SceneLoadSettings
{
    public bool async;
    public bool additive;
    public bool allowMultiple;

    public SceneLoadSettings(bool async, bool additive, bool allowMultiple)
    {
        this.async = async;
        this.additive = additive;
        this.allowMultiple = allowMultiple;
    }
}
