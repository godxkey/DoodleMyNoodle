//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public struct EventAData
//{
//    public string Message;
//}
//public struct EventBData
//{
//    public string Message;
//}

//public class SimTestScriptComponent : SimEventComponent, ISimInputProcessor, ISimEventListener<EventAData>
//{
//    public bool Master = true;

//    public SimEvent<EventAData> EventA;
//    public SimEvent<EventBData> EventB;

//    SimTestScriptComponent _a;
//    SimTestScriptComponent _b;

//    public override void OnSimAwake()
//    {
//        base.OnSimAwake();

//        EventA = CreateLocalEvent<EventAData>();
//        EventB = CreateLocalEvent<EventBData>();
//    }

//    public void OnEventRaised(in EventAData eventData)
//    {
//        Debug.Log("On event raised: " + eventData.Message);
//    }

//    public void ProcessInput(SimInput input)
//    {
//        if (!Master)
//            return;

//        if (input is SimInputKeycode keycode && keycode.state == SimInputKeycode.State.Pressed)
//        {
//            switch (keycode.keyCode)
//            {
//                case KeyCode.A:
//                    Debug.Log("Invoke A's event");

//                    _a.EventA.Raise(new EventAData() { Message = "A's event" });
//                    break;

//                case KeyCode.B:
//                    Debug.Log("Invoke B's event");

//                    _b.EventA.Raise(new EventAData() { Message = "B's event" });
//                    break;

//                case KeyCode.S:
//                    if (_b.EventA.IsListenerRegistered(_a))
//                    {
//                        Debug.Log("Register A to B's event");
//                        _b.EventA.UnregisterListener(_a);
//                    }
//                    else
//                    {
//                        Debug.Log("Unregister A from B's event");
//                        _b.EventA.RegisterListener(_a);
//                    }
//                    break;

//                case KeyCode.N:
//                    if (_a.EventA.IsListenerRegistered(_b))
//                    {
//                        Debug.Log("Unregister B from A's event");
//                        _a.EventA.UnregisterListener(_b);
//                    }
//                    else
//                    {
//                        Debug.Log("Register B to A's event");
//                        _a.EventA.RegisterListener(_b);
//                    }
//                    break;

//                case KeyCode.Q:
//                    if (_a == null)
//                    {
//                        Debug.Log("Spawn A");
//                        _a = Simulation.Instantiate(SimEntity).GetComponent<SimTestScriptComponent>();
//                        _a.Master = false;
//                    }
//                    else
//                    {
//                        Debug.Log("Destroy A");
//                        Simulation.Destroy(_a.SimEntity);
//                        _a = null;
//                    }
//                    break;

//                case KeyCode.G:
//                    if (_b == null)
//                    {
//                        Debug.Log("Spawn B");
//                        _b = Simulation.Instantiate(SimEntity).GetComponent<SimTestScriptComponent>();
//                        _b.Master = false;
//                    }
//                    else
//                    {
//                        Debug.Log("Destroy B");
//                        Simulation.Destroy(_b.SimEntity);
//                        _b = null;
//                    }
//                    break;
//            }
//        }
//    }
//}
