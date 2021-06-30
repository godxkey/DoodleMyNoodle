namespace Sim.Operations
{
    [NetSerializable]
    public class SerializedWorld
    {
        [NetSerializable]
        public class BlobAsset
        {
            public uint Id;
            public byte[] Data;
        }

        public BlobAsset[] BlobAssets;
        public byte[] WorldData;
    }
}