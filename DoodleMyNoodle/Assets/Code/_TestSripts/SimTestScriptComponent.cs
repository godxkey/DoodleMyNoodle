using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EventAData
{
    public string message;
}
public struct EventBData
{
    public string message;
}

public class SimTestScriptComponent : SimEventComponent, ISimInputProcessor, ISimEventListener<EventAData>
{
    public bool master = true;

    public SimEvent<EventAData> EventA;
    public SimEvent<EventBData> EventB;

    SimTestScriptComponent a;
    SimTestScriptComponent b;

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        EventA = CreateLocalEvent<EventAData>();
        EventB = CreateLocalEvent<EventBData>();
    }

    public void OnEventRaised(in EventAData eventData)
    {
        Debug.Log("On event raised: " + eventData.message);
    }

    public void ProcessInput(SimInput input)
    {
        if (!master)
            return;

        if (input is SimInputKeycode keycode && keycode.state == SimInputKeycode.State.Pressed)
        {
            switch (keycode.keyCode)
            {
                case KeyCode.A:
                    Debug.Log("Invoke A's event");

                    a.EventA.Raise(new EventAData() { message = "A's event" });
                    break;

                case KeyCode.B:
                    Debug.Log("Invoke B's event");

                    b.EventA.Raise(new EventAData() { message = "B's event" });
                    break;

                case KeyCode.S:
                    if (b.EventA.IsListenerRegistered(a))
                    {
                        Debug.Log("Register A to B's event");
                        b.EventA.UnregisterListener(a);
                    }
                    else
                    {
                        Debug.Log("Unregister A from B's event");
                        b.EventA.RegisterListener(a);
                    }
                    break;

                case KeyCode.N:
                    if (a.EventA.IsListenerRegistered(b))
                    {
                        Debug.Log("Unregister B from A's event");
                        a.EventA.UnregisterListener(b);
                    }
                    else
                    {
                        Debug.Log("Register B to A's event");
                        a.EventA.RegisterListener(b);
                    }
                    break;

                case KeyCode.Q:
                    if (a == null)
                    {
                        Debug.Log("Spawn A");
                        a = Simulation.Instantiate(SimEntity).GetComponent<SimTestScriptComponent>();
                        a.master = false;
                    }
                    else
                    {
                        Debug.Log("Destroy A");
                        Simulation.Destroy(a.SimEntity);
                        a = null;
                    }
                    break;

                case KeyCode.G:
                    if (b == null)
                    {
                        Debug.Log("Spawn B");
                        b = Simulation.Instantiate(SimEntity).GetComponent<SimTestScriptComponent>();
                        b.master = false;
                    }
                    else
                    {
                        Debug.Log("Destroy B");
                        Simulation.Destroy(b.SimEntity);
                        b = null;
                    }
                    break;
            }
        }
    }
}
