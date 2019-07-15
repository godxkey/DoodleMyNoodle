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

    public SimBlueprintId ballBlueprint;

    private void Start()
    {
    }

    public void Update()
    {
        if (Game.started == false)
            return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            SimulationController.instance.SubmitInput(new SimCommandLog() { message = "hello!" });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SimulationController.instance.SubmitInput(new SimCommandInjectBlueprint() { blueprintId = ballBlueprint });
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            SimulationController.instance.SubmitInput(new SimCommandMoveBall() { moveDirection = FixVector3.right });
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            SimulationController.instance.SubmitInput(new SimCommandMoveBall() { moveDirection = FixVector3.left });
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            SimulationController.instance.SubmitInput(new SimCommandMoveBall() { moveDirection = FixVector3.forward });
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            SimulationController.instance.SubmitInput(new SimCommandMoveBall() { moveDirection = FixVector3.backward });
        }
    }
}