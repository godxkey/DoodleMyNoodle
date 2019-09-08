
public interface ISimInputProcessor
{
    void ProcessInput(SimInput input);
    bool isActiveAndEnabled { get; }
}