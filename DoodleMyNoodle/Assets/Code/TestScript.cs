using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Awake()
    {
        OnlineService.onlineInterface.sessionInterface.RegisterNetMessageReceiver<NetMessageExample>(OnNetMessageReceived);
    }

    void OnDestroy()
    {
        if(OnlineService.onlineInterface != null && OnlineService.onlineInterface.sessionInterface != null)
            OnlineService.onlineInterface.sessionInterface.UnregisterNetMessageReceiver<NetMessageExample>(OnNetMessageReceived);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SessionInterface sessionInterface = OnlineService.onlineInterface.sessionInterface;

            NetMessageExample netMessageExample = new NetMessageExample();

            netMessageExample.valueString = "test"; // 8 + 2
            netMessageExample.valueInt = -10;       // 4
            netMessageExample.valueUInt = 50;       // 4
            netMessageExample.valueShort = -56;     // 2
            netMessageExample.valueUShort = 33;     // 2
            netMessageExample.valueBool = true;     // 0.2
            netMessageExample.valueByte = 12;       // 1
            netMessageExample.listOfNetSerializableValue.Add(5);
            netMessageExample.listOfNetSerializableValue.Add(4);
            netMessageExample.listOfNetSerializableValue.Add(3);
            netMessageExample.listOfNetSerializableValue.Add(2);
            netMessageExample.listOfNetSerializableValue.Add(1);

            sessionInterface.SendNetMessage(sessionInterface.connections[0], netMessageExample);


            DebugScreenMessage.DisplayMessage("Message sent");
            DebugService.Log("Message sent");
        }
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

        foreach (var item in netMessageExample.listOfNetSerializableValue)
        {
            stringBuilder.AppendLine(item.ToString());
        }

        DebugScreenMessage.DisplayMessage(stringBuilder.ToString());
        DebugService.Log(stringBuilder.ToString());
    }
}
