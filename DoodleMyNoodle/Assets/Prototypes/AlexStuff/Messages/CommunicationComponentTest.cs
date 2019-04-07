using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationComponentTest : MonoBehaviour, ICommunicationInterface
{
    public bool onChannelOne = false;

    void Start()
    {
        if (onChannelOne)
        {
            CommunicationCenter.Instance.Listen(this, CommunicationChannel.ExempleChannelOne);
        }
        else
        {
            CommunicationCenter.Instance.Listen(this, CommunicationChannel.ExempleChannelTwo);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (onChannelOne)
            {
                CommunicationCenter.Instance.SendInformations(CommunicationChannel.ExempleChannelTwo,"hello");
            }
            else
            {
                CommunicationCenter.Instance.SendInformations(CommunicationChannel.ExempleChannelOne, "hi");
            }
        }
    }

    public void OnReceive(CommunicationMessage message)
    {
        Debug.Log((string)message.GetAllInformations()[0]);
    }
}
