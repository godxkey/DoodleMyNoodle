using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class ClientBehaviour : MonoBehaviour
{
    public int port;

    UdpCNetworkDriver m_Driver;
    NetworkConnection m_Connection;
    bool Done;

    void Start()
    {
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        m_Connection = default;

        IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);
        m_Connection = m_Driver.Connect(endpoint);
    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (m_Connection.IsCreated == false)
        {
            if (Done != true)
            {
                DebugService.Log("Something went wrong during connect");
            }
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = m_Driver.PopEventForConnection(m_Connection, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                DebugService.Log("We are now connected to the server");

                int value = 1;
                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    m_Connection.Send(m_Driver, writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                DataStreamReader.Context readerCtx = default;
                uint value = stream.ReadUInt(ref readerCtx);
                DebugService.Log("Got the value = " + value + " back from the server");
                Done = true;
                m_Connection.Disconnect(m_Driver);
                m_Connection = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                DebugService.Log("Client got disconnected from server");
                m_Connection = default;
            }
        }
    }

    void OnDestroy()
    {
        m_Driver.Dispose();
    }
}