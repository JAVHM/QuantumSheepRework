using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance;
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> npcUnits = new List<Unit>();

    public static event Action<Unit> OnSheepEnterBarn;

    void Awake() => Instance = this;

    public void Start()
    {
        GameplayManager.onUnitMove += MoveNPCs;
    }

    public void OnDestroy()
    {
        GameplayManager.onUnitMove -= MoveNPCs;
    }

    public void SheepEnterBarn(Unit unit)
    {
        unit.gameObject.SetActive(false);

        if (playerUnits.Contains(unit))
        {
            playerUnits.Remove(unit);
            Debug.Log($"Sheep removed. Remaining player units: {playerUnits.Count}");

            if (!playerUnits.Any(u => u.gameObject.CompareTag("Sheep")))
            {
                GameManager.Instance.NextScene();
            }
        }

        if (OnSheepEnterBarn != null)
        {
            OnSheepEnterBarn(unit);
        }
    }

    public void MoveNPCs()
    {
        StartCoroutine(MoveNPCsCoroutine());
    }

    IEnumerator MoveNPCsCoroutine()
    {
        GridManager.Instance._isNpcTurn = true;
        Unit[] units = FindObjectsOfType<Unit>();

        foreach (Unit unit in npcUnits)
        {
            print(unit.gameObject.name);
            if (playerUnits.Count != 0)
            {
                NodeBase node = unit._actualNode;
                (NodeBase targetNode, List<NodeBase> path, var costs) = Pathfinding._Scripts.Pathfinding.FindNearestEnemyNode(node, units, unit._unitType);
                print(targetNode.Coords.Pos);
                if (path != null)
                {
                    if (path.Count > 0)
                    {
                        yield return new WaitForSeconds(0.02f);
                        node.NodeIsSelected();
                        // yield return new WaitForSeconds(0.25f);
                        if (costs[costs.Count - 1] <= unit._movements)
                        {
                            print(targetNode._tileUnit._unitType);
                            if (unit._canAttack && CanAttackUnit(unit, targetNode._tileUnit._unitType))
                                path[path.Count - path.Count]._tileUnit.GetComponent<Health>().TakeDamage(10);
                            yield return new WaitForSeconds(0.01f);
                            path[path.Count - path.Count].NodeIsMoved();
                        }
                            
                        else
                        {
                            int index = 0;
                            foreach (int cost in costs)
                            {
                                // print(cost + " > " + unit._movements + "index: " + (index + 1));
                                if (cost > unit._movements)
                                    break;
                                index++;
                            }
                            path[path.Count - index].NodeIsMoved();
                        }
                    }
                    else
                    {
                        if(unit._canAttack && CanAttackUnit(unit, targetNode._tileUnit._unitType))
                            targetNode._tileUnit.GetComponent<Health>().TakeDamage(10);
                    }
                }
            }
        }
        GridManager.Instance._isNpcTurn = false;
        yield return null;
    }

    public void RemoveAndDestroyNpcUnits(Unit go)
    {
        if (npcUnits.Contains(go))
        {
            npcUnits.Remove(go);
            Destroy(go.gameObject);
        }
        else
        {
            Debug.LogWarning("The GameObject to be destroyed was not found in the list.");
        }
    }

    public void RemoveAndDestroyPlayerUnits(Unit go)
    {
        if (playerUnits.Contains(go))
        {
            playerUnits.Remove(go);
            Destroy(go.gameObject);
        }
        else
        {
            Debug.LogWarning("The GameObject to be destroyed was not found in the list.");
        }
    }

    public bool CanAttackUnit(Unit baseUnit, UnitType unitType)
    {
        return (baseUnit._canAttackEnums & unitType) == unitType;
    }

    public void MoveUnitToNPC(Unit unitToMove)
    {
        if (playerUnits.Remove(unitToMove))
        {
            npcUnits.Add(unitToMove);
        }
    }

    public void MoveUnitToPlayer(Unit unitToMove)
    {
        if (npcUnits.Remove(unitToMove))
        {
            playerUnits.Add(unitToMove);
        }
    }
}
