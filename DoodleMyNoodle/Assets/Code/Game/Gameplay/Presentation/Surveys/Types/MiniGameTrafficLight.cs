using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameTrafficLight : SurveyBaseController
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

    protected override List<GameAction.ParameterData> GetResult()
    {
        List<GameAction.ParameterData> results = new List<GameAction.ParameterData>();
        int success = Mathf.Clamp(Mathf.CeilToInt(((_currentLight + 1) * 5) / LightDatas.Count), 1, 5);
        results.Add(new GameActionParameterSuccessRate.Data((MiniGameSuccessRate)success));
        return results;
    }

    protected override string GetDebugResult()
    {
        return "Result : " + LightDatas[_currentLight].LightInstance.name;
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButton(0) && !_buttonHeld)
        {
            Vector3 mousePos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    _buttonHeld = true;
                    ContainerBG.color = HeldBGColor;

                    if ((DebugMode && !InitOnStart) || !_running)
                    {
                        StartSurvey(null);
                    }
                }
            }
        }
        else if (!Input.GetMouseButton(0) && _buttonHeld)
        {
            _buttonHeld = false;
            ContainerBG.color = NotHeldBGColor;
            Complete();
        }
    }

    protected override IEnumerator SurveyRoutine()
    {
        // Intro

        for (int i = 0; i < LightDatas.Count; i++)
        {
            ChangeLightActiveState(i, false);
        }

        yield return new WaitForSeconds(DelayBeforeStartDuration);
        
        for (int i = 0; i < LightDatas.Count; i++)
        {
            ChangeLightActiveState(i,true);
        }

        yield return new WaitForSeconds(IntroDuration);

        for (int i = 0; i < LightDatas.Count; i++)
        {
            ChangeLightActiveState(i, false);
        }

        yield return new WaitForSeconds(IntroDuration);

        // Light Switching
        while (_running)
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

            yield return new WaitForEndOfFrame();
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
    }
}
