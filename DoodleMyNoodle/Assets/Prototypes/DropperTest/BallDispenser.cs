using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDispenser : MonoBehaviour
{
    public struct ColorTime
    {
        public Color color;
        public float time;
    }
    [System.Serializable]
    public class BallDropper : SelfRegulatingDropper<ColorTime>
    {
        public BallDropper(int expectedQueueLength, float speedIncreasePerExtraItem = 1.25F)
            : base(expectedQueueLength, speedIncreasePerExtraItem)
        {
        }
    }


    public GameObject ballPrefab;
    public Transform spawnPosition;
    public Vector3 spawnForce;

    [ReadOnly]
    public int queueLength;
    [ReadOnly]
    public int heldQueueLength;

    public float orderDelay;
    public float orderCount;

    public BallDropper ballOrders;

    public Queue<ColorTime> heldBallOrders = new Queue<ColorTime>();

    IEnumerator Start()
    {
        float i = 0;
        while (true)
        {
            yield return new WaitForSeconds(orderDelay);

            i += orderCount;

            for (; i > 0; i--)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    heldBallOrders.Enqueue(PickColorTime());
                }
                else
                {
                    while (heldBallOrders.Count > 0)
                    {
                        ballOrders.Enqueue(heldBallOrders.Dequeue(), orderCount / orderDelay);
                    }

                    ballOrders.Enqueue(PickColorTime(), orderCount / orderDelay);
                }
            }
        }
    }

    private void Update()
    {
        queueLength = ballOrders.queueLength;
        heldQueueLength = heldBallOrders.Count;


        ballOrders.Update(Time.deltaTime);
        if (ballOrders.TryDrop(out ColorTime orderTime))
        {
            SpawnBall(orderTime.color);
            Debug.Log(Time.time - orderTime.time);
        }
    }

    void SpawnBall(Color color)
    {
        var ball = ballPrefab.Duplicate(spawnPosition.position, Quaternion.identity);
        ball.GetComponent<Rigidbody2D>().AddForce(spawnForce, ForceMode2D.Impulse);
        ball.GetComponent<SpriteRenderer>().color = color;
    }


    int colorIndex = -1;
    Color PickColor()
    {
        colorIndex++;
        colorIndex %= 3;
        switch (colorIndex)
        {
            default:
            case 0:
                return Color.red;
            case 1:
                return Color.white;
            case 2:
                return Color.white;
        }
    }

    ColorTime PickColorTime()
    {
        return new ColorTime() { color = PickColor(), time = Time.time };
    }
}
