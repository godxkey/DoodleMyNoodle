public interface ISimEntityListChangeObserver
{
    void OnAddSimObjectToRuntime(SimObject obj);
    void OnRemoveSimObjectFromRuntime(SimObject obj);
}