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

    private void Awake()
    {
        instance = this;
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

    public void Check(GameObject currentDraggable)
    {
        Vector3 roundedPosition = new Vector3(
                Mathf.Round(currentDraggable.transform.position.x),
                Mathf.Round(currentDraggable.transform.position.y),
                Mathf.Round(currentDraggable.transform.position.z)
            );

        LayerMask sheepLayer = LayerMask.GetMask("Tile");
        Collider2D collider = Physics2D.OverlapPoint(roundedPosition, sheepLayer);
        currentDraggable.transform.position = roundedPosition;

        CardSO cardSO = currentDraggable.GetComponent<DraggableObjectScript>().cardData;
        if (collider != null)
        {
            NodeBase currentNode = collider.gameObject.GetComponent<NodeBase>();
            if (currentNode._tileUnit != null && currentNode._tileUnit._unitType == UnitType.Sheep)
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
                }
            }
        }
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

    public bool CanUnitMoveToNode(Unit currentUnit, NodeBase goalNode)
    {
        // Verifica si el goalNode y su tileUnit no pertenecen a los layers permitidos
        if(goalNode._tileUnit != null)
        {
            print(goalNode._tileUnit.gameObject.layer);
            print(goalNode != null);
            print(goalNode._tileUnit == null);
            print(goalNode._tileUnit != null);
            print((1 << goalNode._tileUnit.gameObject.layer & currentUnit._canWalkLayerMask) == 0);
        }

        return goalNode != null &&
            (goalNode._tileUnit == null ||
             ((goalNode._tileUnit != null) &&
              (1 << goalNode._tileUnit.gameObject.layer & currentUnit._canWalkLayerMask) != 0));
    }
}
