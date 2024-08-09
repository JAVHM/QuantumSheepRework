using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nodes.Tiles;
using Pathfinding._Scripts.Grid.Scriptables;
using Pathfinding._Scripts.Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinding._Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance;

        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private ScriptableSquareGrid _scriptableGrid;
        [SerializeField] private bool _drawConnections;

        public Dictionary<Vector2, NodeBase> tiles { get; private set; }

        public List<Unit> _unitList = new List<Unit>();

        public NodeBase _currentNode, _goalNode, _selectCenterNode;
        private Unit _currentUnit;

        public bool _isTileMoved = false;
        public bool _isNpcTurn = false;
        public bool _isUnitMoving = false;

        private List<NodeBase> reacheableNodes, selectableNodes = new List<NodeBase>();

        public GameObject _customGrid;
        public Unit[] _customAllyUnits;
        public Unit[] _customEnemyUnits;

        void Awake() => Instance = this;

        private void Start()
        {
            if (_customGrid != null)
            {
                tiles = _scriptableGrid.SetGrid(_customGrid);
                SetUnits();
            }
            else
            {
                tiles = _scriptableGrid.GenerateGrid();
                SpawnUnits();
            }

            foreach (var tile in tiles.Values) tile.CacheNeighbors();

            NodeBase.OnMoveTile += TileSelected;
            NodeBase.OnSelectTile += TileMapped;
            NodeBase.OnUnselectTile += TileUnselected;
        }

        void SpawnUnits()
        {
            foreach (Unit unit in _unitList)
            {
                NodeBase randomNode = tiles.Where(t => t.Value._isWalkable && t.Value._tileUnit == null).OrderBy(t => Random.value).First().Value;
                Unit instanceUnit = Instantiate(unit, randomNode.Coords.Pos, Quaternion.identity);
                instanceUnit.Init(unit._sprite);
                randomNode._tileUnit = instanceUnit;
                instanceUnit._actualNode = randomNode;
                if (instanceUnit._team == 1)
                    UnitsManager.Instance.playerUnits.Add(instanceUnit);
                else
                    UnitsManager.Instance.npcUnits.Add(instanceUnit);
            }
        }

        void SetUnits()
        {
            foreach (Unit ally in _customAllyUnits)
            {
                UnitsManager.Instance.playerUnits.Add(ally);
            }
            foreach (Unit enemy in _customEnemyUnits)
            {
                UnitsManager.Instance.npcUnits.Add(enemy);
            }
        }

        private void OnDestroy() => NodeBase.OnMoveTile -= TileSelected;

        private void TileSelected(NodeBase nodeBase)
        {

            _goalNode = nodeBase;

            foreach (var t in tiles.Values) t.RevertTile();

            if (Pathfinding.IsReachableNodes(_currentNode, _currentNode._tileUnit._movements).Contains(_goalNode) || _isNpcTurn)
            {
                _isUnitMoving = true;

                List<NodeBase> path = Pathfinding.FindPath(_currentNode, _goalNode);

                if (path != null && path.Count > 0)
                {
                    // Iniciar la corrutina de movimiento
                    StartCoroutine(MoveUnitAlongPath(path));
                }
            }

            ResetReachebleNodes();
        }


        private IEnumerator MoveUnitAlongPath(List<NodeBase> path)
        {
            path.RemoveAt(0);

            var unitMover = _currentUnit.GetComponent<UnitMover>();

            _currentUnit.transform.position = _goalNode.transform.position;
            _currentNode._tileUnit = null;
            _currentNode = _goalNode;
            _currentNode._tileUnit = _currentUnit;
            _currentNode._tileUnit._actualNode = _currentNode;

            yield return StartCoroutine(unitMover.MoveAlongPath(path, 100f));

            _isUnitMoving = false;
        }

        public IEnumerator MoveAlongPath(List<NodeBase> path, float speed)
        {
            foreach (var node in path)
            {
                Vector3 startPosition = transform.position;
                Vector3 endPosition = node.transform.position;
                float journey = 0f;

                while (journey < 1f)
                {
                    journey += Time.deltaTime * speed;
                    transform.position = Vector3.Lerp(startPosition, endPosition, journey);
                    yield return null;
                }
            }
        }

        private void TileMapped(NodeBase nodeBase)
        {
            _currentNode = nodeBase;
            _currentUnit = nodeBase._tileUnit;

            foreach (var t in tiles.Values) t.RevertTile();

            reacheableNodes = Pathfinding.MarkReachableNodes(nodeBase, nodeBase._tileUnit._movements);
        }

        private void TileUnselected(NodeBase nodeBase)
        {
            foreach (var t in tiles.Values) t.RevertTile();
            ResetReachebleNodes();
            _currentNode = null;
            _isUnitMoving = false;
        }

        public void TestAreaAttack(NodeBase nodeBase)
        {
            if (_selectCenterNode == null)
            {
                _selectCenterNode = nodeBase;
                selectableNodes = Pathfinding.MarkNodesInRange(nodeBase, 3);
            }
            if (_selectCenterNode != nodeBase)
            {
                print("New node");
                ResetSelectableNodes();
                _selectCenterNode = nodeBase;
                selectableNodes = Pathfinding.MarkNodesInRange(nodeBase, 3);
            }

        }

        private void ResetReachebleNodes()
        {
            foreach (NodeBase n in reacheableNodes)
            {
                n._isInRange = false;
            }
        }

        private void ResetSelectableNodes()
        {
            foreach (NodeBase n in selectableNodes)
            {
                n.ResetColor();
            }
        }

        public NodeBase GetTileAtPosition(Vector2 pos) => tiles.TryGetValue(pos, out var tile) ? tile : null;

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !_drawConnections) return;
            Gizmos.color = Color.red;
            foreach (var tile in tiles)
            {
                if (tile.Value.Connection == null) continue;
                Gizmos.DrawLine((Vector3)tile.Key + new Vector3(0, 0, -1), (Vector3)tile.Value.Connection.Coords.Pos + new Vector3(0, 0, -1));
            }
        }
    }
}