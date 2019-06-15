using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    internal bool test;
    [NonSerialized]
    private Location locationComponent;

    private void Start()
    {
    }

    public void Update()
    {
        if (Game.started == false)
            return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            SimulationController.instance.SubmitInput(new SimInputLog() { message = "hello!" });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SimulationController.instance.SubmitInput(new SimInputInstantiate() { blueprintId = new SimBlueprintId(1) });
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputMoveBall() { moveDirection = FixVector2.right });
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputMoveBall() { moveDirection = FixVector2.left });
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputMoveBall() { moveDirection = FixVector2.up });
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputMoveBall() { moveDirection = FixVector2.down });
        }
    }
}