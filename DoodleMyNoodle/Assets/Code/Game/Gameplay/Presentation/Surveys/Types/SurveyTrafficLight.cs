using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyTrafficLight : SurveyBaseController
{
    [System.Serializable]
    public struct LightData
    {
        public GameObject LightInstance;
        public Color LightColor;
        public float TimeUntilNextLight;
    }

    public Color InactiveColor;
    public GameObject Container;
    public List<LightData> LightDatas = new List<LightData>();

    private int _currentLight = 0;
    private int _resultLight = 0;
    private bool _buttonHeld = false;
    private bool _complete;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!_buttonHeld)
            {
                _buttonHeld = true;
            }
        }
        else
        {
            if (_buttonHeld)
            {
                _buttonHeld = false;
                Container.SetActive(false);
                _resultLight = _currentLight;
                _complete = true;
            }
        }
    }

    private void ChangeLightActiveState(int index, bool isActive)
    {
        LightData data = LightDatas[index];
        if (isActive)
        {
            data.LightInstance.GetComponent<SpriteRenderer>().color = data.LightColor;
        }
        else
        {
            data.LightInstance.GetComponent<SpriteRenderer>().color = InactiveColor;
        }
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        _complete = false;
    }

    protected override Action.ParameterDescriptionType[] GetExpectedQuery() => new Action.ParameterDescriptionType[]
    {
        Action.ParameterDescriptionType.SuccessRating,
    };

    protected override IEnumerator SurveyRoutine(Context context, List<Action.ParameterData> result, System.Action complete, System.Action cancel)
    {
        Container.SetActive(true);

        // Light Switching
        while (!_complete)
        {
            _currentLight = 0;
            while (_buttonHeld)
            {
                for (int i = 0; i < LightDatas.Count; i++)
                {
                    if (i == _currentLight)
                    {
                        ChangeLightActiveState(i, true);
                    }
                    else
                    {
                        ChangeLightActiveState(i, false);
                    }
                }

                yield return new WaitForSeconds(LightDatas[_currentLight].TimeUntilNextLight);

                _currentLight++;

                if (_currentLight >= LightDatas.Count)
                {
                    _currentLight = 0;
                }
            }

            yield return null;
        }

        int success = Mathf.Clamp(Mathf.CeilToInt((_resultLight + 1) * 5 / LightDatas.Count), 1, 5);
        result.Add(new GameActionParameterSuccessRate.Data((SurveySuccessRating)success));
        complete();
    }
}
