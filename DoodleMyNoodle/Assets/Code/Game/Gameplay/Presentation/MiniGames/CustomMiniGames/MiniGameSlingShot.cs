
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameSlingShot : SurveyBaseController
{
    // Feedback
    public Shader LineShader;
    private GameObject _currentLine = null;
    private GameObject _currentResultLine = null;
    public GameObject DraggingBall;

    // Settings
    public int speedMultiplier = 1;

    // Cache
    private bool _dragging = false;

    // Results
    private fix2 _vector;

    protected override List<GameAction.ParameterData> GetResult()
    {
        List<GameAction.ParameterData> results = new List<GameAction.ParameterData>();
        results.Add(new GameActionParameterVector.Data(_vector));
        return results;
    }

    protected override string GetDebugResult()
    {
        return "Direction : " + _vector;
    }

    protected override void OnUpdate()
    {
        if (Input.GetMouseButton(0) && !_dragging)
        {
            Vector3 mousePos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    _dragging = true;

                    if (!_running)
                    {
                        if (DebugMode && !InitOnStart)
                        {
                            if (_currentResultLine != null)
                            {
                                Destroy(_currentResultLine);
                            }

                            StartSurvey(null); // manually starting the survey for debug
                        }
                    }
                }
            }
        }
        else if (!Input.GetMouseButton(0) && _dragging)
        {
            _dragging = false;

            if (_currentLine != null)
            {
                Destroy(_currentLine);
                if (!DebugMode)
                {
                    Destroy(_currentResultLine);
                }
                
                DraggingBall.transform.position = transform.position;
            }
            
            Complete();
        }
    }

    protected override IEnumerator SurveyRoutine()
    {
        while (_running)
        {
            if (_dragging)
            {
                if (_currentLine != null)
                {
                    Destroy(_currentLine);
                }

                if (_currentResultLine != null)
                {
                    Destroy(_currentResultLine);
                }

                Vector3 mousePos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
                DraggingBall.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
                _currentLine = MiniGameUtility.DrawLine(new Vector3(mousePos.x, mousePos.y, 0), transform.position, Color.grey, -1, 0.1f, LineShader);

                Vector3 DirectionnalVector = transform.position - new Vector3(mousePos.x, mousePos.y, 0);
                Vector3 EndPreviewPosition = transform.position + DirectionnalVector;

                _currentResultLine = MiniGameUtility.DrawLine(transform.position, EndPreviewPosition, Color.blue, -1, 0.1f, LineShader);

                _vector = new fix2((fix)DirectionnalVector.x, (fix)DirectionnalVector.y) * speedMultiplier;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
    }
}
