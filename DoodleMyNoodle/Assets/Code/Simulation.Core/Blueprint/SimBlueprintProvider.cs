using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SimBlueprintRequest
{
    public SimBlueprintId BlueprintId;
    public SimBlueprint Result;

    public bool IsFulfilled => Result.IsValid;
}

public interface ISimBlueprintProvider
{
    bool CanProvideBlueprintFor(in SimBlueprintId blueprintId);
    SimBlueprint ProvideBlueprint(in SimBlueprintId blueprintId);


    // needs to be revisited a bit
    void ProvideBlueprintsAsync(in List<SimBlueprintId> blueprintIds, Action<List<SimBlueprint>> onComplete);
    void ProvideBlueprintAsync(in SimBlueprintId blueprintId, Action<SimBlueprint> onComplete);
    void EndProvideBlueprint();


    bool CanProvideBlueprintSynchronously();
}
