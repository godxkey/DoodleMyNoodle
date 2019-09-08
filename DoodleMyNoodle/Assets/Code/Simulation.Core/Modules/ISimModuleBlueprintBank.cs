using System;

public interface ISimModuleBlueprintBank : IDisposable
{
    SimBlueprint GetBlueprint(in SimBlueprintId blueprintId);
}
