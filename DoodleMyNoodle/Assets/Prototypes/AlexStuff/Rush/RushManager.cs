using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushManager : MonoBehaviour
{
    public RushGrid grid;
    public RushFeedbacks feedbacks;
    public RushUI ui;
    public RushCombatResolver combatResolver;
    public RushInputManager inputManager;

    public Transform entityContainer;
    public GameObject toyPrefab;

    public Vector2 firstTileToTrigger;

    public List<Transform> aiPlacingZone = new List<Transform>();

    private RushToy currentlySpawnedToy;

    public void SpawnNewToy()
    {
        RushToy newToy = Instantiate(toyPrefab, entityContainer).GetComponent<RushToy>();
        currentlySpawnedToy = newToy;
        currentlySpawnedToy.team = 0;
        newToy.Spawn(this);
        inputManager.currentlyPlacingAToy = true;
    }

    public void SpawnAINewToy()
    {
        RushToy newToy = Instantiate(toyPrefab, entityContainer).GetComponent<RushToy>();
        currentlySpawnedToy = newToy;
        currentlySpawnedToy.team = 1;
        newToy.Spawn(this);
        inputManager.currentlyPlacingAToy = false;
    }

    public void UpdateCurrentlySpawnedToy(Vector3 position)
    {
        if (currentlySpawnedToy != null)
        {
            currentlySpawnedToy.SetWorldPosition(position);
        }
    }

    public void PlaceNewToy(Vector2 position)
    {
        if (currentlySpawnedToy != null)
        {
            grid.ToyHasBeenSpawned(currentlySpawnedToy);
            currentlySpawnedToy.Place(position);
            currentlySpawnedToy = null;
            inputManager.currentlyPlacingAToy = false;
        }
    }

    public void KillToy(RushToy toy)
    {
        grid.ToyHasBeenDestroyed(toy);
        Destroy(toy.gameObject);
    }

    public void StartNewTurn()
    {
        Debug.Log("StartNewTurn");

        inputManager.inputEnable = false;

        ResolveAITurn();

        ResolveTurn();
        //StartCoroutine(ResolveTurn());
    }

    private void ResolveTurn()
    {
        Debug.Log("ResolvingTurn");

        // Resolve Middle Row
        for (int x = 0; x < 7; x++)
        {
            RushToy currentTileToy = grid.GetToyAt(new Vector2(x, firstTileToTrigger.y));

            Debug.Log("Resolving - " + x + "," + firstTileToTrigger.y);

            ResolveToyTurn(currentTileToy);
        }

        // Resolve Other Rows
        for (int y = 1; y <= 4; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                Debug.Log("Resolving - " + x + "," + (firstTileToTrigger.y - y));
                Debug.Log("Resolving - " + x + "," + (firstTileToTrigger.y + y));

                RushToy playerOneToy = grid.GetToyAt(new Vector2(x, firstTileToTrigger.y - y));
                RushToy playerTwoToy = grid.GetToyAt(new Vector2(x, firstTileToTrigger.y + y));

                if (playerOneToy == null)
                {
                    ResolveToyTurn(playerTwoToy);
                    continue;
                }
                else if (playerTwoToy == null)
                {
                    ResolveToyTurn(playerOneToy);
                    continue;
                }
                else
                {
                    Vector2 calculatedNewPlayerOnePosition = playerOneToy.GetNextPosition();
                    Vector2 calculatedNewPlayerTwoPosition = playerOneToy.GetNextPosition();

                    if ((playerOneToy.team != playerTwoToy.team) && ((grid.GetToyAt(calculatedNewPlayerOnePosition) == playerTwoToy) && (grid.GetToyAt(calculatedNewPlayerTwoPosition) == playerOneToy)))
                    {
                        RushCombatResolver.CombatResult result = combatResolver.ResolveConflict(playerOneToy, playerTwoToy);
                        feedbacks.SpawnBattleFeedbackOnTile(calculatedNewPlayerOnePosition);
                        if (result.winner != null)
                        {
                            if (result.winner == playerOneToy)
                            {
                                KillToy(playerTwoToy);
                                playerOneToy.Move();
                                playerOneToy.resolvedThisTurn = true;
                                continue;
                            }
                            else
                            {
                                KillToy(playerOneToy);
                                playerTwoToy.Move();
                                playerTwoToy.resolvedThisTurn = true;
                                continue;
                            }
                        }
                        else
                        {
                            KillToy(playerOneToy);
                            KillToy(playerTwoToy);
                            continue;
                        }
                    }
                    else
                    {
                        // IF POSITION IS OUT OF GAME WORLD
                        // IS IT ON THE ENEMY BASE ? YES THEN WIN CONDITION HERE

                        ResolveToyTurn(playerOneToy);
                        ResolveToyTurn(playerTwoToy);
                    }
                }
            }
        }

        OnTurnCompleted();
    }

    private void OnTurnCompleted()
    {
        grid.UnresolveAll();
        inputManager.inputEnable = true;
    }

    private void ResolveAITurn()
    {
        SpawnAINewToy();
        PlaceNewToy(aiPlacingZone[Random.Range(0, aiPlacingZone.Count-1)].localPosition);
    }

    private void ResolveToyTurn(RushToy toy)
    {
        RushToy currentTileToy = toy;
        if (currentTileToy == null)
        {
            return;
        }

        if (currentTileToy.resolvedThisTurn)
        {
            return;
        }

        Vector2 calculatedNewPosition = currentTileToy.GetNextPosition();

        // IF POSITION IS OUT OF GAME WORLD
        // IS IT ON THE ENEMY BASE ? YES THEN WIN CONDITION HERE

        RushToy toyOnDestination = grid.GetToyAt(calculatedNewPosition);
        if (toyOnDestination != null)
        {
            if (toyOnDestination.team != currentTileToy.team)
            {
                RushCombatResolver.CombatResult result = combatResolver.ResolveConflict(currentTileToy, toyOnDestination);
                if (result.winner != null)
                {
                    if (result.winner == currentTileToy)
                    {
                        currentTileToy.Move();
                        currentTileToy.resolvedThisTurn = true;
                        toyOnDestination.resolvedThisTurn = true;
                        KillToy(toyOnDestination);
                        feedbacks.SpawnBattleFeedbackOnTile(calculatedNewPosition);
                        return;
                    }
                    else
                    {
                        currentTileToy.resolvedThisTurn = true;
                        KillToy(currentTileToy);
                        feedbacks.SpawnBattleFeedbackOnTile(currentTileToy.transform.localPosition);
                        return;
                    }
                }
                else
                {
                    currentTileToy.resolvedThisTurn = true;
                    toyOnDestination.resolvedThisTurn = true;
                    KillToy(currentTileToy);
                    KillToy(toyOnDestination);
                    feedbacks.SpawnBattleFeedbackOnTile(calculatedNewPosition);
                    return;
                }
            }
            else
            {
                // ALLY IN THE WAY
                currentTileToy.resolvedThisTurn = true;
                return;
            }
        }
        else
        {
            currentTileToy.Move();
            currentTileToy.resolvedThisTurn = true;
            return;
        }
    }
}
