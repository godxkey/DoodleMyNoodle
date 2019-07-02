using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelClientSimController : DebugPanel
{
    public override string Title => "Sim Controller (Client)";
    public override bool CanBeDisplayed =>
        SimulationController.instance != null &&
        SimulationController.instance is SimulationControllerClient;

    float[] m_simTickQueueLengths = new float[60];
    int m_iterator = 0;
    float m_averagedSimTickQueueLength = 0;

    public override void OnGUI()
    {
        SimulationControllerClient simController = SimulationController.instance as SimulationControllerClient;

        m_simTickQueueLengths[m_iterator] = simController.simTicksInQueue;
        m_iterator++;
        m_iterator %= m_simTickQueueLengths.Length;

        if(m_iterator == 0)
        {
            m_averagedSimTickQueueLength = 0;
            for (int i = 0; i < m_simTickQueueLengths.Length; i++)
            {
                m_averagedSimTickQueueLength += m_simTickQueueLengths[i];
            }
            m_averagedSimTickQueueLength /= m_simTickQueueLengths.Length;
        }

        GUILayout.Label($"Average SimTick queue length (over 60 frames): {m_averagedSimTickQueueLength:F3}");
    }
}
