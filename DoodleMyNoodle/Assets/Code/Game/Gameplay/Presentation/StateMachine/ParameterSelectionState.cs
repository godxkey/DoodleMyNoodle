using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

public class ParameterSelectionState : UIState
{
    private Entity _objectEntity;
    private GameAction.UseParameters _objectUseParameters;
    private GameAction.UseContract _objectUseContract;
    private int _itemIndex;
    private bool _isItem;
    private byte _parameterIndex;

    public override void OnEnter()
    {
        // Are we using a new item ?
        if (_objectEntity != GetData<Entity>(0))
        {
            // Init process of parameter selection

            _objectEntity = GetData<Entity>(0);
            _isItem = GetData<bool>(1);
            if (_isItem)
            {
                _itemIndex = GetData<int>(2);
            }

            GameActionId actionId = SimWorld.GetComponentData<GameActionId>(_objectEntity);
            GameAction objectGameAction = GameActionBank.GetAction(actionId);

            GameAction.UseContext useContext = new GameAction.UseContext()
            {
                InstigatorPawn = Cache.LocalPawn,
                InstigatorPawnController = Cache.LocalController,
                Entity = _objectEntity
            };

            _objectUseContract = objectGameAction.GetUseContract(SimWorld, useContext);

            _objectUseParameters = GameAction.UseParameters.Create(new GameAction.ParameterData[_objectUseContract.ParameterTypes.Length]);
            _parameterIndex = 0;
        }
        else // same item as we were the last time in this state, we're coming from a child state
        {
            // no data found, cancel and go back to gameplay
            var paramList = GetData<List<GameAction.ParameterData>>(1);
            if (paramList == null || paramList.Count == 0)
            {
                UIStateMachine.Instance.TransitionTo(StateTypes.Gameplay);
                return;
            }

            // let's get the info child state sent us
            List<GameAction.ParameterData> parameterDatas = GetData<List<GameAction.ParameterData>>(1);
            foreach (GameAction.ParameterData parameterData in parameterDatas)
            {
                _objectUseParameters.ParameterDatas[_parameterIndex] = parameterData;
                _parameterIndex++;
            }
        }

        // We still have parameters to process in order to get the data we need to execute the game action
        if (_objectUseContract.ParameterTypes.Length > _parameterIndex)
        {
            List<GameAction.ParameterDescription> newParameterList = new List<GameAction.ParameterDescription>();

            // Get only the parameters we still need to trigger survey and get data (the rest is already done)
            for (int i = _parameterIndex; i < _objectUseContract.ParameterTypes.Length; i++)
            {
                newParameterList.Add(_objectUseContract.ParameterTypes[i]);
            }

            // We feed those parameters to the substate in order for it to find the best way to get the data
            UIStateMachine.Instance.TransitionTo(UIState.StateTypes.Survey, _objectEntity, newParameterList.ToArray());
        }
        else
        {
            // process completed, we have all info let's use the game action
            if (_isItem)
            {
                SimPlayerInputUseItem simInput = new SimPlayerInputUseItem(_itemIndex, _objectUseParameters);
                SimWorld.SubmitInput(simInput);
            }
            else
            {
                int2 entityPosition = (int2)SimWorld.GetComponentData<FixTranslation>(_objectEntity).Value;

                SimPlayerInputUseObjectGameAction simInput = new SimPlayerInputUseObjectGameAction(entityPosition, _objectUseParameters);
                SimWorld.SubmitInput(simInput);
            }

            UIStateMachine.Instance.TransitionTo(StateTypes.Gameplay);
        }
    }

    public override void OnUpdate() { }

    public override void OnExit(StateTypes newState) 
    {
        // we're completly exiting the parameter selection process, so there's no item currently used anymore
        if (newState != StateTypes.Survey)
        {
            _objectEntity = Entity.Null;
        }
    }

    public override StateTypes StateType => StateTypes.ParameterSelection;

    public override bool IsTransitionValid(StateTypes newState)
    {
        return true;
    }
}





