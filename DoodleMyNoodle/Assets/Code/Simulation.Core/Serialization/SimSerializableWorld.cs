using System.Collections.Generic;

public class SimSerializableWorld
{
    public uint TickId = 0;

    public struct Entity
    {
        public string Name;
        public uint BlueprintIdIndex;
        public bool Active;
        public SimObjectId Id;

        // 1 for every component
        public List<Component> Components;

        public struct Component
        {
            public bool Enabled;
            public SimObjectId Id;
            public System.Type Type;
        }
    }

    public List<SimBlueprintId> ReferencedBlueprints;

    public List<Entity> Entities;

    // This should contain all the custom component data, serialized into a string (could also be a byte[])
    public string SerializedComponentDataStack;

    public SimObjectId NextObjectId;

    public int Seed = 0;

    public List<SimObjectId> ObjectsThatHaventStartedYet;

    public List<string> PresentationScenes;
}