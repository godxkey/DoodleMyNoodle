using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideAreaTiming : SurveyBaseController, IWorldUIPointerClickHandler
{
    ////////////////////////////////////////////////////////////////////////////////////////
    //      WARNING! THIS WAS NOT TESTED AFTER BEING PORTED TO NEW SurveyBaseController
    ////////////////////////////////////////////////////////////////////////////////////////

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

    private bool _hasClicked = false;
    private Sequence _circleAnim;

    protected override GameAction.ParameterDescriptionType[] GetExpectedQuery() => new GameAction.ParameterDescriptionType[]
    {
        GameAction.ParameterDescriptionType.SuccessRating
    };

    void IWorldUIPointerClickHandler.OnPointerClick()
    {
        _hasClicked = true;
    }

    protected override IEnumerator SurveyRoutine(GameAction.ParameterDescription[] queryParams, List<GameAction.ParameterData> result, Action complete, Action cancel)
    {
        _hasClicked = false;

        Circle.transform.localPosition = LeftCircleLimit.localPosition;

        Section.localPosition = new Vector3(UnityEngine.Random.Range(SectionLeftRandomLimit.localPosition.x, SectionRightRandomLimit.localPosition.x), 0, 0);
        Section.localScale = new Vector3(UnityEngine.Random.Range(SectionSizeMin, SectionSizeMax), 1, 1);


        _circleAnim = DOTween.Sequence();

        _circleAnim.Append(Circle.transform.DOLocalMoveX(RightCircleLimit.transform.localPosition.x, CircleSpeed).SetEase(Ease.Linear));
        _circleAnim.SetLoops(-1, LoopType.Yoyo);

        while (!_hasClicked)
        {
            yield return null;
        }

        _circleAnim.Kill();

        if ((Circle.transform.position.x < RightSectionLimit.transform.position.x) && (Circle.transform.position.x > LeftSectionLimit.transform.position.x))
        {
            // Success
            result.Add(new GameActionParameterSuccessRate.Data(SurveySuccessRating.Five));
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
