using System;

public interface ISimModuleBlueprintBank
{
    SimBlueprint GetBlueprint(in SimBlueprintId blueprintId);
}
