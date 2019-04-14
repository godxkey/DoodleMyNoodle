using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    private int myHP = 5;
    private int ennemyHP = 5;

    private RushToy currentlySpawnedToy;

    private bool progressInTurn = true;

    private bool isGameOver = false;

    void Start()
    {
        ui.ModifyMyHealthDisplay(myHP);
        ui.ModifyEnnemyHealthDisplay(ennemyHP);
    }

    public void SpawnNewToy()
    {
        if (inputManager.inputEnable)
        {
            RushToy newToy = Instantiate(toyPrefab, entityContainer).GetComponent<RushToy>();
            currentlySpawnedToy = newToy;
            currentlySpawnedToy.team = 0;
            newToy.Spawn(this);
            inputManager.currentlyPlacingAToy = true;
        }
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
        if (inputManager.inputEnable)
        {
            Debug.Log("StartNewTurn");

            inputManager.inputEnable = false;

            ResolveAITurn();

            StartCoroutine(ResolveTurn());
        }
    }

    public void ToyReachedOutside(RushToy toy)
    {
        if (toy.team == 0)
        {
            ennemyHP -= toy.power;
        }
        else
        {
            myHP -= toy.power;
        }

        UpdateHealthDisplay();

        if ((ennemyHP <= 0) || (myHP <= 0))
        {
            isGameOver = true;
        }

        grid.ToyHasBeenDestroyed(toy);
        Destroy(toy.gameObject);
    }

    private void GameOver()
    {
        this.DelayedCall(5, () =>
         {
             ui.ResetUI();
             ennemyHP = 5;
             myHP = 5;
             UpdateHealthDisplay();
             grid.ResetGrid();
             ui.NextTurnToyListAnimation(() => {
                 isGameOver = false;
                 inputManager.inputEnable = true;
             });
         });

    }

    private void UpdateHealthDisplay()
    {
        ui.ModifyMyHealthDisplay(myHP);
        ui.ModifyEnnemyHealthDisplay(ennemyHP);
    }

    private IEnumerator ResolveTurn()
    {
        Debug.Log("ResolvingTurn");

        // Resolve Middle Row
        for (int x = 0; x < 7; x++)
        {
            RushToy currentTileToy = grid.GetToyAt(new Vector2(x, firstTileToTrigger.y));

            Debug.Log("Resolving - " + x + "," + firstTileToTrigger.y);

            yield return ResolveToyTurn(currentTileToy);
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
                    yield return StartCoroutine(ResolveToyTurn(playerTwoToy));
                    continue;
                }
                else if (playerTwoToy == null)
                {
                    yield return StartCoroutine(ResolveToyTurn(playerOneToy));
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
                                progressInTurn = false;
                                playerOneToy.Move(ProgressTurn);
                                StartCoroutine(WaitUntilTweenIsCompleted());
                                playerOneToy.resolvedThisTurn = true;
                                continue;
                            }
                            else
                            {
                                KillToy(playerOneToy);
                                progressInTurn = false;
                                playerTwoToy.Move(ProgressTurn);
                                StartCoroutine(WaitUntilTweenIsCompleted());
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
                        yield return StartCoroutine(ResolveToyTurn(playerOneToy));
                        yield return StartCoroutine(ResolveToyTurn(playerTwoToy));
                    }
                }
            }
        }

        OnTurnCompleted();
    }

    private void OnTurnCompleted()
    {
        if (isGameOver)
        {
            if (ennemyHP <= 0)
            {
                isGameOver = true;
                ui.DisplayYouWin();
                GameOver();
            }
            else if (myHP <= 0)
            {
                isGameOver = true;
                ui.DisplayEnnemyWin();
                GameOver();
            }
        }
        else
        {
            grid.UnresolveAll();
            ui.NextTurnToyListAnimation(() => { inputManager.inputEnable = true; });
        }
    }

    private void ResolveAITurn()
    {
        SpawnAINewToy();
        PlaceNewToy(aiPlacingZone[Random.Range(0, aiPlacingZone.Count - 1)].localPosition);
    }

    private IEnumerator ResolveToyTurn(RushToy toy)
    {
        RushToy currentTileToy = toy;
        if (currentTileToy != null)
        {
            if (!currentTileToy.resolvedThisTurn)
            {
                Vector2 calculatedNewPosition = currentTileToy.GetNextPosition();

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
                                progressInTurn = false;
                                currentTileToy.Move(ProgressTurn);
                                yield return StartCoroutine(WaitUntilTweenIsCompleted());
                                currentTileToy.resolvedThisTurn = true;
                                toyOnDestination.resolvedThisTurn = true;
                                KillToy(toyOnDestination);
                                feedbacks.SpawnBattleFeedbackOnTile(calculatedNewPosition);
                            }
                            else
                            {
                                currentTileToy.resolvedThisTurn = true;
                                KillToy(currentTileToy);
                                feedbacks.SpawnBattleFeedbackOnTile(currentTileToy.transform.localPosition);
                            }
                        }
                        else
                        {
                            currentTileToy.resolvedThisTurn = true;
                            toyOnDestination.resolvedThisTurn = true;
                            KillToy(currentTileToy);
                            KillToy(toyOnDestination);
                            feedbacks.SpawnBattleFeedbackOnTile(calculatedNewPosition);
                        }
                    }
                    else
                    {
                        // ALLY IN THE WAY
                        currentTileToy.resolvedThisTurn = true;
                    }
                }
                else
                {
                    progressInTurn = false;
                    currentTileToy.Move(ProgressTurn);
                    yield return StartCoroutine(WaitUntilTweenIsCompleted());
                    currentTileToy.resolvedThisTurn = true;
                }
            }
        }
    }

    IEnumerator WaitUntilTweenIsCompleted()
    {
        while (!progressInTurn)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    void ProgressTurn()
    {
        progressInTurn = true;
    }
}
