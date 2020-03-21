public struct SimDeserializationResult
{
    // Might seem like a copy/paste of SimSerializationResult but its ok because they are not logically
    // identical. One struct might evolve one way while the other doesn't
    public enum SuccessType
    {
        /// <summary>
        /// Everything went correctly
        /// </summary>
        Succeeded,

        /// <summary>
        /// Some data was not properly deserialized
        /// </summary>
        PartialSuccess,

        /// <summary>
        /// The deserialization failed
        /// </summary>
        Failed
    }

    public SuccessType SuccessLevel;
}

