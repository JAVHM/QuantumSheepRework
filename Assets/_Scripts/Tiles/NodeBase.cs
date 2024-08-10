using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nodes.Tiles
{
    public abstract class NodeBase : MonoBehaviour, IComparable<NodeBase>
    {
        [Header("References")]

        [SerializeField] private Color _obstacleColor;
        [SerializeField] private Color _walkableColor;
        [SerializeField] protected SpriteRenderer _renderer;
        public int _tileWalkValue;
        public Unit _tileUnit;

        public ICoords Coords;
        public float GetDistance(NodeBase other) => Coords.GetDistance(other.Coords); // Helper to reduce noise in pathfinding
        public bool _isWalkable;
        private Color _defaultColor;

        public static event Action<NodeBase> OnMoveTile;
        public static event Action<NodeBase> OnTeleportTile;
        public static event Action<NodeBase> OnSelectTile;
        public static event Action<NodeBase> OnUnselectTile;

        public bool _isInRange = false;

        public virtual void Init(bool walkable, ICoords coords)
        {
            _isWalkable = walkable;

            if (walkable == false)
                gameObject.GetComponent<Collider2D>().enabled = false;

            _renderer.color = walkable ? _walkableColor : _obstacleColor;

            _defaultColor = _renderer.color;

            Coords = coords;
            transform.position = Coords.Pos;
        }

        public void NodeIsMoved()
        {
            GridManager.Instance._isTileMoved = false;
            OnMoveTile?.Invoke(this);
        }

        public void NodeIsTeleported()
        {
            GridManager.Instance._isTileMoved = false;
            OnTeleportTile?.Invoke(this);
        }

        public void NodeIsSelected()
        {
            _isInRange = true;
            GridManager.Instance._isTileMoved = true;
            OnSelectTile?.Invoke(this);
        }

        public void NodeIsUnselected()
        {
            GridManager.Instance._isTileMoved = false;
            OnUnselectTile?.Invoke(this);
        }

        public void NodeFindNearestUnit()
        {
            if (!_isWalkable) return;
            _isInRange = true;
            GridManager.Instance._isTileMoved = true;
            OnSelectTile?.Invoke(this);
        }

        public void ResetColor()
        {
            _renderer.color = _defaultColor;
        }

        #region Pathfinding

        [Header("Pathfinding")]
        [SerializeField]
        private TextMeshPro _fCostText;

        [SerializeField] private TextMeshPro _gCostText, _hCostText;
        public List<NodeBase> Neighbors { get; protected set; }
        public NodeBase Connection { get; private set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;

        public abstract void CacheNeighbors();

        public void SetConnection(NodeBase nodeBase)
        {
            Connection = nodeBase;
        }

        public void SetG(float g)
        {
            G = g;
            SetText();
        }

        public void SetH(float h)
        {
            H = h;
            SetText();
        }

        private void SetText()
        {
            _gCostText.text = G.ToString();
            _hCostText.text = H.ToString();
            _fCostText.text = F.ToString();
        }

        public void SetColor(Color color) => _renderer.color = color;

        public void RevertTile()
        {
            _renderer.color = _defaultColor;
            //_gCostText.text = "";
            //_hCostText.text = "";
            //_fCostText.text = "";
        }

        public int CompareTo(NodeBase other)
        {
            int compare = F.CompareTo(other.F);
            if (compare == 0)
            {
                compare = H.CompareTo(other.H);
            }
            return compare;
        }

        #endregion
    }
}

public interface ICoords
{
    public float GetDistance(ICoords other);
    public Vector2 Pos { get; set; }
}
