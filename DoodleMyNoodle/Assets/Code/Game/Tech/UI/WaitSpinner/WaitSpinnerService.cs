using Internals.WaitSpinnerService;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitSpinnerService : MonoCoreService<WaitSpinnerService>
{
    public override void Initialize(System.Action<ICoreService> onComplete) => onComplete(this);

    /// <summary>
    /// If the key already exists, calling this method will not do anything 
    /// </summary>
    public static void Enable(object key, bool blockInput = true)
    {
        if (Instance.GetIndexOfKey(key) == -1)
        {
            Instance._requests.Add(new Request() { key = key, blockInput = blockInput });
            Instance.OnRequestsChange();
        }
    }

    public static void Disable(object key)
    {
        int keyIndex = Instance.GetIndexOfKey(key);
        if (keyIndex != -1)
        {
            Instance._requests.RemoveAt(keyIndex);
            Instance.OnRequestsChange();
        }
    }

    int GetIndexOfKey(object key)
    {
        for (int i = 0; i < _requests.Count; i++)
        {
            if (_requests[i].key == key)
            {
                return i;
            }
        }

        return -1;
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
