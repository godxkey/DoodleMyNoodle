using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct GameActionSettingUseInstigatorAsTarget : IComponentData
{
    public enum EType
    {
        /// <summary>
        /// E.g. The player that shot the fire ball
        /// </summary>
        FirstInstigatorActor,

        /// <summary>
        /// E.g. The fire ball item
        /// </summary>
        FirstInstigator,

        /// <summary>
        /// The frog on fire
        /// </summary>
        InstigatorActor,

        /// <summary>
        /// The fire effect on the frog triggering this action
        /// </summary>
        Instigator,
    }

    public EType Type;
}