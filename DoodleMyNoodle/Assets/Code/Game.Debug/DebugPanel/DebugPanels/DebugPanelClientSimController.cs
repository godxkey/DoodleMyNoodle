using SimulationControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugPanelClientSimController : DebugPanel
{
    public override string Title => "Sim Controller (Client)";
    public override bool CanBeDisplayed =>
        PresentationHelpers.PresentationWorld?.GetExistingSystem<ReceiveSimulationTickSystem>() != null;

    float[] _simTickQueueLengths = new float[60];
    int _simTickQueueLengthsIterator = 0;
    float _simTickQueueLengthAverage = 0;

    float[] _simTickDropperSpeeds = new float[60];
    float _simTickDropperSpeedAverage = 0;

    float[] _simTickQueueMaxDurations = new float[60];
    float _simTickQueueMaxDurationAverage = 0;

    DirtyValue<uint> _currentSimTick;
    bool[] _offsettedSimTicks = new bool[60];
    int _offsettedSimTicksIterator = 0;
    int _totalOffsettedSimTicks;

    public override void OnGUI()
    {
        var receiveTickSystem = PresentationHelpers.PresentationWorld.GetExistingSystem<ReceiveSimulationTickSystem>();

        _simTickQueueLengths[_simTickQueueLengthsIterator] = receiveTickSystem.SimTicksInQueue;
        _simTickDropperSpeeds[_simTickQueueLengthsIterator] = receiveTickSystem.CurrentSimPlayingSpeed;
        _simTickQueueMaxDurations[_simTickQueueLengthsIterator] = receiveTickSystem.SimTickQueueMaxDuration;
        _simTickQueueLengthsIterator++;
        _simTickQueueLengthsIterator %= _simTickQueueLengths.Length;

        if (_simTickQueueLengthsIterator == 0)
        {
            _simTickQueueLengthAverage = average(_simTickQueueLengths);
            _simTickDropperSpeedAverage = average(_simTickDropperSpeeds);
            _simTickQueueMaxDurationAverage = average(_simTickQueueMaxDurations);
        }

        GUILayout.Label($"Average SimTick queue length (over 60 frames): {_simTickQueueLengthAverage:F3}");
        GUILayout.Label($"SimTick queue max duration (over 60 frames): {_simTickQueueMaxDurationAverage:F4}");
        GUILayout.Label($"SimTick Dropper Speed (over 60 frames): {_simTickDropperSpeedAverage}");

        float average(float[] array)
        {
            float result = 0;
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i];
            }
            result /= array.Length;
            return result;
        }
    }

    void OnFixedUpdate()
    {
        if (!GamePresentationCache.Instance.Ready)
            return;

        SimulationWorld simWorld = (SimulationWorld)GamePresentationCache.Instance.SimWorld.EntityManager.World;

        _currentSimTick.Set(simWorld.LatestTickId);

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

        _currentSimTick.ClearDirty();
    }



    public override void OnStartDisplay()
    {
        base.OnStartDisplay();

        Updater.FixedUpdate += OnFixedUpdate;
    }

    public override void OnStopDisplay()
    {
        base.OnStopDisplay();

        Updater.FixedUpdate -= OnFixedUpdate;
    }
}
