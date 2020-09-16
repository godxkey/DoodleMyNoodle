using Unity.Entities;

[UpdateBefore(typeof(AISystemGroup))]
public class PreAISystemGroup : SimComponentSystemGroup { }

[UpdateBefore(typeof(InputSystemGroup))]
public class AISystemGroup : SimComponentSystemGroup { }

[UpdateBefore(typeof(MovementSystemGroup))]
public class InputSystemGroup : SimComponentSystemGroup { }

public class MovementSystemGroup : SimComponentSystemGroup { }