using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngineX;

public class PlayerAssetManagerServer : PlayerAssetManager
{
    private SessionServerInterface _session;

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        _session = OnlineService.ServerInterface?.SessionServerInterface;
        _session.RegisterNetMessageReceiver<NetMessagePlayerAssets>(OnAssetUpdatedByClient);
        _session.OnConnectionAdded += OnClientConnectionAdded;

        foreach (var connection in _session.Connections)
        {
            OnClientConnectionAdded(connection);
        }
    }

    private void OnClientConnectionAdded(INetworkInterfaceConnection connection)
    {
        // send all doodles to client
        SendPlayerAssetsToClients(_assetMap.Values.ToArray(), new INetworkInterfaceConnection[] { connection });
    }

    protected override void OnDestroy()
    {
        if (_session != null)
        {
            _session.OnConnectionAdded -= OnClientConnectionAdded;
            _session.UnregisterNetMessageReceiver<NetMessagePlayerAssets>(OnAssetUpdatedByClient);
        }

        base.OnDestroy();
    }

    protected override void PublishAssetChangesInternal(PlayerAsset playerAsset)
    {
        if (_session == null)
            return;

        SendPlayerAssetsToClients(new PlayerAsset[] { playerAsset }, _session.Connections.ToArray());
    }

    private void OnAssetUpdatedByClient(NetMessagePlayerAssets netMessage, INetworkInterfaceConnection source)
    {
        if (netMessage.Assets == null || netMessage.Assets.Length == 0)
            return;

        UpdateOrCreateLocalAssets(netMessage);

        List<PlayerAsset> assetsToSend = new List<PlayerAsset>(netMessage.Assets.Length);

        for (int i = 0; i < netMessage.Assets.Length; i++)
        {
            PlayerAsset playerAsset = GetAsset(netMessage.Assets[i].Guid);
            if (playerAsset != null)
            {
                assetsToSend.Add(playerAsset);
            }
        }

        List<INetworkInterfaceConnection> clientsToSendTo = _session.Connections.ToList();
        clientsToSendTo.Remove(source);

        SendPlayerAssetsToClients(assetsToSend.ToArray(), clientsToSendTo.ToArray());
    }

    private void SendPlayerAssetsToClients(PlayerAsset[] playerAssets, INetworkInterfaceConnection[] clients)
    {
        NetMessagePlayerAssets.Data[] datas = new NetMessagePlayerAssets.Data[playerAssets.Length];
        for (int i = 0; i < playerAssets.Length; i++)
        {
            datas[i] = new NetMessagePlayerAssets.Data(playerAssets[i]);
        }

        NetMessagePlayerAssets netMessage = new NetMessagePlayerAssets() { Assets = datas };

        // send message
        foreach (var client in clients)
        {
            _session.BeginLargeDataTransfer(netMessage, client, "Doodles");
        }
    }
}
