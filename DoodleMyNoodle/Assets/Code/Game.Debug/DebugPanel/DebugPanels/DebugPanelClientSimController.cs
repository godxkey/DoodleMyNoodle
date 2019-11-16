using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelClientSimController : DebugPanel
{
    public override string title => "Sim Controller (Client)";
    public override bool canBeDisplayed =>
        SimulationController.Instance != null &&
        SimulationController.Instance is SimulationControllerClient;

    float[] m_simTickQueueLengths = new float[60];
    int m_simTickQueueLengthsIterator = 0;
    float m_averagedSimTickQueueLength = 0;

    DirtyValue<uint> m_currentSimTick;
    bool[] m_offsettedSimTicks = new bool[60];
    int m_offsettedSimTicksIterator = 0;
    int m_totalOffsettedSimTicks;

    public override void OnGUI()
    {
        SimulationControllerClient simController = SimulationController.Instance as SimulationControllerClient;

        m_simTickQueueLengths[m_simTickQueueLengthsIterator] = simController.simTicksInQueue;
        m_simTickQueueLengthsIterator++;
        m_simTickQueueLengthsIterator %= m_simTickQueueLengths.Length;

        if (m_simTickQueueLengthsIterator == 0)
        {
            m_averagedSimTickQueueLength = 0;
            for (int i = 0; i < m_simTickQueueLengths.Length; i++)
            {
                m_averagedSimTickQueueLength += m_simTickQueueLengths[i];
            }
            m_averagedSimTickQueueLength /= m_simTickQueueLengths.Length;
        }

        GUILayout.Label($"Average SimTick queue length (over 60 frames): {m_averagedSimTickQueueLength:F3}");
        GUILayout.Label($"Offsetted SimTicks (over 60 frames): {m_totalOffsettedSimTicks}");
    }

    void OnFixedUpdate()
    {
        if (!SimulationView.IsRunningOrReadyToRun)
            return;

        m_currentSimTick.Value = SimulationView.TickId;

        // 'not dirty' means no change. That means the simulation has NOT played a sim tick this past fixed update
        m_offsettedSimTicks[m_offsettedSimTicksIterator] = !m_currentSimTick.IsDirty;

        m_offsettedSimTicksIterator++;
        m_offsettedSimTicksIterator %= m_offsettedSimTicks.Length;

        if (m_offsettedSimTicksIterator == 0)
        {
            m_totalOffsettedSimTicks = 0;
            for (int i = 0; i < m_offsettedSimTicks.Length; i++)
            {
                if (m_offsettedSimTicks[i])
                    m_totalOffsettedSimTicks++;
            }
        }

        m_currentSimTick.Reset();
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
