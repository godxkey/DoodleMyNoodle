using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGridBattlePlayerCharacterComponent : SimComponent, ISimPawnInputHandler
{
    public SimBlueprintId BulletPrefab;

    public bool HandleInput(SimInput input)
    {
        if (input is SimInputKeycode keyCodeInput && keyCodeInput.state == SimInputKeycode.State.Pressed)
        {
            switch (keyCodeInput.keyCode)
            {
                case KeyCode.Space:
                    // Shoot!
                    Simulation.Instantiate(BulletPrefab);
                    return false;

                case KeyCode.RightArrow:
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.right);
                    return false;

                case KeyCode.LeftArrow:
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.left);
                    return false;

                case KeyCode.UpArrow:
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.up);
                    return false;

                case KeyCode.DownArrow:
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.down);
                    return false;
            }
        }

        return false;
    }




    #region Component Caching
    [System.NonSerialized] SimGridWalkerComponent _gridWalker;
    public override void OnAddedToRuntime()
    {
        base.OnAddedToRuntime();
        _gridWalker = GetComponent<SimGridWalkerComponent>();
    }
    #endregion
}
