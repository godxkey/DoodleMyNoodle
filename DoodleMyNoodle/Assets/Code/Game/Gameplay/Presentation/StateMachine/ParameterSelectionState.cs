using System.Collections.Generic;
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

            GameActionId actionId = SimWorld.GetComponentData<GameActionId>(_itemEntity);
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
            List<GameAction.ParameterData> parameterDatas = GetData<List<GameAction.ParameterData>>(1);
            foreach (GameAction.ParameterData parameterData in parameterDatas)
            {
                _itemUseParameters.ParameterDatas[_parameterIndex] = parameterData;
                _parameterIndex++;
            }
        }

        if (_itemUseContract.ParameterTypes.Length > _parameterIndex)
        {
            List<GameAction.ParameterDescription> newParameterList = new List<GameAction.ParameterDescription>();

            for (int i = _parameterIndex; i < _itemUseContract.ParameterTypes.Length; i++)
            {
                newParameterList.Add(_itemUseContract.ParameterTypes[i]);
            }

            UIStateMachine.Instance.TransitionTo(UIState.StateTypes.GameActionRequest, _itemEntity, newParameterList.ToArray());
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

    public override StateTypes StateType => StateTypes.ParameterSelection;

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}





