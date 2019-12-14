using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGridBattlePlayerCharacterComponent : SimEventComponent, 
    ISimPawnInputHandler
{
    [System.Serializable]
    struct SerializedData
    {
        public SimGridBattleBulletComponent BulletPrefab;
    }

    public bool HandleInput(SimPlayerInput input)
    {
        if (input is SimInputKeycode keyCodeInput && keyCodeInput.state == SimInputKeycode.State.Pressed)
        {
            switch (keyCodeInput.keyCode)
            {
                case KeyCode.Space:
                    // Shoot!
                    Simulation.Instantiate(_data.BulletPrefab, SimTransform.WorldPosition, FixQuaternion.Identity)
                        .Speed = FixVector2.Left * 10;
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


    public void ProcessInput(SimInput input)
    {
        //if (input is SimPlayerInput playerInput)
        //{
        //    SimPlayerInfo playerInfo = SimPlayerManager.Instance.GetSimPlayerInfo(playerInput.SimPlayerId);
        //    playerInfo.
        //}
    }


    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void SerializeToDataStack(SimComponentDataStack dataStack)
    {
        base.SerializeToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void DeserializeFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.DeserializeFromDataStack(dataStack);
    }
    #endregion


    #region Component Caching
    [System.NonSerialized] SimGridWalkerComponent _gridWalker;
    public override void OnAddedToRuntime()
    {
        base.OnAddedToRuntime();
        _gridWalker = GetComponent<SimGridWalkerComponent>();
    }
    #endregion
}
