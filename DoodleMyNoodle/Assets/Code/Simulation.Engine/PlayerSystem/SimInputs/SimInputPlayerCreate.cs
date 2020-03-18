
[NetSerializable]
public class SimInputPlayerCreateOld : SimInput
{
    public SimPlayerInfo SimPlayerInfo;
}

[NetSerializable]
public class SimInputPlayerCreate : SimMasterInput
{
    public string PlayerName;
}