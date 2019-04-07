using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RushToy : MonoBehaviour
{
    private RushManager gameManager;

    public int power = 1;

    public int team = -1;

    public TextMeshProUGUI powerText;

    public bool resolvedThisTurn = false;

    public SpriteRenderer teamIdentifier;

    private bool isBeingPlaced = false;

    public void Spawn(RushManager gameManager)
    {
        powerText.text = "" + power;
        this.gameManager = gameManager;
        isBeingPlaced = true;

        if(team == 0)
        {
            teamIdentifier.color = Color.blue;
        }
        else
        {
            teamIdentifier.color = Color.red;
        }
    }

    public void Place(Vector2 position)
    {
        transform.localPosition = position;
        isBeingPlaced = false;
    }

    public void SetPosition(Vector2 position)
    {
        transform.localPosition = position;
    }

    public void SetWorldPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void Move()
    {
        if(team == 0)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + 1);
        }
        else
        {
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - 1);
        }
    }

    public Vector2 GetNextPosition()
    {
        if(team == 0)
        {
            return new Vector2(transform.localPosition.x, transform.localPosition.y + 1);
        }
        else
        {
            return new Vector2(transform.localPosition.x, transform.localPosition.y - 1);
        }
    }
}
