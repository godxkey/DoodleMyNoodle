using System.Net;
using UnityEngine;

using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Jobs;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public enum LoggingFlag
{
    None = 0,
    SomethingWentWrongDuringConnect = 1,
    ConnectedToTheServer = 1 << 1,
    DisconnectedFromTheServer = 1 << 2,
    GotValueFromTheServer = 1 << 3
}

public struct LoggingData
{
    public LoggingFlag flags;
    public uint valueGotFromServer;
}

public class JobifiedClientBehaviour : MonoBehaviour
{
    public int port;

    UdpCNetworkDriver m_Driver;
    NativeArray<NetworkConnection> m_Connection;
    NativeArray<byte> m_Done;
    NativeArray<LoggingData> m_loggingData;
    JobHandle m_ClientJobHandle;

    // Start is called before the first frame update
    void Start()
    {
        DebugService.Log("Client Start");

        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        m_Connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        m_Done = new NativeArray<byte>(1, Allocator.Persistent);
        m_loggingData = new NativeArray<LoggingData>(1, Allocator.Persistent);


        IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);
        m_Connection[0] = m_Driver.Connect(endpoint);
    }

    void OnDestroy()
    {
        m_ClientJobHandle.Complete();

        m_Connection.Dispose();
        m_Driver.Dispose();
        m_Done.Dispose();
        m_loggingData.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        m_ClientJobHandle.Complete();

        ProcessLoggingData();

        var job = new ClientUpdateJob
        {
            driver = m_Driver,
            connection = m_Connection,
            done = m_Done,
            loggingData = m_loggingData
        };


        m_ClientJobHandle = m_Driver.ScheduleUpdate();
        m_ClientJobHandle = job.Schedule(m_ClientJobHandle);
    }

    private void ProcessLoggingData()
    {
        LoggingData loggingData = m_loggingData[0];
        if ((loggingData.flags & LoggingFlag.SomethingWentWrongDuringConnect) != 0)
        {
            DebugService.Log("Something went wrong during connect");
        }
        if ((loggingData.flags & LoggingFlag.ConnectedToTheServer) != 0)
        {
            DebugService.Log("We are now connected to the server");
        }
        if ((loggingData.flags & LoggingFlag.DisconnectedFromTheServer) != 0)
        {
            DebugService.Log("Client got disconnected from server");
        }
        if ((loggingData.flags & LoggingFlag.GotValueFromTheServer) != 0)
        {
            DebugService.Log("We got a value back from the server: " + m_loggingData[0].valueGotFromServer);
        }
        loggingData.flags = LoggingFlag.None;
        m_loggingData[0] = loggingData;
    }
}


struct ClientUpdateJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> done;
    public NativeArray<LoggingData> loggingData;

    public void Execute()
    {
        if (connection[0].IsCreated == false)
        {
            if (done[0] != 1) // done != true
            {
                TurnLogFlagOn(LoggingFlag.SomethingWentWrongDuringConnect);
            }
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = driver.PopEventForConnection(connection[0], out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                TurnLogFlagOn(LoggingFlag.ConnectedToTheServer);

                int value = 1;
                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    connection[0].Send(driver, writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                DataStreamReader.Context readerCtx = default;

                uint value = stream.ReadUInt(ref readerCtx);

                TurnLogFlagOn(LoggingFlag.GotValueFromTheServer);
                var temp = loggingData[0];
                temp.valueGotFromServer = value;
                loggingData[0] = temp;

                done[0] = 1; // true
                connection[0].Disconnect(driver);
                connection[0] = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                TurnLogFlagOn(LoggingFlag.DisconnectedFromTheServer);
                connection[0] = default;
            }
        }
    }

    void TurnLogFlagOn(LoggingFlag loggingFlag)
    {
        var temp = loggingData[0];
        temp.flags |= loggingFlag;
        loggingData[0] = temp;
    }
}