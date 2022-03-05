using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public abstract class AnimationDefinition : ScriptableObject
{
    public struct PresentationTarget
    {
        public Transform Bone;
        public SpriteRenderer SpriteRenderer;
        public GameObject Root;

        public PresentationTarget(GameObject root, Transform bone, SpriteRenderer spriteRenderer)
        {
            Root = root;
            Bone = bone;
            SpriteRenderer = spriteRenderer;
        }
    }

    public struct TriggerInput
    {
        public Entity SimulationTarget;
        public PresentationTarget PresentationTarget;
        public Dictionary<string, object> Parameters;

        /// <summary>
        /// The unique ID for the trigger of animation
        /// </summary>
        public int TriggerId;

        public TriggerInput(Entity simulationTarget, PresentationTarget presentationTarget, Dictionary<string, object> parameters, int triggerId)
        {
            SimulationTarget = simulationTarget;
            PresentationTarget = presentationTarget;
            Parameters = parameters;
            TriggerId = triggerId;
        }

        public T GetAnimationData<T>(string dataTypeID)
        {
            T result = default;
            bool success = false;
            if (Parameters != null && Parameters.TryGetValue(dataTypeID, out object obj))
            {
                if (obj is T castedObj)
                {
                    result = castedObj;
                    success = true;
                }

            }

            if (!success)
                Log.Warning($"Animation Data {dataTypeID} of type {typeof(T).Name} couldn't be found.");

            return result;
        }

        public GameAction.ResultDataElement GetGameActionResultData()
        {
            return GetAnimationData<GameAction.ResultDataElement>("GameActionContextResult");
        }
    }

    public struct TriggerOuput
    {
        public float Duration;
    }

    public struct FinishInput
    {
        /// <summary>
        /// The unique ID for the trigger of animation
        /// </summary>
        public int TriggerId;
        public PresentationTarget PresentationTarget;

        public FinishInput(int triggerId, PresentationTarget presentationTarget)
        {
            TriggerId = triggerId;
            PresentationTarget = presentationTarget;
        }
    }

    public struct StopInput
    {
        /// <summary>
        /// The unique ID for the trigger of animation
        /// </summary>
        public int TriggerId;
        public PresentationTarget PresentationTarget;

        public StopInput(int triggerId, PresentationTarget presentationTarget)
        {
            TriggerId = triggerId;
            PresentationTarget = presentationTarget;
        }
    }

    public abstract void TriggerAnimation(TriggerInput input, ref TriggerOuput output);
    public abstract void StopAnimation(StopInput input);
}
