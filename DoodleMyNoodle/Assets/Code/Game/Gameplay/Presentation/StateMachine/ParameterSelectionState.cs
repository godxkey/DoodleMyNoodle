using Unity.Entities;

public class ParameterSelectionState : UIState
{
    private Entity _itemEntity;
    private GameAction.UseParameters _itemUseParameters;
    private GameAction.UseContract _itemUseContract;
    private int _itemIndex;
    private byte _parameterIndex;

    public override void OnEnter()
    {
        // Are we using a new item ?
        if (_itemEntity != GetData<Entity>(0))
        {
            // Init process of parameter selection

            _itemEntity = GetData<Entity>(0);
            _itemIndex = GetData<int>(1);

            SimWorld.TryGetComponentData(_itemEntity, out GameActionId actionId);
            GameAction itemGameAction = GameActionBank.GetAction(actionId);

            GameAction.UseContext useContext = new GameAction.UseContext()
            {
                InstigatorPawn = Cache.LocalPawn,
                InstigatorPawnController = Cache.LocalController,
                Entity = _itemEntity
            };

            _itemUseContract = itemGameAction.GetUseContract(SimWorld, useContext);

            _itemUseParameters = GameAction.UseParameters.Create(new GameAction.ParameterData[_itemUseContract.ParameterTypes.Length]);
            _parameterIndex = 0;
        }
        else // same item as we were the last time in this state, we're coming from a child state
        {
            // no data found, cancel and go back to gameplay
            if (Data[1] == null)
            {
                UIStateMachine.Instance.TransitionTo(StateTypes.Gameplay);
                return;
            }

            // let's get the info child state sent us
            _itemUseParameters.ParameterDatas[_parameterIndex] = GetData<GameAction.ParameterData>(1);
            _parameterIndex++;
        }

        if (_itemUseContract.ParameterTypes.Length > _parameterIndex)
        {
            IdentifyParameterTypeAndTransition(_itemUseContract.ParameterTypes[_parameterIndex], _parameterIndex);
        }
        else
        {
            // process completed, we have all info let's use the item
            SimPlayerInputUseItem simInput = new SimPlayerInputUseItem(_itemIndex, _itemUseParameters);
            SimWorld.SubmitInput(simInput);

            UIStateMachine.Instance.TransitionTo(StateTypes.Gameplay);
        }
    }

    public override void OnUpdate() { }

    public override void OnExit(StateTypes newState) 
    {
        // we're completly exiting the parameter selection process, so there's no item currently used anymore
        if (newState == StateTypes.Gameplay || newState == StateTypes.BlockedGameplay)
        {
            _itemEntity = Entity.Null;
        }
    }

    public override StateTypes GetStateType()
    {
        return StateTypes.ParameterSelection;
    }

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }

    private void IdentifyParameterTypeAndTransition(GameAction.ParameterDescription parameterDescription, byte index)
    {
        // SELECT A SINGLE TILE
        if (parameterDescription is GameActionParameterTile.Description TileDescription)
        {
            if (TileDescription != null)
            {
                UIStateMachine.Instance.TransitionTo(UIState.StateTypes.TileSelection, _itemEntity, TileDescription);
                return;
            }
        }

        // TODO Entity selection...

        // MINI GAME
        if (parameterDescription is GameActionParameterMiniGame.Description MiniGameDescription)
        {
            if (MiniGameDescription != null)
            {
                UIStateMachine.Instance.TransitionTo(UIState.StateTypes.MiniGame, _itemEntity, MiniGameDescription);
                return;
            }
        }

        // Default Case
        _itemUseParameters.ParameterDatas[index] = new GameActionParameter.Data();
    }
}





