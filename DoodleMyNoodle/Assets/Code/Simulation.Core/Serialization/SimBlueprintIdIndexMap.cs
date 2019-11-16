using System.Collections.Generic;

public class SimBlueprintIdIndexMap
{
    Dictionary<SimBlueprintId, uint> _blueprintIdMap = new Dictionary<SimBlueprintId, uint>(); // used for quick search
    List<SimBlueprintId> _blueprintIdList = new List<SimBlueprintId>(256);

    public uint GetIndexFromBlueprintId(in SimBlueprintId blueprintId)
    {
        uint index;

        if(_blueprintIdMap.TryGetValue(blueprintId, out uint foundIndex))
        {
            index = foundIndex;
        }
        else
        {
            index = (uint)_blueprintIdList.Count;

            // add to list
            _blueprintIdMap.Add(blueprintId, index);
            _blueprintIdList.Add(blueprintId);
        }

        return index;
    }

    public List<SimBlueprintId> GetList()
    {
        return _blueprintIdList;
    }
}
