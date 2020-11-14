using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniGameClickTiming : MiniGameBaseController
{
    public TextMeshPro NumberDisplay;

    private int _currentNumber = 0;

    protected override GameActionParameterMiniGame.Data GetResult()
    {
        return new GameActionParameterMiniGame.Data((MiniGameDescriptionBase.SuccessRate)_currentNumber);
    }

    protected override string GetTextResult()
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
                    Complete();
                }
            }
        }
    }

    protected override IEnumerator MiniGameLoop()
    {
        _currentNumber++;

        if (_currentNumber > 5)
        {
            _currentNumber = 1;
        }

        NumberDisplay.text = _currentNumber.ToString();

        yield return new WaitForSeconds(GetMiniGameDescription<ClickTimingExampleDescription>().TimeBetweenChanges);

        yield return MiniGameLoop();
    }
}
