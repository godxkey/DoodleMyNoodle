using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideAreaTiming : SurveyBaseController
{
    public Transform LeftCircleLimit;
    public Transform RightCircleLimit;

    public Transform Section;
    public Transform LeftSectionLimit;
    public Transform RightSectionLimit;
    public Transform SectionLeftRandomLimit;
    public Transform SectionRightRandomLimit;
    public float SectionSizeMin;
    public float SectionSizeMax;

    public GameObject Circle;
    public float CircleSpeed = 1;

    public GameObject ClickButton;

    private bool _inProgress = false;
    private bool _hasClicked = false;

    private bool _resultSuccess = false;

    protected override List<GameAction.ParameterData> GetResult()
    {
        List<GameAction.ParameterData> results = new List<GameAction.ParameterData>();

        if (_resultSuccess)
        {
            // Success
            results.Add(new GameActionParameterSuccessRate.Data(SurveySuccessRating.Five));
        }
        else
        {
            // Failed
            results.Add(new GameActionParameterSuccessRate.Data(SurveySuccessRating.One));
        }

        return results;
    }

    protected override string GetDebugResult()
    {
        string result = _resultSuccess ? "Success" : "Failure";
        return "Result : " + result;
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
                if (hit.collider.gameObject == ClickButton)
                {
                    if (!_inProgress)
                    {
                        StartSurvey(null);
                    }
                    else
                    {
                        _hasClicked = true;
                    }
                }
            }
        }
    }

    protected override IEnumerator SurveyRoutine()
    {
        _inProgress = true;

        Circle.transform.localPosition = LeftCircleLimit.localPosition;

        Section.localPosition = new Vector3(Random.Range(SectionLeftRandomLimit.localPosition.x, SectionRightRandomLimit.localPosition.x), 0, 0);
        Section.localScale = new Vector3(Random.Range(SectionSizeMin, SectionSizeMax), 1, 1);


        Sequence circleAnim = DOTween.Sequence();

        circleAnim.Append(Circle.transform.DOLocalMoveX(RightCircleLimit.transform.localPosition.x, CircleSpeed).SetEase(Ease.Linear));
        circleAnim.SetLoops(-1, LoopType.Yoyo);

        while (true)
        {
            if (_hasClicked)
            {
                _hasClicked = false;

                circleAnim.Kill();

                _inProgress = false;

                if ((Circle.transform.position.x < RightSectionLimit.transform.position.x) && (Circle.transform.position.x > LeftSectionLimit.transform.position.x))
                {
                    // Success
                    _resultSuccess = true;
                }
                else
                {
                    // Failed
                    _resultSuccess = false;
                }

                Complete();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
    }
}
