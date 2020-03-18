using SimulationControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelClientSimController : DebugPanel
{
    public override string title => "Sim Controller (Client)";
    public override bool canBeDisplayed =>
        GameMonoBehaviourHelpers.PresentationWorld?.GetExistingSystem<ReceiveSimulationTickSystem>() != null;

    float[] _simTickQueueLengths = new float[60];
    int _simTickQueueLengthsIterator = 0;
    float _averagedSimTickQueueLength = 0;

    float[] _simTickDropperSpeeds = new float[60];
    float _averagedSimTickDropperSpeed = 0;

    DirtyValue<uint> _currentSimTick;
    bool[] _offsettedSimTicks = new bool[60];
    int _offsettedSimTicksIterator = 0;
    int _totalOffsettedSimTicks;

    public override void OnGUI()
    {
        var receiveTickSystem = GameMonoBehaviourHelpers.PresentationWorld.GetExistingSystem<ReceiveSimulationTickSystem>();

        _simTickQueueLengths[_simTickQueueLengthsIterator] = receiveTickSystem.SimTicksInQueue;
        _simTickDropperSpeeds[_simTickQueueLengthsIterator] = receiveTickSystem.CurrentSimPlayingSpeed;
        _simTickQueueLengthsIterator++;
        _simTickQueueLengthsIterator %= _simTickQueueLengths.Length;

        if (_simTickQueueLengthsIterator == 0)
        {
            _averagedSimTickQueueLength = 0;
            for (int i = 0; i < _simTickQueueLengths.Length; i++)
            {
                _averagedSimTickQueueLength += _simTickQueueLengths[i];
            }
            _averagedSimTickQueueLength /= _simTickQueueLengths.Length;


            _averagedSimTickDropperSpeed = 0;
            for (int i = 0; i < _simTickDropperSpeeds.Length; i++)
            {
                _averagedSimTickDropperSpeed += _simTickDropperSpeeds[i];
            }
            _averagedSimTickDropperSpeed /= _simTickDropperSpeeds.Length;
        }

        GUILayout.Label($"Average SimTick queue length (over 60 frames): {_averagedSimTickQueueLength:F3}");
        GUILayout.Label($"Offsetted SimTicks (over 60 frames): {_totalOffsettedSimTicks}");
        GUILayout.Label($"SimTick Dropper Speed (over 60 frames): {_averagedSimTickDropperSpeed}");
    }

    void OnFixedUpdate()
    {
        if (!SimulationView.IsRunningOrReadyToRun)
            return;

        _currentSimTick.Set(SimulationView.TickId);

        // 'not dirty' means no change. That means the simulation has NOT played a sim tick this past fixed update
        _offsettedSimTicks[_offsettedSimTicksIterator] = !_currentSimTick.IsDirty;

        _offsettedSimTicksIterator++;
        _offsettedSimTicksIterator %= _offsettedSimTicks.Length;

        if (_offsettedSimTicksIterator == 0)
        {
            _totalOffsettedSimTicks = 0;
            for (int i = 0; i < _offsettedSimTicks.Length; i++)
            {
                if (_offsettedSimTicks[i])
                    _totalOffsettedSimTicks++;
            }
        }

        _currentSimTick.Reset();
    }



    public override void OnStartDisplay()
    {
        base.OnStartDisplay();

        UpdaterService.AddFixedUpdateCallback(OnFixedUpdate);
    }

    public override void OnStopDisplay()
    {
        base.OnStopDisplay();

        UpdaterService.RemoveFixedUpdateCallback(OnFixedUpdate);
    }
}
