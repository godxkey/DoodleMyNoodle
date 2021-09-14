using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyZoneBar : SurveyBaseController
{
    [Header("Moving Circle")]
    public Transform LeftCircleLimit;
    public Transform RightCircleLimit;

    public GameObject Circle;
    public float CircleSpeed = 1;

    [Header("Sections")]
    public Transform Section;
    public Transform LeftSectionLimit;
    public Transform RightSectionLimit;
    public Transform SectionLeftRandomLimit;
    public Transform SectionRightRandomLimit;
    public float SectionSizeAverageMax;
    public float SectionSizeFailedMax;
    public float SectionSizeMax;
    public float SectionGrowthSpeed = 0.05f;

    [Header("Colors")]
    public SpriteRenderer SectionSpriteRenderer;
    public Color SuccessColor;
    public Color AverageColor;
    public Color FailedColor;

    [Header("Button")]
    public GameObject ClickButton;

    private bool _hasClicked = false;
    private Sequence _circleAnim;
    private SurveySuccessRating _currentSuccessRate = SurveySuccessRating.Five;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.SuccessRating
    };

    public override GameAction.ParameterDescription[] CreateDebugQuery()
    {
        return new GameAction.ParameterDescription[] { new GameActionParameterSuccessRate.Description() {  } };
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!_hasClicked)
            {
                _hasClicked = true;
            }
        }
        else
        {
            if (_hasClicked)
            {
                _hasClicked = false;
            }
        }
    }

    protected override IEnumerator SurveyRoutine(Context context, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        _hasClicked = false;

        Circle.transform.localPosition = LeftCircleLimit.localPosition;

        Section.localPosition = new Vector3(UnityEngine.Random.Range(SectionLeftRandomLimit.localPosition.x, SectionRightRandomLimit.localPosition.x), 0, 0);
        Section.localScale = new Vector3(0, 1, 1);

        _circleAnim = DOTween.Sequence();
        _circleAnim.Append(Circle.transform.DOLocalMoveX(RightCircleLimit.transform.localPosition.x, CircleSpeed).SetEase(Ease.Linear));
        _circleAnim.SetLoops(-1, LoopType.Yoyo);

        SectionSpriteRenderer.color = SuccessColor;

        while (!_hasClicked)
        {
            Section.localScale = new Vector3(Mathf.Min(Section.localScale.x + (Time.deltaTime * SectionGrowthSpeed), SectionSizeMax), 1, 1);
            if (Section.localScale.x > SectionSizeFailedMax)
            {
                _currentSuccessRate = SurveySuccessRating.Three;
                SectionSpriteRenderer.color = FailedColor;
            }
            else if (Section.localScale.x > SectionSizeAverageMax)
            {
                _currentSuccessRate = SurveySuccessRating.Four;
                SectionSpriteRenderer.color = AverageColor;
            }
            
            yield return null;
        }

        _circleAnim.Kill();

        if ((Circle.transform.position.x < RightSectionLimit.transform.position.x) && (Circle.transform.position.x > LeftSectionLimit.transform.position.x))
        {
            // Success
            result.Add(new GameActionParameterSuccessRate.Data(_currentSuccessRate));
        }
        else
        {
            // Failed
            result.Add(new GameActionParameterSuccessRate.Data(SurveySuccessRating.One));
        }

        complete();
    }

    protected override void OnEndSurvey(bool wasCompleted)
    {
        _circleAnim?.Kill();
    }
}
