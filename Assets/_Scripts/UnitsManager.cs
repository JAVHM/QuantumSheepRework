using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance;
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> npcUnits = new List<Unit>();

    public static event Action<Unit> OnSheepEnterBarn;

    void Awake() => Instance = this;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !GridManager.Instance._isNpcTurn && playerUnits.Count != 0)
        {
            StartCoroutine(Test());
        }
    }

    public void SheepEnterBarn(Unit unit)
    {
        // unit.gameObject.SetActive(false);
        if (playerUnits.Contains(unit))
        {
            playerUnits.Remove(unit);
            Debug.Log($"Sheep removed. Remaining player units: {playerUnits.Count}");
        }

        if (OnSheepEnterBarn != null)
        {
            OnSheepEnterBarn(unit);
        }
    }

    IEnumerator Test()
    {
        GridManager.Instance._isNpcTurn = true;
        Unit[] units = FindObjectsOfType<Unit>();

        foreach (Unit unit in npcUnits)
        {
            if (playerUnits.Count != 0)
            {
                NodeBase node = unit._actualNode;
                (NodeBase targetNode, List<NodeBase> path, var costs) = Pathfinding._Scripts.Pathfinding.FindNearestEnemyNode(node, units, unit._unitType);
                if (path != null)
                {
                    if (path.Count > 0)
                    {
                        yield return new WaitForSeconds(0.01f);
                        node.NodeIsSelected();
                        // yield return new WaitForSeconds(0.25f);
                        if (costs[costs.Count - 1] <= unit._movements)
                        {
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
}
