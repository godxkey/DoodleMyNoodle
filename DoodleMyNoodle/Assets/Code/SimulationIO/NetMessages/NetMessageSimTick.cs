
namespace SimulationControl
{
    /// <summary>
    /// Sent by the server to all clients for each processed simulation tick
    /// </summary>
    [NetSerializable]
    public struct NetMessageSimTick
    {
        /// <summary>
        /// All of the inputs to process in the simulation tick
        /// </summary>
        public SimTickData TickData;
    }
}