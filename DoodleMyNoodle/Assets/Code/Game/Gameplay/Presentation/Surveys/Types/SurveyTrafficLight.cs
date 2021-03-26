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
    public float DelayBeforeStartDuration;
    public float IntroDuration;

    public SpriteRenderer ContainerBG;
    public Color NotHeldBGColor;
    public Color HeldBGColor;

    public List<LightData> LightDatas = new List<LightData>();

    private int _currentLight = 0;
    private bool _buttonHeld = false;
    private bool _complete;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!_buttonHeld && Cache.PointedGameObjects.Contains(gameObject))
            {
                _buttonHeld = true;
                ContainerBG.color = HeldBGColor;
            }
        }
        else
        {
            if (_buttonHeld)
            {
                _buttonHeld = false;
                ContainerBG.color = NotHeldBGColor;
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

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.SuccessRating,
    };

    protected override IEnumerator SurveyRoutine(GameAction.ParameterDescription[] queryParams, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        // Intro

        for (int i = 0; i < LightDatas.Count; i++)
        {
            ChangeLightActiveState(i, false);
        }

        yield return new WaitForSeconds(DelayBeforeStartDuration);

        for (int i = 0; i < LightDatas.Count; i++)
        {
            ChangeLightActiveState(i, true);
        }

        yield return new WaitForSeconds(IntroDuration);

        for (int i = 0; i < LightDatas.Count; i++)
        {
            ChangeLightActiveState(i, false);
        }

        yield return new WaitForSeconds(IntroDuration);

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


        int success = Mathf.Clamp(Mathf.CeilToInt((_currentLight + 1) * 5 / LightDatas.Count), 1, 5);
        result.Add(new GameActionParameterSuccessRate.Data((SurveySuccessRating)success));
        complete();
    }
}
