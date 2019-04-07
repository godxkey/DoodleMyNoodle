using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommunicationInterface
{
    void OnReceive(CommunicationMessage message);
}