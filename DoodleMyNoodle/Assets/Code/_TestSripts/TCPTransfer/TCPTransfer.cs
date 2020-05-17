using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// State object for receiving data from remote device.  
public class StateObject
{
    // Client socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 256;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder str = new StringBuilder();
}

public interface IUpdate
{
    void Update();
}

public interface IPooPoo : IUpdate, IDisposable { }

public class TCPTransfer : MonoBehaviour
{
    public const int PORT = 11000;
    public const string HOST_NAME = "Fred-PC";

    public Toggle IsClient;
    public TextMeshProUGUI Text;
    public Toggle StartListening;
    public Toggle ConnectAndSend;

    private IPooPoo _tcpObject;

    private void Update()
    {
        if (IsClient.isOn && (_tcpObject == null || _tcpObject is TCPServer))
        {
            if (_tcpObject != null)
                _tcpObject.Dispose();
            _tcpObject = new TCPClient(this);
        }

        if (!IsClient.isOn && (_tcpObject == null || _tcpObject is TCPClient))
        {
            if (_tcpObject != null)
                _tcpObject.Dispose();
            _tcpObject = new TCPServer(this);
        }

        StartListening.gameObject.SetActive(_tcpObject is TCPServer);
        ConnectAndSend.gameObject.SetActive(_tcpObject is TCPClient);

        if (StartListening.isOn)
        {
            StartListening.isOn = false;
            StartCoroutine((_tcpObject as TCPServer).StartListening());
        }

        //if (ConnectAndSend.isOn)
        //{
        //    ConnectAndSend.isOn = false;
        //    (_tcpObject as TCPClient).ConnectAndSend();
        //}

        _tcpObject.Update();
    }

    public void WriteLine(string text)
    {
        if (ThreadUtility.IsMainThread)
            WriteLineNow(text);
        else
            MainThreadService.AddMainThreadCallbackFromThread(() => WriteLineNow(text));
    }


    void WriteLineNow(string text)
    {
        Text.text += "\n" + text;
    }
}

public class TCPClient : IPooPoo
{
    TCPTransfer _ui;

    public TCPClient(TCPTransfer ui)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    public void Update()
    {
    }

    public void Dispose()
    {
    }

    // ManualResetEvent instances signal completion.  
    private volatile bool _connectDone = false;
    private volatile bool _sendDone = false;
    private volatile bool _receiveDone = false;

    // The response from the remote device.  
    private string _response = string.Empty;

    private IEnumerator StartClient()
    {
        // Connect to a remote device.  
        // Establish the remote endpoint for the socket.  
        // The name of the
        // remote device is "host.contoso.com".  
        IPHostEntry ipHostInfo = Dns.GetHostEntry(TCPTransfer.HOST_NAME);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, TCPTransfer.PORT);

        // Create a TCP/IP socket.  
        Socket client = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        // Connect to the remote endpoint.  
        client.BeginConnect(remoteEP, ConnectCallback, client);
        while (!_connectDone) yield return null;

        // Send test data to the remote device.  
        Send(client, "This is a test<EOF>");
        while (!_sendDone) yield return null;

        // Receive the response from the remote device.  
        Receive(client);
        while (!_receiveDone) yield return null;

        // Write the response to the console.  
        _ui.WriteLine($"Response received : {_response}");

        // Release the socket.  
        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.  
            client.EndConnect(ar);

            _ui.WriteLine($"Socket connected to {client.RemoteEndPoint}");

            // Signal that the connection has been made.  
            _connectDone = true;
        }
        catch (Exception e)
        {
            _ui.WriteLine(e.ToString());
        }
    }

    private void Receive(Socket client)
    {
        try
        {
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.  
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            _ui.WriteLine(e.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There might be more data, so store the data received so far.  
                state.str.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Get the rest of the data.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            else
            {
                // All the data has arrived; put it in response.  
                if (state.str.Length > 1)
                {
                    _response = state.str.ToString();
                }
                // Signal that all bytes have been received.  
                _receiveDone = true;
            }
        }
        catch (Exception e)
        {
            _ui.WriteLine(e.ToString());
        }
    }

    private void Send(Socket client, String data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), client);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);
            _ui.WriteLine($"Sent {bytesSent} bytes to server.");

            // Signal that all bytes have been sent.  
            _sendDone = true;
        }
        catch (Exception e)
        {
            _ui.WriteLine(e.ToString());
        }
    }
}

public class TCPServer : IPooPoo
{
    // Incoming data from the client.  
    TCPTransfer _ui;
    Socket _socket;

    public TCPServer(TCPTransfer ui)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));

        // Establish the local endpoint for the socket.  
        // The DNS name of the computer  
        // running the listener is "host.contoso.com".  
        _ui.WriteLine($"StartListening() - host name: {Dns.GetHostName()}");

        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, TCPTransfer.PORT);

        // Create a TCP/IP socket.  
        _socket = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.  

        _socket.Bind(localEndPoint);
        _socket.Listen(100);
    }

    public void Update() { }

    public void Dispose()
    {
    }

    // Thread signal.  
    private volatile bool _allDone = false;

    public IEnumerator StartListening()
    {
        while (true)
        {
            // Set the event to nonsignaled state.
            _allDone = false;

            // Start an asynchronous socket to listen for connections.  
            _ui.WriteLine("Waiting for a connection...");
            _socket.BeginAccept(AcceptCallback, state: _socket);

            // Wait until a connection is made before continuing.  
            while (!_allDone)
            {
                yield return null;
            }
        }
    }

    public void AcceptCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.
        _allDone = true;

        // Get the socket that handles the client request.  
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        // Create the state object.  
        StateObject state = new StateObject();
        state.workSocket = handler;

        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, ReadCallback, state);
    }

    public void ReadCallback(IAsyncResult ar)
    {
        string content = string.Empty;

        // Retrieve the state object and the handler socket  
        // from the asynchronous state object.  
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

        // Read data from the client socket.
        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            // There  might be more data, so store the data received so far.  
            state.str.Append(Encoding.ASCII.GetString(
                state.buffer, 0, bytesRead));

            // Check for end-of-file tag. If it is not there, read
            // more data.  
            content = state.str.ToString();
            if (content.IndexOf("<EOF>") > -1)
            {
                // All the data has been read from the
                // client. Display it on the console.  
                _ui.WriteLine($"Read {content.Length} bytes from socket. \n Data : {content}");

                // Echo the data back to the client.  
                Send(handler, content);
            }
            else
            {
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
        }
    }

    private void Send(Socket handler, string data)
    {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = handler.EndSend(ar);
            _ui.WriteLine($"Sent {bytesSent} bytes to client.");

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }
        catch (Exception e)
        {
            _ui.WriteLine(e.ToString());
        }
    }
}
