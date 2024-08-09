using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using UnityEngine;
using System.Collections;
using System;

public class GameplayManager : MonoBehaviour
{
    private bool _isSelected = false;
    public Unit _currentUnit;
    private GridManager _gridManager;
    private Camera _mainCamera;
    private bool _canHandleInput = true;
    private float _inputDelay = 0.15f;
    public static event Action OnPlayerMove;

    private void Awake()
    {
        _gridManager = GridManager.Instance;
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _currentUnit = UnitsManager.Instance.playerUnits[0];
    }

    private void Update()
    {
        if (!_gridManager._isNpcTurn && !_gridManager._isUnitMoving && _canHandleInput && _currentUnit != null)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            HandleMapClick(3);
            StartCoroutine(InputCooldown());
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            HandleMapClick(1);
            StartCoroutine(InputCooldown());
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            HandleMapClick(0);
            StartCoroutine(InputCooldown());
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            HandleMapClick(2);
            StartCoroutine(InputCooldown());
        }
    }

    private IEnumerator InputCooldown()
    {
        _canHandleInput = false;
        yield return new WaitForSeconds(_inputDelay);
        _canHandleInput = true;
    }

    private void HandleMapClick(int direction)
    {
        NodeBase node = _currentUnit._actualNode;

        if (node._tileUnit != null && node._tileUnit._team == 1)
        {
            SelectNode(node);
            if (node._isWalkable)
            {
                MoveToNeighbor(node, direction);
            }
        }
    }

    private void SelectNode(NodeBase node)
    {
        if (!_isSelected)
        {
            node.NodeIsSelected();
            _isSelected = true;
        }
    }

    private void MoveToNeighbor(NodeBase node, int direction)
    {
        if (node.Neighbors[direction]._tileUnit != null)
        {
            node.Neighbors[direction]._tileUnit.GetComponent<Health>().TakeDamage(10);
        }
        else
        {
            node.Neighbors[direction].NodeIsMoved();
        }
        _isSelected = false;

        OnPlayerMove?.Invoke();
    }
}
