using System.Net;
using System;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using UnityEngine.Assertions;

public class JobifiedServerBehaviour : MonoBehaviour
{
    public int port = 9001;
    public int maxConnections = 16;

    [NonSerialized] UdpCNetworkDriver m_Driver;
    [NonSerialized] NativeList<NetworkConnection> m_Connections;
    [NonSerialized] JobHandle ServerJobHandle;

    void Start()
    {
        DebugService.Log("Server Start");

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
            DebugService.Log("Server Listening");
        }

        m_Connections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);
    }

    void OnDestroy()
    {
        ServerJobHandle.Complete();
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        ServerJobHandle.Complete();

        var updateConnectionsJob = new ServerUpdateConnectionsJob
        {
            driver = m_Driver,
            connections = m_Connections
        };

        var processEventsJob = new ServerProcessEventsJob
        {
            driver = m_Driver.ToConcurrent(),
            connections = m_Connections.AsDeferredJobArray()
        };

        ServerJobHandle = m_Driver.ScheduleUpdate();
        ServerJobHandle = updateConnectionsJob.Schedule(ServerJobHandle);
        ServerJobHandle = processEventsJob.Schedule(m_Connections, 1, ServerJobHandle);
    }
}

struct ServerUpdateConnectionsJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    public void Execute()
    {
        // Clean up connections
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = driver.Accept()) != default)
        {
            connections.Add(c);
            DebugService.Log("Accepted a connection");
        }
    }
}

struct ServerProcessEventsJob : IJobParallelFor
{
    public UdpCNetworkDriver.Concurrent driver;
    public NativeArray<NetworkConnection> connections;

    public void Execute(int index)
    {
        if (connections[index].IsCreated == false) // ignore non-created connections
            Assert.IsTrue(true);

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = driver.PopEventForConnection(connections[index], out stream)) != NetworkEvent.Type.Empty)
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
                    driver.Send(connections[index], writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                DebugService.Log("Client disconnected from server");
                connections[index] = default;
            }
        }
    }
}