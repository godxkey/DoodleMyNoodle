using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class WaitSpinner
{
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

    static List<Request> _requests = new List<Request>();
    static State _state = State.Disabled;
    static WaitSpinnerSceneController _sceneController;

    public static void Enable(object key, bool blockInput = true)
    {
        _requests.Add(new Request() { key = key, blockInput = blockInput });

        OnRequestsChange();
    }

    public static void Disable(object key)
    {
        _requests.RemoveAll((x) => x.key == key);

        OnRequestsChange();
    }

    private static State EvaluateState()
    {
        if (_requests.Count == 0)
        {
            return State.Disabled;
        }
        else
        {
            for (int i = 0; i < _requests.Count; i++)
            {
                if (_requests[i].blockInput)
                {
                    return State.ActiveAndBlockingInput;
                }
            }

            return State.Active;
        }
    }

    private static void OnRequestsChange()
    {
        State previousState = _state;
        _state = EvaluateState();

        if (_state != previousState)
        {
            if(_state == State.Disabled)
            {
                SceneService.UnloadAsync(_sceneController.gameObject.scene);
                _sceneController = null;
            }
            else
            {
                if (_sceneController == null)
                {
                    SceneService.Load(WaitSpinnerSceneController.SCENE_NAME, LoadSceneMode.Additive, OnSceneLoaded);
                }

                if(_sceneController != null)
                {
                    _sceneController.BlockInput = (_state == State.ActiveAndBlockingInput);
                }
            }
        }
    }

    private static void OnSceneLoaded(Scene scene)
    {
        _sceneController = scene.FindRootObject<WaitSpinnerSceneController>();
        _sceneController.BlockInput = (_state == State.ActiveAndBlockingInput);
    }
}
