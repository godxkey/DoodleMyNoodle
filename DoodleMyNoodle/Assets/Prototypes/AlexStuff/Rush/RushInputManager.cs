using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushInputManager : MonoBehaviour
{
    public RushManager gameManager;

    public Transform gameWorld;

    public bool inputEnable = true;

    public Camera gameCamera;

    public Transform borderBottomLeft;
    public Transform borderTopRight;

    public List<Transform> placingZone = new List<Transform>();

    public bool currentlyPlacingAToy = false;

    private Vector2 lastTileClicked = new Vector2(-1,-1);

    void Update()
    {
        if (inputEnable)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0;
            Vector3 worldMousePosition = gameCamera.ScreenToWorldPoint(mousePosition);

            gameManager.UpdateCurrentlySpawnedToy(worldMousePosition);

            gameManager.feedbacks.ClearAll();

            if (currentlyPlacingAToy)
            {
                ColorizePlacingZone();
            }

            // Is inside the game world space ?
            if (worldMousePosition.x <= borderTopRight.position.x
            && worldMousePosition.x >= borderBottomLeft.position.x
            && worldMousePosition.y <= borderTopRight.position.y
            && worldMousePosition.y >= borderBottomLeft.position.y)
            {
                Vector2 gameWorldMousePosition = worldMousePosition;
                gameWorldMousePosition.x = gameWorldMousePosition.x + Mathf.Abs(gameWorld.position.x);
                gameWorldMousePosition = new Vector2(Mathf.RoundToInt(gameWorldMousePosition.x), Mathf.RoundToInt(gameWorldMousePosition.y));

                if (Input.GetMouseButtonDown(0))
                {
                    if (ClickInputInPlacingZone(gameWorldMousePosition))
                    {
                        lastTileClicked = gameWorldMousePosition;
                        gameManager.PlaceNewToy(gameWorldMousePosition);
                    }
                }
                else
                {
                    if (gameWorldMousePosition != lastTileClicked)
                    {
                        lastTileClicked = new Vector2(-1, -1);
                        gameManager.feedbacks.ChangeTileDisplay(RushFeedbacks.TileType.Hover, gameWorldMousePosition);
                    }
                }
            }
        }
    }

    private void ColorizePlacingZone()
    {
        for (int i = 0; i < placingZone.Count; i++)
        {
            gameManager.feedbacks.ChangeTileDisplay(RushFeedbacks.TileType.Available, placingZone[i].localPosition);
        }
    }

    private bool ClickInputInPlacingZone(Vector2 clickInputLocation)
    {
        bool result = false;
        for (int i = 0; i < placingZone.Count; i++)
        {
            Vector2 currentPlacingTileLocation = placingZone[i].localPosition;
            if (currentPlacingTileLocation == clickInputLocation)
            {
                result = true;
            }
        }
        return result;
    }
}
