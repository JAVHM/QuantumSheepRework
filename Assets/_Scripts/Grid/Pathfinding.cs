using System;
using System.Collections.Generic;
using System.Linq;
using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pathfinding._Scripts
{
    public static class Pathfinding
    {
        private static readonly Color PathColor = new Color(0.65f, 0.35f, 0.35f);
        private static readonly Color OpenColor = new Color(.4f, .6f, .4f);
        private static readonly Color ClosedColor = new Color(0.35f, 0.4f, 0.5f);

        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode)
        {
            Unit startUnit = startNode._tileUnit;
            var toSearch = new MinHeap<NodeBase>();
            toSearch.Add(startNode);
            var processed = new HashSet<NodeBase>();

            while (toSearch.Count > 0)
            {
                var current = toSearch.RemoveMin();

                processed.Add(current);

                if (current == targetNode)
                {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    var count = 100;
                    while (currentPathTile != startNode)
                    {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection;
                        count--;
                        if (count < 0) throw new Exception();
                    }
                    path.Reverse();
                    return path;
                }

                foreach (var neighbor in current.Neighbors.Where(t => t._isWalkable && t._tileUnit == null && (startUnit._canWalkLayerMask == (startUnit._canWalkLayerMask | (1 << t.gameObject.layer))) && !processed.Contains(t)))
                {
                    var inSearch = toSearch.Contains(neighbor);

                    var costToNeighbor = current.G + current.GetDistance(neighbor) + current._tileWalkValue * 10;

                    if (!inSearch || costToNeighbor < neighbor.G)
                    {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);

                        if (!inSearch)
                        {
                            neighbor.SetH(neighbor.GetDistance(targetNode));
                            toSearch.Add(neighbor);
                        }
                    }
                }
            }
            return null;
        }



        public static List<NodeBase> MarkReachableNodes(NodeBase startNode, int maxCost)
        {
            Unit startUnit = startNode._tileUnit;
            var toSearch = new SortedSet<(float cost, NodeBase node)>(Comparer<(float cost, NodeBase node)>.Create((a, b) =>
                a.cost == b.cost ? a.node.GetHashCode().CompareTo(b.node.GetHashCode()) : a.cost.CompareTo(b.cost)));
            var processed = new HashSet<NodeBase>();
            var reachableNodes = new List<NodeBase>();
            var nodeCosts = new Dictionary<NodeBase, float> { { startNode, 0 } };

            toSearch.Add((0, startNode));
            processed.Add(startNode);

            while (toSearch.Any())
            {
                var (currentCost, current) = toSearch.Min;
                toSearch.Remove(toSearch.Min);

                if (currentCost > maxCost) continue;

                current.SetColor(Color.red);
                current._isInRange = true;
                reachableNodes.Add(current);

                Debug.Log(current.Neighbors.Where(t => t._isWalkable && t._tileUnit == null));
                foreach (var neighbor in current.Neighbors.Where(t => t._isWalkable && (startUnit._canWalkLayerMask == (startUnit._canWalkLayerMask | (1 << t.gameObject.layer))) && t._tileUnit == null))
                {
                    var costToNeighbor = currentCost + neighbor._tileWalkValue;

                    if (!nodeCosts.ContainsKey(neighbor) || costToNeighbor < nodeCosts[neighbor])
                    {
                        nodeCosts[neighbor] = costToNeighbor;
                        if (processed.Add(neighbor))
                        {
                            toSearch.Add((costToNeighbor, neighbor));
                        }
                    }
                }
            }

            return reachableNodes;
        }

        public static List<NodeBase> IsReachableNodes(NodeBase startNode, int maxCost)
        {
            var toSearch = new SortedSet<(float cost, NodeBase node)>(Comparer<(float cost, NodeBase node)>.Create((a, b) =>
                a.cost == b.cost ? a.node.GetHashCode().CompareTo(b.node.GetHashCode()) : a.cost.CompareTo(b.cost)));
            var processed = new HashSet<NodeBase>();
            var reachableNodes = new List<NodeBase>();
            var nodeCosts = new Dictionary<NodeBase, float> { { startNode, 0 } };
            var startUnit = startNode._tileUnit;

            toSearch.Add((0, startNode));
            processed.Add(startNode);

            while (toSearch.Any())
            {
                var (currentCost, current) = toSearch.Min;
                toSearch.Remove(toSearch.Min);

                if (currentCost > maxCost) continue;

                reachableNodes.Add(current);

                foreach (var neighbor in current.Neighbors.Where(t => (t._isWalkable && t._tileUnit == null && (startUnit._canWalkLayerMask == (startUnit._canWalkLayerMask | (1 << t.gameObject.layer))) && !processed.Contains(t))))
                {
                    var costToNeighbor = currentCost + neighbor._tileWalkValue;

                    if (!nodeCosts.ContainsKey(neighbor) || costToNeighbor < nodeCosts[neighbor])
                    {
                        nodeCosts[neighbor] = costToNeighbor;
                        if (processed.Add(neighbor))
                        {
                            toSearch.Add((costToNeighbor, neighbor));
                        }
                    }
                }
            }

            return reachableNodes;
        }


        public static void MarkReachableNodesInFourDirections(NodeBase startNode, int maxSteps)
        {
            Unit startUnit = startNode._tileUnit;
            var directions = new List<Vector2>
            {
                new Vector2(0, 1),  // Up
                new Vector2(0, -1), // Down
                new Vector2(1, 0),  // Right
                new Vector2(-1, 0)  // Left
            };

            var processed = new HashSet<NodeBase>();

            foreach (var direction in directions)
            {
                var toSearch = new Queue<(NodeBase node, int steps)>();
                toSearch.Enqueue((startNode, 0));
                processed.Add(startNode);

                while (toSearch.Any())
                {
                    var (current, currentSteps) = toSearch.Dequeue();

                    if (currentSteps >= maxSteps) continue;

                    var nextPos = current.Coords.Pos + direction;
                    var neighbor = GetNeighborAtPosition(nextPos);

                    if (neighbor != null && neighbor._isWalkable && neighbor._tileUnit == null && (startUnit._canWalkLayerMask == (startUnit._canWalkLayerMask | (1 << neighbor.gameObject.layer))) && !processed.Contains(neighbor))
                    {
                        neighbor.SetColor(Color.red);
                        processed.Add(neighbor);
                        toSearch.Enqueue((neighbor, currentSteps + 1));
                    }
                }
            }
        }

        private static NodeBase GetNeighborAtPosition(Vector2 position)
        {
            return GridManager.Instance.GetTileAtPosition(position);
        }

        public static (NodeBase target, List<NodeBase> path, List<int> costs) FindNearestEnemyNode(NodeBase startNode, Unit[] units, UnitType unitType)
        {
            Unit startUnit = startNode._tileUnit;
            NodeBase targetNode = null;
            List<Unit> validUnits = new List<Unit>();

            // Primero, obtenemos una lista de unidades que pueden ser atacadas y calculamos sus distancias
            foreach (Unit unit in units)
            {
                if (unit != startNode._tileUnit && startUnit.CanAttackUnit(unit._unitType) && unit != null)
                {
                    float distance = Vector3.Distance(startNode.gameObject.transform.position, unit.transform.position);
                    validUnits.Add(unit);
                }
            }

            // Ordenamos las unidades por distancia ascendente
            validUnits = validUnits.OrderBy(unit => Vector3.Distance(startNode.gameObject.transform.position, unit.transform.position)).ToList();

            foreach (var unit in validUnits)
            {
                targetNode = unit._actualNode;

                var toSearch = new List<NodeBase>() { startNode };
                var processed = new List<NodeBase>();

                while (toSearch.Any())
                {
                    var current = toSearch[0];
                    foreach (var t in toSearch)
                        if (t.F < current.F || t.F == current.F && t.H < current.H) current = t;

                    processed.Add(current);
                    toSearch.Remove(current);

                    current.SetColor(ClosedColor);

                    if (current == targetNode)
                    {
                        // Se encontró un camino al targetNode más cercano
                        var currentPathTile = targetNode;
                        var path = new List<NodeBase>();
                        var costs = new List<int>();
                        var acumCosts = new List<int>();
                        var count = 100;
                        var acumCost = 0;
                        while (currentPathTile != startNode)
                        {
                            path.Add(currentPathTile);
                            costs.Add(currentPathTile._tileWalkValue);
                            currentPathTile = currentPathTile.Connection;
                            count--;
                            if (count < 0) throw new Exception();
                        }

                        costs.Reverse();
                        foreach (int c in costs)
                        {
                            acumCost += c;
                            acumCosts.Add(acumCost);
                        }

                        foreach (var tile in path) tile.SetColor(PathColor);
                        startNode.SetColor(PathColor);
                        return (targetNode, path, acumCosts);
                    }

                    // Procesar vecinos
                    foreach (var neighbor in current.Neighbors.Where(t => (t._isWalkable && t._tileUnit == null &&
                    (startUnit._canWalkLayerMask == (startUnit._canWalkLayerMask | (1 << t.gameObject.layer))) &&
                    !processed.Contains(t)) || t == targetNode))
                    {
                        var inSearch = toSearch.Contains(neighbor);

                        var costToNeighbor = current.G + current.GetDistance(neighbor) + current._tileWalkValue * 10;

                        if (!inSearch || costToNeighbor < neighbor.G)
                        {
                            neighbor.SetG(costToNeighbor);
                            neighbor.SetConnection(current);

                            if (!inSearch)
                            {
                                neighbor.SetH(neighbor.GetDistance(targetNode));
                                toSearch.Add(neighbor);
                                neighbor.SetColor(OpenColor);
                            }
                        }
                    }
                }
            }

            // Si no se encuentra ningún camino para ninguna de las unidades
            return (null, null, null);
        }


        public static List<NodeBase> MarkNodesInRange(NodeBase startNode, int range)
        {
            var toSearch = new Queue<(NodeBase node, int distance)>();
            var processed = new HashSet<NodeBase>();
            var markedNodes = new List<NodeBase>();

            toSearch.Enqueue((startNode, 0));
            processed.Add(startNode);

            while (toSearch.Count > 0)
            {
                var (currentNode, currentDistance) = toSearch.Dequeue();

                if (currentDistance > range)
                    continue;

                currentNode.SetColor(Color.red);
                markedNodes.Add(currentNode);

                foreach (var neighbor in currentNode.Neighbors.Where(t => !processed.Contains(t)))
                {
                    processed.Add(neighbor);
                    toSearch.Enqueue((neighbor, currentDistance + 1));
                }
            }

            return markedNodes;
        }

    }
}

