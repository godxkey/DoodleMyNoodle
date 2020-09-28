using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.SceneManagement;

public interface IWorldOwner
{
    World OwnedWorld { get; }

    void OnBeginEntitiesInjectionFromGameObjectConversion(List<Scene> comingFromScenes);
    void OnEndEntitiesInjectionFromGameObjectConversion();

    uint ReplaceVersion { get; }
    event Action WorldReplaced;
}