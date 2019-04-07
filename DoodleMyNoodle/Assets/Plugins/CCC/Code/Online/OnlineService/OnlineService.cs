using System;
using UnityEngine;

public abstract class OnlineService : MonoCoreService<OnlineService>
{
    protected abstract NetworkInterface CreateNetworkInterface();
    protected abstract INetMessageFactoryImpl CreateNetMessageFactory();


    public static NetworkInterface networkInterface => Instance?._networkInterface;

    public static OnlineInterface onlineInterface => Instance?._onlineInterface;
    public static OnlineClientInterface clientInterface => onlineInterface as OnlineClientInterface;
    public static OnlineServerInterface serverInterface => onlineInterface as OnlineServerInterface;

    public static OnlineRole targetRole { get; private set; } = OnlineRole.None;
    public static OnlineRole currentRole
    {
        get
        {
            if (networkInterface.state != NetworkState.Running)
                return OnlineRole.None;

            if (networkInterface.isClient)
                return OnlineRole.Client;
            else
                return OnlineRole.Server;
        }
    }

    public static bool isChangingRole => targetRole != currentRole;

    public static void SetTargetRole(OnlineRole role)
    {
        if (targetRole != role)
        {
            targetRole = role;

            ProcessRoleChange();
        }
    }

    public override void Initialize(Action<ICoreService> onComplete)
    {
        _networkInterface = CreateNetworkInterface();
        NetMessageFactory.impl = CreateNetMessageFactory();

        _networkInterface.onShutdownBegin += OnNetworkInterfaceShutdownBegin;

        onComplete(this);
    }

    void OnNetworkInterfaceShutdownBegin()
    {
        if (ApplicationUtilityService.ApplicationIsQuitting == false)
        {
            // If we're exiting the application (or pressing stop in the editor), we don't care about this
            _onlineInterface?.Dispose();
            _onlineInterface = null;
        }
    }

    void Update()
    {
        _networkInterface.Update();
        _onlineInterface?.Update();

        if (isChangingRole)
        {
            ProcessRoleChange();
        }
    }

    static void ProcessRoleChange()
    {
        switch (targetRole)
        {
            case OnlineRole.Client:
                {
                    if (networkInterface.state == NetworkState.Running && networkInterface.isServer)
                    {
                        networkInterface.Shutdown();
                    }

                    if (networkInterface.state == NetworkState.Stopped)
                    {
                        networkInterface.LaunchClient(OnLaunchClientResult);
                    }
                }
                break;
            case OnlineRole.Server:
                {
                    if (networkInterface.state == NetworkState.Running && networkInterface.isClient)
                    {
                        networkInterface.Shutdown();
                    }

                    if (networkInterface.state == NetworkState.Stopped)
                    {
                        networkInterface.LaunchServer(OnLaunchServerResult);
                    }
                }
                break;
            case OnlineRole.None:
                {
                    if (networkInterface.state == NetworkState.Running)
                    {
                        networkInterface.Shutdown();
                    }
                }
                break;
        }
    }

    static void OnLaunchClientResult(bool success, string message)
    {
        if (success)
        {
            if (clientInterface == null)
                Instance._onlineInterface = new OnlineClientInterface(networkInterface);
        }
    }

    static void OnLaunchServerResult(bool success, string message)
    {
        if (success)
        {
            if (serverInterface == null)
                Instance._onlineInterface = new OnlineServerInterface(networkInterface);
        }
    }

    NetworkInterface _networkInterface;
    OnlineInterface _onlineInterface;
}
