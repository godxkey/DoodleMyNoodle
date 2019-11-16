using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SceneLoadSettings
{
    public bool Async;
    public LoadSceneMode LoadSceneMode;
    public LocalPhysicsMode LocalPhysicsMode;
}
