using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

namespace MyNamespace
{
    public class FredTestScript : MonoBehaviour
    {
        class TestClass<T, U>
        {
            public class Potato
            {
                public class Legume<X>
                {
                    public struct Why<A, B, C, D, E, F, G, H, I, J, K, L, M>
                    {

                    }
                }
            }
        }

        [ContextMenu("do it")]
        void TEst()
        {
            Debug.Log(typeof(TestClass<int, float>.Potato.Legume<FredTestScript>.Why<bool, int, float, bool, bool2, bool2x2, float2x2, int, PointEffector2D, AudioReverbZone, half, WaitForSeconds, float>[]).GetPrettyName());
            Debug.Log(typeof(TestClass<int, float>.Potato.Legume<FredTestScript>.Why<bool, int, float, bool, bool2, bool2x2, float2x2, int, PointEffector2D, AudioReverbZone, half, WaitForSeconds[], float>[]).GetPrettyFullName());
        }
    }
}


//public class TestSystem : ComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            var entities = simEntityManager().CreateEntityQuery(typeof(FixTranslation)).ToEntityArray(Unity.Collections.Allocator.TempJob);
//            simEntityManager().Instantiate(entities[0]);

//            entities.Dispose();
//        }

//        if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            var entities = simEntityManager().CreateEntityQuery(typeof(FixTranslation)).ToEntityArray(Unity.Collections.Allocator.TempJob);
//            simEntityManager().DestroyEntity(entities[0]);
            
//            entities.Dispose();
//        }

//        if (Input.GetKeyDown(KeyCode.Alpha3))
//        {
//            simEntityManager().SetComponentData(GamePresentationCache.Instance.LocalPawn, new FixTranslation());
//        }

//        EntityManager simEntityManager()=> GameMonoBehaviourHelpers.SimulationWorld.EntityManager;
//    }
//}