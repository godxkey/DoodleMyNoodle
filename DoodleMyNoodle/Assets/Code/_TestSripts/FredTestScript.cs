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

public class FredTestScript : MonoBehaviour
{
    [Serializable]
    public struct Value : IElementIndexHint
    {
        public string Val;
        public int IndexHint { get; set; }

        public override string ToString()
        {
            return Val.ToString();
        }
    }

    public List<Value> Values;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NoInterruptList<Value> list = new NoInterruptList<Value>(Values);

            foreach (var item in list)
            {
                Log.Info(item);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NoInterruptList<Value> list = new NoInterruptList<Value>(Values);

            int i = 0;
            foreach (var item in list)
            {
                if (i % 4 == 0)
                {
                    Log.Info("remove " + item + " " + list.Remove(item));
                }
                else
                {
                    Log.Info(item);
                }

                i++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NoInterruptList<Value> list = new NoInterruptList<Value>(Values);

            int i = 0;
            foreach (var x in list)
            {
                Log.Info("parent " + x);

                foreach (var item in list)
                {
                    if (i % 4 == 0)
                    {
                        Log.Info("remove " + item + " " + list.Remove(item));
                    }
                    else
                    {
                        Log.Info(item);
                    }

                    i++;
                }
            }
        }
    }
}
