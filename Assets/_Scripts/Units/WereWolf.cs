using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WereWolf : MonoBehaviour
{
    public enum Werewolfstatus
    {
        villager,
        werewolf,
    }

    private Werewolfstatus status;
    public Sprite _villagerSprite;
    public Sprite _wereWolfSprite;
    public Unit _unit;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager._instance.onChangeDaycycle += ChangeStatus;
    }

    private void OnDestroy()
    {
        LevelManager._instance.onChangeDaycycle -= ChangeStatus;
    }

    public void ChangeStatus()
    {
        if(status == Werewolfstatus.villager)
        {
            status = Werewolfstatus.werewolf;
            GetComponentInChildren<SpriteRenderer>().sprite = _wereWolfSprite;
            this.gameObject.tag = "WereWolf";
            this.gameObject.layer = 14;
            _unit._unitType = UnitType.WereWolf;
            UnitsManager.Instance.MoveUnitToNPC(_unit);
        }
        else
        {
            status = Werewolfstatus.villager;
            GetComponentInChildren<SpriteRenderer>().sprite = _villagerSprite;
            this.gameObject.tag = "Dog";
            this.gameObject.layer = 13;
            _unit._unitType = UnitType.Dog;
            UnitsManager.Instance.MoveUnitToPlayer(_unit);
        }
    }
}
