using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        LevelManager._instance.onChangeDaycycle += MoveNPCs2;
    }

    public void OnDestroy()
    {
        GameplayManager.onUnitMove -= MoveNPCs;
        LevelManager._instance.onChangeDaycycle -= MoveNPCs2;
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
        yield return new WaitForSeconds(0.15f);
        GridManager.Instance._isNpcTurn = true;
        Unit[] units = FindObjectsOfType<Unit>();

        foreach (Unit unit in npcUnits)
        {
            if (playerUnits.Count == 0)
                continue;

            NodeBase node = unit._actualNode;
            (NodeBase targetNode, List<NodeBase> path, var costs) = Pathfinding._Scripts.Pathfinding.FindNearestEnemyNode(node, units, unit._unitType);
            if (targetNode == null)
            {
                continue;
            }
            if (path == null || path.Count == 0)
            {
                
                if (unit._canAttack && CanAttackUnit(unit, targetNode._tileUnit._unitType))
                {
                    yield return new WaitForSeconds(0.25f);
                    targetNode._tileUnit.GetComponent<Health>().TakeDamage(10);
                }
                continue;
            }

            yield return new WaitForSeconds(0.02f);
            node.NodeIsSelected();

            if (costs[costs.Count - 1] <= unit._movements)
            {
                if (unit._canAttack && CanAttackUnit(unit, targetNode._tileUnit._unitType))
                {
                    if (costs.Count > 1)
                    {
                        int index = 0;
                        while (index < costs.Count && costs[index] <= unit._movements)
                        {
                            index++;
                        }

                        path[path.Count - index + 1].NodeIsMoved();
                    }
                    
                    yield return new WaitForSeconds(0.2f);
                    path[0]._tileUnit.GetComponent<Health>().TakeDamage(10);
                }

                yield return new WaitForSeconds(0.01f);
                path[0].NodeIsMoved();
            }
            else
            {
                int index = 0;
                while (index < costs.Count && costs[index] <= unit._movements)
                {
                    index++;
                }

                path[path.Count - index].NodeIsMoved();
            }
        }

        GridManager.Instance._isNpcTurn = false;
        yield return null;
    }

    public void MoveNPCs2()
    {
        StartCoroutine(MoveNPCsCoroutine2());
    }

    IEnumerator MoveNPCsCoroutine2()
    {
        yield return new WaitForSeconds(0.5f);
        GridManager.Instance._isNpcTurn = true;
        Unit[] units = FindObjectsOfType<Unit>();

        foreach (Unit unit in npcUnits)
        {
            if (unit._unitType != UnitType.Wolf)
            {
                continue;
            }
            if (playerUnits.Count == 0)
                continue;

            NodeBase node = unit._actualNode;
            (NodeBase targetNode, List<NodeBase> path, var costs) = Pathfinding._Scripts.Pathfinding.FindNearestEnemyNode(node, units, unit._unitType);
            if (targetNode == null)
            {
                continue;
            }
            if (path == null || path.Count == 0)
            {

                if (unit._canAttack && CanAttackUnit(unit, targetNode._tileUnit._unitType))
                {
                    yield return new WaitForSeconds(0.25f);
                    targetNode._tileUnit.GetComponent<Health>().TakeDamage(10);
                }
                continue;
            }

            yield return new WaitForSeconds(0.02f);
            node.NodeIsSelected();

            if (costs[costs.Count - 1] <= unit._movements)
            {
                if (unit._canAttack && CanAttackUnit(unit, targetNode._tileUnit._unitType))
                {
                    if (costs.Count > 1)
                    {
                        int index = 0;
                        while (index < costs.Count && costs[index] <= unit._movements)
                        {
                            index++;
                        }

                        path[path.Count - index + 1].NodeIsMoved();
                    }

                    yield return new WaitForSeconds(0.05f);
                    path[0]._tileUnit.GetComponent<Health>().TakeDamage(10);
                }

                yield return new WaitForSeconds(0.01f);
                path[0].NodeIsMoved();
            }
            else
            {
                int index = 0;
                while (index < costs.Count && costs[index] <= unit._movements)
                {
                    index++;
                }

                path[path.Count - index].NodeIsMoved();
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
