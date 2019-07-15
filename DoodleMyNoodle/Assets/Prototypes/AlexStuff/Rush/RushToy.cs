using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class RushToy : MonoBehaviour
{
    private RushManager gameManager;

    public int power = 1;

    public int team = -1;

    public TextMeshProUGUI powerText;

    public bool resolvedThisTurn = false;

    public SpriteRenderer teamIdentifier;

    public float movementSpeed = 0.1f;

    //private bool isBeingPlaced = false; // fbessette: j'lai commenté pour enlevé le warning de l'editeur comme quoi elle était 'unused'

    public void Spawn(RushManager gameManager)
    {
        powerText.text = "" + power;
        this.gameManager = gameManager;
        //isBeingPlaced = true;

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
        //isBeingPlaced = false;
    }

    public void SetPosition(Vector2 position)
    {
        transform.localPosition = position;
    }

    public void SetWorldPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void Move(System.Action onComplete)
    {
        Tween currentTween;
        if(team == 0)
        {
            currentTween = transform.DOLocalMove(new Vector2(transform.localPosition.x, transform.localPosition.y + 1), movementSpeed).OnComplete(()=> 
            {
                if (team == 0)
                {
                    if (transform.localPosition.y > gameManager.inputManager.borderTopRight.localPosition.y)
                    {
                        gameManager.ToyReachedOutside(this);
                    }
                }
                else
                {
                    if (transform.localPosition.y < gameManager.inputManager.borderBottomLeft.localPosition.y)
                    {
                        gameManager.ToyReachedOutside(this);
                    }
                }
                onComplete?.Invoke();
            });
        }
        else
        {
            currentTween = transform.DOLocalMove(new Vector2(transform.localPosition.x, transform.localPosition.y - 1), movementSpeed).OnComplete(() => 
            {
                if (team == 0)
                {
                    if (transform.localPosition.y > gameManager.inputManager.borderTopRight.localPosition.y)
                    {
                        gameManager.ToyReachedOutside(this);
                    }
                }
                else
                {
                    if (transform.localPosition.y < gameManager.inputManager.borderBottomLeft.localPosition.y)
                    {
                        gameManager.ToyReachedOutside(this);
                    }
                }
                onComplete?.Invoke();
            });
        }

        currentTween.Play();
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
