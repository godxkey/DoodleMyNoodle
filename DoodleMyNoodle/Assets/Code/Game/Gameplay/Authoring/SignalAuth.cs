using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class SignalAuth : MonoBehaviour, IConvertGameObjectToEntity, SignalAuth.Gizmos.ISignalListener
{
    [FormerlySerializedAs("StayOnForever")]
    public bool NeverOffAfterOn = false;
    public ESignalEmissionType Emission = ESignalEmissionType.None;
    public List<SignalAuth> LogicTargets = new List<SignalAuth>();
    //public List<SignalAuth> PropagationTargets = new List<SignalAuth>();

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Signal>(entity);
        dstManager.AddComponent<PreviousSignal>(entity);

        if (Emission != ESignalEmissionType.None)
        {
            dstManager.AddComponent<SignalEmissionFlags>(entity);
            dstManager.AddComponentData<InteractableFlag>(entity, Emission == ESignalEmissionType.OnClick || Emission == ESignalEmissionType.ToggleOnClick);
            dstManager.AddComponentData<SignalEmissionType>(entity, Emission);

            var logicTargetsECS = dstManager.AddBuffer<SignalLogicTarget>(entity);
            foreach (var target in LogicTargets)
            {
                if (target != null)
                    logicTargetsECS.Add(conversionSystem.GetPrimaryEntity(target.gameObject));
            }
        }

        dstManager.AddComponentData<SignalStayOnForever>(entity, NeverOffAfterOn);
        //var targetsECS = dstManager.AddBuffer<SignalPropagationTarget>(entity);

        //foreach (var target in PropagationTargets)
        //{
        //    if (target != null)
        //        targetsECS.Add(conversionSystem.GetPrimaryEntity(target.gameObject));
        //}

    }

    void OnValidate()
    {
        for (int i = LogicTargets.Count - 1; i >= 0; i--)
        {
            if (LogicTargets[i] != null && !LogicTargets[i].gameObject.scene.IsValid())
            {
                LogicTargets.RemoveAt(i);
                Log.Error($"Cannot reference an asset. {nameof(LogicTargets)} needs to be a scene reference");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSignalGizmos(this);
    }

    private void OnEnable()
    {
        Gizmos.Register(this);
    }

    private void OnDisable()
    {
        Gizmos.Unregister(this);
    }

    void Gizmos.ISignalListener.GetSignalReferences(List<SignalAuth> result)
    {
        if (Emission == ESignalEmissionType.AND || Emission == ESignalEmissionType.OR)
            result.AddRange(LogicTargets);
    }


    public static class Gizmos
    {
        public interface ISignalListener
        {
            void GetSignalReferences(List<SignalAuth> result);
        }

        private static List<ISignalListener> s_listeners = new List<ISignalListener>();

        public static void Register(ISignalListener listener)
        {
            s_listeners.AddUnique(listener);
        }

        public static void Unregister(ISignalListener listener)
        {
            s_listeners.Remove(listener);
        }


        private static Dictionary<Component, List<SignalAuth>> s_listener2Emitter = new Dictionary<Component, List<SignalAuth>>();
        private static Dictionary<SignalAuth, List<Component>> s_emitter2Listener = new Dictionary<SignalAuth, List<Component>>();

        public static void RegisterOrUpdate(Component listener, List<SignalAuth> emitters)
        {
            if (!s_listener2Emitter.TryGetValue(listener, out List<SignalAuth> emittersList))
            {
                emittersList = new List<SignalAuth>();
                s_listener2Emitter[listener] = emittersList;
            }

            emittersList.Clear();
            emittersList.AddRange(emitters);

            foreach (var emitter in emitters)
            {
                if (!s_emitter2Listener.TryGetValue(emitter, out List<Component> listenersList))
                {
                    listenersList = new List<Component>();
                    s_emitter2Listener[emitter] = listenersList;
                }

                listenersList.AddUnique(listener);
            }
        }

        public static void DrawSignalGizmos(ISignalListener obj)
        {
            List<SignalAuth> references = ListPool<SignalAuth>.Take();
            if (obj is SignalAuth signal)
            {
                foreach (var listener in s_listeners)
                {
                    if (listener is Component listenerComponent && listenerComponent != null)
                    {
                        references.Clear();
                        listener.GetSignalReferences(references);
                        if (references.Contains(signal))
                        {
                            DrawSignalGizmo(listenerComponent, signal);
                        }
                    }
                }
            }

            if (obj is Component objComponent && objComponent != null)
            {
                references.Clear();
                obj.GetSignalReferences(references);
                foreach (var item in references)
                {
                    DrawSignalGizmo(objComponent, item);
                }
            }

            ListPool<SignalAuth>.Release(references);
        }

        private static void DrawSignalGizmo(Component listener, SignalAuth emitter)
        {
#if UNITY_EDITOR
            const float SEGMENT_LENGTH = 0.35f;
            const float LINE_THICKNESS = 6f;

            Handles.color = new Color(1f, 0.35f, 0);

            Vector2 p1 = listener.transform.position;
            Vector2 p2 = emitter.transform.position;
            Vector2 dir = (p2 - p1).normalized;

            // Draw arrow cap
            DrawLine(p1, p1 + dir.Rotate(30));
            DrawLine(p1, p1 + dir.Rotate(-30));

            // Draw lines
            bool draw = true;
            while (p1 != p2)
            {
                Vector2 p1Next = Vector2.MoveTowards(p1, p2, SEGMENT_LENGTH);

                if (draw)
                {
                    DrawLine(p1, p1Next);
                }

                p1 = p1Next;
                draw = !draw;
            }


            void DrawLine(Vector2 a, Vector2 b)
            {
                Handles.DrawLine(a, b, LINE_THICKNESS);
            }
#endif
        }
    }
}
