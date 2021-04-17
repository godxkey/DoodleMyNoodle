using CCC.Fix2D;
using Unity.Entities;

[UpdateBefore(typeof(AISystemGroup))]
public class PreAISystemGroup : SimComponentSystemGroup { }

[UpdateBefore(typeof(InputSystemGroup))]
public class AISystemGroup : SimComponentSystemGroup { }

[UpdateBefore(typeof(MovementSystemGroup))]
[UpdateBefore(typeof(SignalSystemGroup))]
public class InputSystemGroup : SimComponentSystemGroup { }

[UpdateAfter(typeof(ReactOnCollisionSystem))]
public class SignalSystemGroup : SimComponentSystemGroup { }

public class MovementSystemGroup : SimComponentSystemGroup { }