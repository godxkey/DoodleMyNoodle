using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationCenter : MonoCoreService<CommunicationCenter>
{
    private List<Listeners> listeners;
    private List<IncommingTransmission> transmissions;

    public struct Listeners
    {
        public ICommunicationInterface listener;
        public CommunicationChannel channel;
    }

    public struct IncommingTransmission
    {
        public CommunicationMessage message;
        public CommunicationChannel channel;
    }

    public override void Initialize(Action<ICoreService> onComplete)
    {
        listeners = new List<Listeners>();
        transmissions = new List<IncommingTransmission>();

        onComplete(this);
    }

    // LOOP

    void Update()
    {
        for (int i = 0; i < transmissions.Count; i++)
        {
            for (int j = 0; j < listeners.Count; j++)
            {
                if(transmissions[i].channel == listeners[j].channel)
                {
                    listeners[j].listener.OnReceive(transmissions[i].message);
                }
            }
        }
        transmissions.Clear();
    }

    // SENDING

    public void Notify(CommunicationChannel channel)
    {
        transmissions.Add(MakeTransmission(channel));
    }

    public void SendMessage(CommunicationChannel channel, CommunicationMessage message)
    {
        transmissions.Add(MakeTransmission(channel, message));
    }

    public void SendInformations(CommunicationChannel channel, params object[] informations)
    {
        CommunicationMessage newMessage = new CommunicationMessage(informations);
        transmissions.Add(MakeTransmission(channel, newMessage));
    }

    private IncommingTransmission MakeTransmission(CommunicationChannel channel, CommunicationMessage message = null)
    {
        IncommingTransmission currentTransmission = new IncommingTransmission();
        currentTransmission.channel = channel;
        currentTransmission.message = message;
        return currentTransmission;
    }

    // LISTENNING

    public void Listen(ICommunicationInterface listener, CommunicationChannel channel)
    {
        if(listener != null)
        {
            Listeners newListener;
            newListener.listener = listener;
            newListener.channel = channel;

            listeners.Add(newListener);
        }
    }
}
