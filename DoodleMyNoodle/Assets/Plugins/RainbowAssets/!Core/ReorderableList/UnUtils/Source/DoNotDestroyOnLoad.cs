// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEngine;

namespace Borodar.RainbowAssets.Core.Games
{
    /// <summary>
    /// Marks a game object upon awakening so that it is not destroyed between scenes.
    /// </summary>
    public sealed class DoNotDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
