
[NetSerializable]
public class SimInputPlayerCreateOld : SimInput
{
    public SimPlayerInfo SimPlayerInfo;
}

[NetSerializable]
public class SimInputPlayerCreate : SimInput
{
    public string PlayerName;
}