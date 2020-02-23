using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightClicker : GameMonoBehaviour
{
    private SimPawnComponent _playerPawn;
    private SimGridWalkerComponent _playerGridWalkerComponent;

    public override void OnGameUpdate()
    {
        if (_playerPawn == null)
            _playerPawn = PlayerIdHelpers.GetLocalSimPawnComponent();

        if (_playerPawn != null)
        {
            if (_playerGridWalkerComponent == null)
                _playerGridWalkerComponent = _playerPawn.GetComponent<SimGridWalkerComponent>();

            if (_playerGridWalkerComponent != null)
            {
                if (_playerGridWalkerComponent.WantsToWalk && IsMouseInsideHighlight(GetMousePositionOnTile()))
                {
                    if (Input.GetMouseButtonDown(0) && SimTurnManager.Instance.IsMyTurn(Team.Player))
                    {
                        SimTileId currentTileID = new SimTileId((int)transform.position.x, (int)transform.position.y);

                        _playerPawn.GetComponent<SimPlayerActions>().IncreaseValue(-CalculateAmountOfActionToMoveThere(_playerPawn.GetComponent<SimGridWalkerComponent>().TileId, currentTileID));

                        _playerPawn.GetComponent<SimGridWalkerComponent>().TryWalkTo(currentTileID);

                        _playerPawn.GetComponent<SimGridWalkerComponent>().WantsToWalk = false;
                        _playerPawn.GetComponent<SimGridWalkerComponent>().ChoiceMade = true;
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

    private int CalculateAmountOfActionToMoveThere(SimTileId start, SimTileId end)
    {
        int up = Mathf.Abs(end.Y - start.Y);
        int right = Mathf.Abs(end.X - start.X);
        return up + right;
    }
}
