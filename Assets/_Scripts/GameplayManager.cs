using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private bool _isSelected = false;
    private GridManager _gridManager;
    public static GameplayManager instance;

    public static Action<CardSO> onMouseDown;
    public static Action onMouseUp;

    public PauseMenu _pauseMenuUI;
    private PauseMenu _pauseMenuInstance;

    private UnitType validUnits = UnitType.Sheep | UnitType.Dog;

    private void Awake()
    {
        instance = this;
        _pauseMenuInstance = Instantiate(_pauseMenuUI);
        _gridManager = GridManager.Instance;
    }

    public void MouseDown(CardSO card)
    {
        if (onMouseDown != null)
        {
            onMouseDown(card);
        }
    }

    public void MouseUp()
    {
        if (onMouseUp != null)
        {
            onMouseUp();
        }
    }

    public bool Check(GameObject currentDraggable)
    {
        Vector3 roundedPosition = RoundPosition(currentDraggable.transform.position);
        currentDraggable.transform.position = roundedPosition;
        CardSO cardSO = currentDraggable.GetComponent<DraggableObjectScript>().cardData;

        NodeBase currentNode = GetNodeAtPosition(roundedPosition);
        if (currentNode == null)
            return false;
        if (currentNode != null && currentNode._tileUnit == null)
            return true;

        print("current node: " + currentNode._tileUnit);

        if (IsUnitValid(currentNode._tileUnit._unitType))
        {
            return HandleValidUnit(currentNode, cardSO);
        }

        return true;
    }

    private Vector3 RoundPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x),
            Mathf.Round(position.y),
            Mathf.Round(position.z)
        );
    }

    private NodeBase GetNodeAtPosition(Vector3 position)
    {
        Collider2D collider = Physics2D.OverlapPoint(position, LayerMask.GetMask("Tile"));
        if (collider != null)
        {
            return collider.gameObject.GetComponent<NodeBase>();
        }
        return null;
    }

    private bool IsUnitValid(UnitType unitType)
    {
        return (unitType & validUnits) != 0;
    }

    private bool HandleValidUnit(NodeBase currentNode, CardSO cardSO)
    {
        GridManager.Instance._currentNode = currentNode;
        NodeBase goalNode = FindTile(currentNode.Coords.Pos, cardSO);
        GridManager.Instance._goalNode = goalNode;

        if (CanUnitMoveToNode(currentNode._tileUnit, goalNode))
        {
            GridManager.Instance._currentUnit = currentNode._tileUnit;
            Unit tempUnit = currentNode._tileUnit;
            goalNode.NodeIsTeleported();

            if (goalNode._tileUnit._unitType == UnitType.Barn)
            {
                UnitsManager.Instance.SheepEnterBarn(tempUnit);
            }

            return true;
        }

        return false;
    }


    public NodeBase FindTile(Vector2 originPos, CardSO card)
    {
        HashSet<Vector2> triedPositions = new HashSet<Vector2>();
        int attempts = 1 + card.multiplierMax - card.multiplierMin;

        for (int i = 0; i < attempts; i++)
        {
            // Generate a possible position
            Vector2 addedPos;
            do
            {
                addedPos = card.movement * UnityEngine.Random.Range(card.multiplierMin, card.multiplierMax + 1);
            } while (triedPositions.Contains(addedPos));  // Ensure the position is unique

            // Add the position to the set of tried positions
            triedPositions.Add(addedPos);

            // Check if there is a NodeBase at the calculated position
            Collider2D hitCollider = Physics2D.OverlapPoint(originPos + addedPos);

            // If a NodeBase is found, return it
            if (hitCollider != null && hitCollider.gameObject.GetComponent<NodeBase>() != null)
            {
                Debug.Log("Objeto encontrado en la posición: " + hitCollider.gameObject.name);
                return hitCollider.gameObject.GetComponent<NodeBase>();
            }
        }

        // If no valid NodeBase is found after all attempts, return null
        return null;
    }


    public NodeBase FindTile(Vector2 originPos)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(originPos);

        // Si se encuentra un objeto en esa posición
        if (hitCollider != null)
        {
            if (hitCollider.gameObject.GetComponent<NodeBase>() != null)
                return hitCollider.gameObject.GetComponent<NodeBase>();
        }

        return null;
    }

    // Verifica si el goalNode y su tileUnit no pertenecen a los layers permitidos
    public bool CanUnitMoveToNode(Unit currentUnit, NodeBase goalNode)
    {
        return goalNode != null &&
            (goalNode._tileUnit == null ||
             ((goalNode._tileUnit != null) &&
              (1 << goalNode._tileUnit.gameObject.layer & currentUnit._canWalkLayerMask) != 0));
    }

    public void GameOver()
    {
        _pauseMenuInstance.GameOver();
    }
}
