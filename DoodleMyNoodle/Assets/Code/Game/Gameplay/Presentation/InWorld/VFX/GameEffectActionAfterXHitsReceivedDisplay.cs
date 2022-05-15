using System;
using UnityEngine;
using UnityEngineX;
using UnityEngineX.InspectorDisplay;

public class GameEffectActionAfterXHitsReceivedDisplay : GameEffectDisplayBase
{
    [SerializeField] private GameObject[] _hitVisuals;
    [SerializeField, Suffix("seconds")] private float _hideIfFinishedAfter;

    private BindedSimEntityManaged _bindedSimEntity;
    private float _finishedTime = -1;

    protected override void Awake()
    {
        base.Awake();

        _bindedSimEntity = GetComponentInParent<BindedSimEntityManaged>();

        if (_bindedSimEntity == null)
            Log.Warning($"No {nameof(BindedSimEntityManaged)} component found on parent of {gameObject.name}. Prehaps the VFX is not attached to the presentation entity?");
    }

    public override void PresentationUpdate()
    {
        if (_bindedSimEntity == null)
            return;

        int displayedHits = 0;
        if (SimWorld.TryGetComponent(SimEffectEntity, out GameEffectActionAfterXHitsReceived.EffectData effectData))
        {
            displayedHits = effectData.HitCounter;

            if (effectData.HitCounter == effectData.RequiredHits && _finishedTime == -1)
            {
                _finishedTime = Time.deltaTime;
            }
        }

        if (_finishedTime != -1 && Time.time - _finishedTime > _hideIfFinishedAfter)
        {
            gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < _hitVisuals.Length; i++)
            {
                _hitVisuals[i].SetActive(i < displayedHits);
            }
        }
    }
}