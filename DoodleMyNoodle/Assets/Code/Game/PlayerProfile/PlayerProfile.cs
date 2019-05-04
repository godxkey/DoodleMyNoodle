
[System.Serializable]
public class PlayerProfile : PlayerProfileReadOnly
{
    public string playerName { get; set; }
    public int localId { get; set; }


    public const string fileNameExtension = ".prof";
    public string GenerateFileName() => "profile-" + playerName + fileNameExtension;
}



public interface PlayerProfileReadOnly
{
    string playerName { get; }
    int localId { get; }
}