public class MinHeap<T> where T : IComparable<T>
{
    private List<T> _elements = new List<T>();

    public int Count => _elements.Count;

    public T Peek()
    {
        if (Count == 0)
            throw new InvalidOperationException("Heap is empty.");
        return _elements[0];
    }

    public void Add(T item)
    {
        _elements.Add(item);
        HeapifyUp(_elements.Count - 1);
    }

    public T RemoveMin()
    {
        if (Count == 0)
            throw new InvalidOperationException("Heap is empty.");

        var min = _elements[0];
        _elements[0] = _elements[_elements.Count - 1];
        _elements.RemoveAt(_elements.Count - 1);

        HeapifyDown(0);
        return min;
    }

    public bool Contains(T item)
    {
        return _elements.Contains(item);
    }

    private void HeapifyUp(int index)
    {
        while (index > 0 && _elements[index].CompareTo(_elements[Parent(index)]) < 0)
        {
            Swap(index, Parent(index));
            index = Parent(index);
        }
    }

    private void HeapifyDown(int index)
    {
        while (LeftChild(index) < _elements.Count)
        {
            int smallerChildIndex = LeftChild(index);
            if (RightChild(index) < _elements.Count && _elements[RightChild(index)].CompareTo(_elements[LeftChild(index)]) < 0)
            {
                smallerChildIndex = RightChild(index);
            }

            if (_elements[index].CompareTo(_elements[smallerChildIndex]) < 0)
            {
                break;
            }

            Swap(index, smallerChildIndex);
            index = smallerChildIndex;
        }
    }

    private void Swap(int index1, int index2)
    {
        var temp = _elements[index1];
        _elements[index1] = _elements[index2];
        _elements[index2] = temp;
    }

    private int Parent(int index) => (index - 1) / 2;
    private int LeftChild(int index) => 2 * index + 1;
    private int RightChild(int index) => 2 * index + 2;
}
