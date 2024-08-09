using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private bool _isSelected = false;
    private GridManager _gridManager;
    private Camera _mainCamera;

    private void Awake()
    {
        _gridManager = GridManager.Instance;
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!_gridManager._isNpcTurn && !_gridManager._isUnitMoving && Input.GetMouseButtonDown(0))
        {
            HandleMapClick();
        }
    }

    private void HandleMapClick()
    {
        NodeBase node = GetNodeUnderMouse();

        if (node != null)
        {
            if (node._tileUnit != null && node._tileUnit._team == 1)
            {
                HandleUnitSelection(node);
            }
            else if (_isSelected)
            {
                HandleNodeInteraction(node);
            }
        }
    }

    private void HandleUnitSelection(NodeBase node)
    {
        if (!_isSelected)
        {
            node.NodeIsSelected();
            _isSelected = true;
        }
        else
        {
            _gridManager._currentNode.NodeIsUnselected();
            node.NodeIsSelected();
        }
    }

    private void HandleNodeInteraction(NodeBase node)
    {
        if (node._isWalkable && node._isInRange)
        {
            HandleWalkableNode(node);
        }
        else if (_gridManager._currentNode.Neighbors.Contains(node) && node._tileUnit != null)
        {
            node._tileUnit.GetComponent<Health>().TakeDamage(10);
            DeselectNode(node);
        }
    }

    private void HandleWalkableNode(NodeBase node)
    {
        if (_gridManager._currentNode == node || node._tileUnit != null)
        {
            node.NodeIsUnselected();
        }
        else
        {
            node.NodeIsMoved();
        }
        _isSelected = false;
    }

    private void DeselectNode(NodeBase node)
    {
        node.NodeIsUnselected();
        _isSelected = false;
    }

    private NodeBase GetNodeUnderMouse()
    {
        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        return hit.collider != null ? hit.collider.gameObject.GetComponent<NodeBase>() : null;
    }
}
