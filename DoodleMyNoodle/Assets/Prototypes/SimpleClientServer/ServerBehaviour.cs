using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class ServerBehaviour : MonoBehaviour
{
    public int port = 9001;
    public int maxConnections = 16;

    public UdpCNetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    void Start()
    {
        Application.targetFrameRate = 60;

        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);

        IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
        if (m_Driver.Bind(endpoint) != 0)
        {
            DebugService.Log("Failed to bind to port " + port);
        }
        else
        {
            m_Driver.Listen();
        }

        m_Connections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);
    }

    // Update is called once per frame
    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (m_Connections[i].IsCreated == false)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default)
        {
            m_Connections.Add(c);
            DebugService.Log("Accepted a connection");
        }

        // Look at events
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (m_Connections[i].IsCreated == false) // ignore non-created connections
                continue;

            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    DataStreamReader.Context readerCtx = default;

                    uint number = stream.ReadUInt(ref readerCtx);
                    DebugService.Log("Got " + number + " from the Client adding + 2 to it.");
                    number += 2;

                    using (DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp))
                    {
                        writer.Write(number);
                        m_Driver.Send(m_Connections[i], writer);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    DebugService.Log("Client disconnected from server");
                    m_Connections[i] = default;
                }
            }
        }
    }

    void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }
}
