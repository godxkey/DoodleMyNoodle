using System;
using UnityEngine;

public abstract class OnlineService : MonoCoreService<OnlineService>
{
    protected abstract NetworkInterface CreateNetworkInterface();
    protected abstract IDynamicNetSerializerImpl CreateNetMessageFactory();

    private static NetworkInterface NetworkInterface => Instance?._networkInterface;

    public static OnlineInterface OnlineInterface => Instance?._onlineInterface;
    public static OnlineClientInterface ClientInterface => OnlineInterface as OnlineClientInterface;
    public static OnlineServerInterface ServerInterface => OnlineInterface as OnlineServerInterface;

    public static OnlineRole TargetRole { get; private set; } = OnlineRole.None;
    public static OnlineRole CurrentRole
    {
        get
        {
            if (NetworkInterface.State != NetworkState.Running)
                return OnlineRole.None;

            if (NetworkInterface.IsClient)
                return OnlineRole.Client;
            else
                return OnlineRole.Server;
        }
    }

    public static bool IsChangingRole => TargetRole != CurrentRole;

    public static void SetTargetRole(OnlineRole role)
    {
        if (TargetRole != role)
        {
            TargetRole = role;

            ProcessRoleChange();
        }
    }

    public override void Initialize(Action<ICoreService> onComplete)
    {
        _networkInterface = CreateNetworkInterface();
        DynamicNetSerializer.impl = CreateNetMessageFactory();

        _networkInterface.OnShutdownBegin += OnNetworkInterfaceShutdownBegin;

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

        if (IsChangingRole)
        {
            ProcessRoleChange();
        }
    }

    static void ProcessRoleChange()
    {
        switch (TargetRole)
        {
            case OnlineRole.Client:
                {
                    if (NetworkInterface.State == NetworkState.Running && NetworkInterface.IsServer)
                    {
                        NetworkInterface.Shutdown();
                    }

                    if (NetworkInterface.State == NetworkState.Stopped)
                    {
                        NetworkInterface.LaunchClient(OnLaunchClientResult);
                    }
                }
                break;
            case OnlineRole.Server:
                {
                    if (NetworkInterface.State == NetworkState.Running && NetworkInterface.IsClient)
                    {
                        NetworkInterface.Shutdown();
                    }

                    if (NetworkInterface.State == NetworkState.Stopped)
                    {
                        NetworkInterface.LaunchServer(OnLaunchServerResult);
                    }
                }
                break;
            case OnlineRole.None:
                {
                    if (NetworkInterface.State == NetworkState.Running)
                    {
                        NetworkInterface.Shutdown();
                    }
                }
                break;
        }
    }

    static void OnLaunchClientResult(bool success, string message)
    {
        if (success)
        {
            if (ClientInterface == null)
                Instance._onlineInterface = new OnlineClientInterface(NetworkInterface);
        }
    }

    static void OnLaunchServerResult(bool success, string message)
    {
        if (success)
        {
            if (ServerInterface == null)
                Instance._onlineInterface = new OnlineServerInterface(NetworkInterface);
        }
    }

    NetworkInterface _networkInterface;
    OnlineInterface _onlineInterface;
}
