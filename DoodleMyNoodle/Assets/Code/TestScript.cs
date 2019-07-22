using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public SceneInfo ballpitScene;

    public SimBlueprintId ballBlueprint;

    //FixVector3[] dirs = new FixVector3[500];
    //Color[] colrs = new Color[500];

    //private void Start()
    //{
    //    FixRandom random = new FixRandom();
    //    for (int i = 0; i < dirs.Length; i++)
    //    {
    //        dirs[i] = new FixVector3(random.RandomDirection2D(), 0);
    //        colrs[i] = ColorHSV.ToColor(new ColorHSV(UnityEngine.Random.value, 1, 1));
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < dirs.Length; i++)
    //    {
    //        Gizmos.color = colrs[i];
    //        Gizmos.DrawLine(Vector3.zero, dirs[i].ToUnityVec());
    //    }
    //}

    public void Update()
    {
        if (Game.started == false)
            return;


        if (Input.GetKeyDown(KeyCode.L))
        {
            SimulationController.instance.SubmitInput(new SimCommandLog() { message = "hello!" });
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SimulationController.instance.SubmitInput(new SimCommandLoadScene() { sceneName = ballpitScene.SceneName });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SimulationController.instance.SubmitInput(new SimCommandInjectBlueprint() { blueprintId = ballBlueprint });
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.Space });
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