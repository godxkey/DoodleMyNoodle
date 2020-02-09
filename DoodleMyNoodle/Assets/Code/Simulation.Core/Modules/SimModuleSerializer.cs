using Newtonsoft.Json;
using Sim.Operations;
using System;
using System.Collections.Generic;

internal class SimModuleSerializer : SimModuleBase
{
    internal bool CanSimWorldBeSaved =>
        SimModules._SceneLoader.PendingSceneLoads == 0
        && SimModules._Ticker.IsTicking == false
        && IsInDeserializationProcess == false;

    internal bool CanSimWorldBeLoaded =>
        SimModules._SceneLoader.PendingSceneLoads == 0
        && SimModules._Ticker.IsTicking == false
        && IsInDeserializationProcess == false;

    internal bool IsInDeserializationProcess =>
        _deserializationOperation != null
        && _deserializationOperation.IsRunning;

    internal bool IsInSerializationProcess =>
        _serializationOperation != null
        && _serializationOperation.IsRunning;

    internal Action<SimSerializationResult> OnSerializationResult;
    internal Action<SimDeserializationResult> OnDeserializationResult;

    SimDeserializationOperation _deserializationOperation;
    SimSerializationOperationWithCache _serializationOperation;

    JsonSerializerSettings _cachedJsonSettings;
    SimObjectJsonConverter _cachedSimObjectJsonConverter;

    JsonSerializerSettings GetJsonSettings()
    {
        if (_cachedJsonSettings == null)
        {
            _cachedSimObjectJsonConverter = new SimObjectJsonConverter();

            _cachedJsonSettings = new JsonSerializerSettings();

            _cachedJsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            _cachedJsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            _cachedJsonSettings.TypeNameHandling = TypeNameHandling.Auto;
            _cachedJsonSettings.Converters = new List<JsonConverter>()
            {
                _cachedSimObjectJsonConverter,
                new IDTypeJsonConverter(),
                new Fix64JsonConverter(),
            };
            _cachedJsonSettings.ContractResolver = new CustomJsonContractResolver();
            _cachedJsonSettings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
        }
        return _cachedJsonSettings;
    }

    SimObjectJsonConverter GetSimObjectJsonConverter()
    {
        GetJsonSettings();
        return _cachedSimObjectJsonConverter;
    }

    public SimSerializationOperationWithCache SerializeSimulation()
    {
        if (!SimModules._Serializer.CanSimWorldBeSaved)
        {
            DebugService.LogError("Cannot serialize SimWorld right now. We must not be ticking nor loading a scene");
            return null;
        }

        _serializationOperation = new SimSerializationOperationWithCache(GetSimObjectJsonConverter(), GetJsonSettings());

        _serializationOperation.OnFailCallback = (op) =>
        {
            OnSerializationResult?.Invoke(new SimSerializationResult()
            {
                SuccessLevel = SimSerializationResult.SuccessType.Failed,
            });
        };

        _serializationOperation.OnSucceedCallback = (op) =>
        {
            SimSerializationOperationWithCache serializationOp = (SimSerializationOperationWithCache)op;
            OnSerializationResult?.Invoke(new SimSerializationResult()
            {
                SuccessLevel = serializationOp.PartialSuccess ?
                    SimSerializationResult.SuccessType.PartialSuccess :
                    SimSerializationResult.SuccessType.Succeeded,
                Data = serializationOp.SerializationData
            });
        };

        _serializationOperation.Execute();

        return _serializationOperation;
    }

    public SimDeserializationOperation DeserializeSimulation(string data)
    {
        if (!CanSimWorldBeLoaded)
        {
            DebugService.LogError("Cannot deserialize SimWorld right now. We must not be ticking nor loading a scene");
            return null;
        }

        _deserializationOperation = new SimDeserializationOperation(data, GetSimObjectJsonConverter(), GetJsonSettings());

        _deserializationOperation.OnFailCallback = (op) =>
        {
            OnDeserializationResult?.Invoke(new SimDeserializationResult()
            {
                SuccessLevel = SimDeserializationResult.SuccessType.Failed,
            });
        };

        _deserializationOperation.OnSucceedCallback = (op) =>
        {
            OnDeserializationResult?.Invoke(new SimDeserializationResult()
            {
                SuccessLevel = ((SimDeserializationOperation)op).PartialSuccess ?
                    SimDeserializationResult.SuccessType.PartialSuccess :
                    SimDeserializationResult.SuccessType.Succeeded
            });
        };

        _deserializationOperation.Execute();

        return _deserializationOperation;
    }

    public override void Dispose()
    {
        base.Dispose();

        if (_deserializationOperation != null && _deserializationOperation.IsRunning)
            _deserializationOperation.TerminateWithFailure();

        if (_serializationOperation != null && _serializationOperation.IsRunning)
            _serializationOperation.TerminateWithFailure();
    }
}