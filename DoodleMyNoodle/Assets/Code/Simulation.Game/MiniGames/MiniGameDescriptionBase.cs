using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/MiniGames/Description/Default")]
public class MiniGameDescriptionBase : ScriptableObject
{
    public enum SuccessRate
    {
        Failed = 1,
        Almost = 2,
        Good = 3,
        Success = 4,
        Critical = 5
    }
}