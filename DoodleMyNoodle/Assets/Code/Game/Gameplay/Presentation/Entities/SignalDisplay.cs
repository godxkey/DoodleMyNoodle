using System;
using UnityEngine;
using UnityEngineX;

public class SignalDisplay : BindedPresentationEntityComponent
{
    public GameObject VisualsOn;
    public GameObject VisualsOff;
    public float MinOnDuration = 0.1f;

    private ClickableDisplay _emitterClicker;
    private bool _isOn;
    private bool _wasOn; // we use this to make sure we display the ON for a small duration if signal was turned OFF before we could reach the presentation update
    private float _onTimer;
    private DirtyValue<bool> _visuallyOn;

    protected override void Awake()
    {
        base.Awake();

        _emitterClicker = GetComponent<ClickableDisplay>();
    }

    public override void OnPostSimulationTick()
    {
        base.OnPostSimulationTick();

        _isOn = SimWorld.TryGetComponentData(SimEntity, out Signal signal) && signal;
        _wasOn |= _isOn;
    }

    protected override void OnGamePresentationUpdate()
    {
        _visuallyOn.Set(_isOn || (_wasOn && _onTimer < MinOnDuration) || (_visuallyOn && _onTimer < MinOnDuration));
        _wasOn = false;

        if (_visuallyOn)
        {
            _onTimer += Time.deltaTime;
        }
        else
        {
            _onTimer = 0;
        }

        if (_visuallyOn.ClearDirty())
        {
            VisualsOn.SetActive(_visuallyOn);
            VisualsOff.SetActive(!_visuallyOn);

            // update emitter click sprite if needed
            if (_emitterClicker != null)
                _emitterClicker.SetSprite(_visuallyOn ? VisualsOn.GetComponentInChildren<SpriteRenderer>() : VisualsOff.GetComponentInChildren<SpriteRenderer>());
        }
    }
}