using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraSystem : GameSystem<GameCameraSystem>
{
    public override bool SystemReady => true;

    public Camera Camera => GetComponent<Camera>();
}
