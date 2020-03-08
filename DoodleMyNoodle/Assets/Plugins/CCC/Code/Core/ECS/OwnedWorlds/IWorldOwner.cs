using Unity.Entities;

public interface IWorldOwner
{
    World OwnedWorld { get; }

    void OnBeginEntitiesInjectionFromGameObjectConversion();
    void OnEndEntitiesInjectionFromGameObjectConversion();
}