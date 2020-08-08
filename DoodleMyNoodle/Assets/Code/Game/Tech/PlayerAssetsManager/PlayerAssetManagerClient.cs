public class PlayerAssetManagerClient : PlayerAssetManager
{
    private SessionClientInterface _session;

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        _session = OnlineService.ClientInterface?.SessionClientInterface;
        _session.RegisterNetMessageReceiver<NetMessagePlayerAssets>(OnAssetUpdatedByServer);
    }

    protected override void OnDestroy()
    {
        if (_session != null)
        {
            _session.UnregisterNetMessageReceiver<NetMessagePlayerAssets>(OnAssetUpdatedByServer);
        }

        base.OnDestroy();
    }

    protected override void PublishAssetChangesInternal(PlayerAsset playerAsset)
    {
        if (_session == null)
            return;

        NetMessagePlayerAssets.Data[] asset = new NetMessagePlayerAssets.Data[]
        {
            new NetMessagePlayerAssets.Data(playerAsset)
        };
        
        // send message
        _session.BeginLargeDataTransfer(new NetMessagePlayerAssets() { Assets = asset }, _session.ServerConnection, "Doodle");
    }

    private void OnAssetUpdatedByServer(NetMessagePlayerAssets netMessage, INetworkInterfaceConnection source)
    {
        UpdateOrCreateLocalAssets(netMessage);
    }
}
