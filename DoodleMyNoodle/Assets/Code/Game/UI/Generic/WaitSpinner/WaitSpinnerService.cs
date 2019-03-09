using Internals.WaitSpinnerService;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitSpinnerService : MonoCoreService<WaitSpinnerService>
{
    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);

    public static void Enable(object key, bool blockInput = true)
    {
        Instance._requests.Add(new Request() { key = key, blockInput = blockInput });

        Instance.OnRequestsChange();
    }

    public static void Disable(object key)
    {
        Instance._requests.RemoveFirst((x) => x.key == key);

        Instance.OnRequestsChange();
    }

    State EvaluateState()
    {
        if (_requests.Count == 0)
        {
            return State.Disabled;
        }
        else
        {
            for (int i = 0; i < Instance._requests.Count; i++)
            {
                if (_requests[i].blockInput)
                {
                    return State.ActiveAndBlockingInput;
                }
            }

            return State.Active;
        }
    }

    void OnRequestsChange()
    {
        State previousState = _state;
        _state = EvaluateState();

        if (_state != previousState)
        {
            if (_state == State.Disabled)
            {
                _sceneController.Deactivate();
            }
            else
            {
                _sceneController.Activate();
                _sceneController.BlockInput = (_state == State.ActiveAndBlockingInput);
            }
        }
    }


    struct Request
    {
        public object key;
        public bool blockInput;
    }

    enum State
    {
        Disabled,
        Active,
        ActiveAndBlockingInput
    }

    [SerializeField] WaitSpinnerSceneController _sceneController;
    List<Request> _requests = new List<Request>();
    State _state = State.Disabled;
}
