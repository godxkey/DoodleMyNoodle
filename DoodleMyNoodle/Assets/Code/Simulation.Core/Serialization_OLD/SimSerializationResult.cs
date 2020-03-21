public struct SimSerializationResult
{
    public enum SuccessType
    {
        /// <summary>
        /// Everything went correctly
        /// </summary>
        Succeeded,

        /// <summary>
        /// Some data was not properly serialized
        /// </summary>
        PartialSuccess,

        /// <summary>
        /// The serialization failed
        /// </summary>
        Failed
    }
    
    public SuccessType SuccessLevel;
    public string Data;
}

