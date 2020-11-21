
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameSlingShot : SurveyBaseController
{
    public Shader LineShader;

    private bool _dragging = false;
    private GameObject _currentLine = null;
    private GameObject _currentResultLine = null;

    protected override List<GameAction.ParameterData> GetResult()
    {
        List<GameAction.ParameterData> results = new List<GameAction.ParameterData>();
        // TODO : Parameter Vector / Direction ?
        return results;
    }

    protected override string GetDebugResult()
    {
        return "";
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

                    if ((DebugMode && !InitOnStart) || _isComplete)
                    {
                        if (_currentResultLine != null)
                        {
                            Destroy(_currentResultLine);
                        }

                        StartSurvey(null);
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
            }
            
            Complete();
        }
    }

    protected override IEnumerator SurveyLoop()
    {
        while (!_isComplete)
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
                _currentLine = MiniGameUtility.DrawLine(new Vector3(mousePos.x, mousePos.y, 0), transform.position, Color.red, -1, 0.1f, LineShader);

                Vector3 DirectionnalVector = transform.position - new Vector3(mousePos.x, mousePos.y, 0);
                Vector3 EndPreviewPosition = transform.position + DirectionnalVector;

                _currentResultLine = MiniGameUtility.DrawLine(transform.position, EndPreviewPosition, Color.blue, -1, 0.1f, LineShader);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
