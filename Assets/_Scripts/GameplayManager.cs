using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using System;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private bool _isSelected = false;
    private GridManager _gridManager;
    public static GameplayManager instance;

    private void Awake()
    {
        instance = this;
        _gridManager = GridManager.Instance;
    }

    public void Check(GameObject currentDraggable)
    {
        Vector3 roundedPosition = new Vector3(
                Mathf.Round(currentDraggable.transform.position.x),
                Mathf.Round(currentDraggable.transform.position.y),
                Mathf.Round(currentDraggable.transform.position.z)
            );

        Collider2D collider = Physics2D.OverlapPoint(roundedPosition, 9);
        currentDraggable.transform.position = roundedPosition;
        CardSO cardSO = currentDraggable.GetComponent<DraggableObjectScript>().cardData;
        if (collider != null)
        {
            Debug.Log("Ya hay un objeto en la posición: " + roundedPosition);
            NodeBase currentNode = collider.gameObject.GetComponent<NodeBase>();
            if (currentNode._tileUnit != null && currentNode._tileUnit._team == 1)
            {
                GridManager.Instance._currentNode = currentNode;
                NodeBase goalNode = FindTile(currentNode.Coords.Pos, cardSO);
                GridManager.Instance._goalNode = goalNode;
                if(goalNode != null)
                {
                    GridManager.Instance._currentUnit = currentNode._tileUnit;
                    goalNode.NodeIsTeleported();
                }
            }
        }
    }

    public NodeBase FindTile(Vector2 originPos, CardSO card)
    {
        Vector2 addedPos = card.movement * UnityEngine.Random.Range(card.multiplierMin, card.multiplierMax + 1);
        // Realizar un OverlapPoint en la posición
        Collider2D hitCollider = Physics2D.OverlapPoint(originPos + addedPos);

        // Si se encuentra un objeto en esa posición
        if (hitCollider != null)
        {
            Debug.Log("Objeto encontrado: " + hitCollider.gameObject.name);
            if (hitCollider.gameObject.GetComponent<NodeBase>() != null)
                return hitCollider.gameObject.GetComponent<NodeBase>();
        }

        return null;
    }
}
