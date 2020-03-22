using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightClicker : GameMonoBehaviour
{
    private SimPawnComponent _playerPawn;
    private SimGridWalkerComponent _playerGridWalkerComponent;
    private SimCharacterAttackComponent _playerCharacterAttackComponent;
    private SimCharacterHealComponent _playerCharacterHealComponent;

    public override void OnGameUpdate()
    {
        // PORT TO ECS

        //if (_playerPawn == null)
        //    _playerPawn = PlayerIdHelpers.GetLocalSimPawnComponent();

        if (_playerPawn != null)
        {
            if (_playerGridWalkerComponent == null)
                _playerGridWalkerComponent = _playerPawn.GetComponent<SimGridWalkerComponent>();

            if (_playerCharacterAttackComponent == null)
                _playerCharacterAttackComponent = _playerPawn.GetComponent<SimCharacterAttackComponent>();

            if (_playerCharacterHealComponent == null)
                _playerCharacterHealComponent = _playerPawn.GetComponent<SimCharacterHealComponent>();

            if (_playerGridWalkerComponent != null)
            {
                if (_playerGridWalkerComponent.WantsToWalk && IsMouseInsideHighlight(GetMousePositionOnTile()))
                {
                    if (SimTurnManager.Instance.IsMyTurn(Team.Player))
                    {
                        if (Input.GetMouseButtonDown(0)) 
                        {
                            SimTileId_OLD currentTileID = new SimTileId_OLD((int)transform.position.x, (int)transform.position.y);

                            _playerGridWalkerComponent.OnDestinationChoosen(currentTileID);

                            return;
                        }
                    }
                    else
                    {
                        _playerGridWalkerComponent.OnCancelWalkRequest();
                    }
                }
            }

            if (_playerCharacterAttackComponent != null)
            {
                if (_playerCharacterAttackComponent.WantsToAttack && IsMouseInsideHighlight(GetMousePositionOnTile()))
                {
                    if (SimTurnManager.Instance.IsMyTurn(Team.Player))
                    {
                        if (Input.GetMouseButtonDown(0)) 
                        {
                            SimTileId_OLD currentTileID = new SimTileId_OLD((int)transform.position.x, (int)transform.position.y);

                            _playerCharacterAttackComponent.OnAttackDestinationChoosen(currentTileID);

                            return;
                        }
                    }
                    else
                    {
                        _playerCharacterAttackComponent.OnCancelAttackRequest();
                    }
                }

                if (_playerCharacterAttackComponent.WantsToShootProjectile && IsMouseInsideHighlight(GetMousePositionOnTile()))
                {
                    if (SimTurnManager.Instance.IsMyTurn(Team.Player))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            SimTileId_OLD currentTileID = new SimTileId_OLD((int)transform.position.x, (int)transform.position.y);

                            _playerCharacterAttackComponent.OnShootDestinationChoosen(currentTileID);

                            return;
                        }
                    }
                    else
                    {
                        _playerCharacterAttackComponent.OnCancelShootRequest();
                    }
                }

                if(_playerCharacterHealComponent != null)
                {
                    if (_playerCharacterHealComponent.WantsToHeal && IsMouseInsideHighlight(GetMousePositionOnTile()))
                    {
                        if (SimTurnManager.Instance.IsMyTurn(Team.Player))
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                SimTileId_OLD currentTileID = new SimTileId_OLD((int)transform.position.x, (int)transform.position.y);

                                _playerCharacterHealComponent.OnHealDestinationChoosen(currentTileID);

                                return;
                            }
                        }
                        else
                        {
                            _playerCharacterHealComponent.OnCancelHealRequest();
                        }
                    }
                }
            }
        }
    }

    private Vector3 GetMousePositionOnTile()
    {
        Ray ray = CameraService.Instance.ActiveCamera.ScreenPointToRay(Input.mousePosition);

        float enter = 0.0f;
        if (SimTileManager.Instance.FloorPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return hitPoint;
        }

        return new Vector3();
    }

    private bool IsMouseInsideHighlight(Vector3 mousePos)
    {
        return mousePos.x <= transform.position.x + 0.5f
            && mousePos.x >= transform.position.x - 0.5f
            && mousePos.y <= transform.position.y + 0.5f
            && mousePos.y >= transform.position.y - 0.5f;
    }
}
