using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Awake()
    {
        if (OnlineService.onlineInterface != null && OnlineService.onlineInterface.sessionInterface != null)
            OnlineService.onlineInterface.sessionInterface.RegisterNetMessageReceiver<NetMessageExample>(OnNetMessageReceived);
    }

    void OnDestroy()
    {
        if (OnlineService.onlineInterface != null && OnlineService.onlineInterface.sessionInterface != null)
            OnlineService.onlineInterface.sessionInterface.UnregisterNetMessageReceiver<NetMessageExample>(OnNetMessageReceived);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SessionInterface sessionInterface = OnlineService.onlineInterface.sessionInterface;

        //    NetMessageExample netMessageExample = new NetMessageExample();

        //    netMessageExample.valueString = "test"; // 8 + 2
        //    netMessageExample.valueInt = -10;       // 4
        //    netMessageExample.valueUInt = 50;       // 4
        //    netMessageExample.valueShort = -56;     // 2
        //    netMessageExample.valueUShort = 33;     // 2
        //    netMessageExample.valueBool = true;     // 0.2
        //    netMessageExample.valueByte = 12;       // 1
        //    netMessageExample.listOnInts = new int[5];
        //    netMessageExample.listOnInts[0] = 5;
        //    netMessageExample.listOnInts[1] = 4;
        //    netMessageExample.listOnInts[2] = 3;
        //    netMessageExample.listOnInts[3] = 2;
        //    netMessageExample.listOnInts[4] = 1;

        //    sessionInterface.SendNetMessage(netMessageExample, sessionInterface.connections[0]);


        //    DebugScreenMessage.DisplayMessage("Message sent");
        //    DebugService.Log("Message sent");
        //}
    }

    void OnNetMessageReceived(NetMessageExample netMessageExample, INetworkInterfaceConnection source)
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(netMessageExample.valueString);
        stringBuilder.AppendLine(netMessageExample.valueInt.ToString());
        stringBuilder.AppendLine(netMessageExample.valueUInt.ToString());
        stringBuilder.AppendLine(netMessageExample.valueShort.ToString());
        stringBuilder.AppendLine(netMessageExample.valueUShort.ToString());
        stringBuilder.AppendLine(netMessageExample.valueBool.ToString());
        stringBuilder.AppendLine(netMessageExample.valueByte.ToString());

        foreach (var item in netMessageExample.listOnInts)
        {
            stringBuilder.AppendLine(item.ToString());
        }

        DebugScreenMessage.DisplayMessage(stringBuilder.ToString());
        DebugService.Log(stringBuilder.ToString());
    }
}
