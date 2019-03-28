using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommunicationInterface
{
    void RegisterChannelListenner();
    void OnReceive(CommunicationMessage message);
}