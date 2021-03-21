using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniGameClickTiming : SurveyBaseController
{
    public TextMeshPro NumberDisplay;
    public float TimeBetweenChanges = 0.5f;

    private int _currentNumber = 0;

    protected override List<GameAction.ParameterData> GetResult()
    {
        List<GameAction.ParameterData> results = new List<GameAction.ParameterData>();
        results.Add(new GameActionParameterSuccessRate.Data((MiniGameSuccessRate)_currentNumber));
        return results;
    }

    protected override string GetDebugResult()
    {
        return "Result : " + _currentNumber;
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (!_running)
                    {
                        StartSurvey(null);
                    }
                    else
                    {
                        Complete();
                    }
                }
            }
        }
    }

    protected override IEnumerator SurveyRoutine()
    {
        while (true)
        {
            _currentNumber++;

            if (_currentNumber > 5)
            {
                _currentNumber = 1;
            }

            NumberDisplay.text = _currentNumber.ToString();

            yield return new WaitForSeconds(TimeBetweenChanges);
        }
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
    }
}
