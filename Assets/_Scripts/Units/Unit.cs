using Nodes.Tiles;
using UnityEngine;

[System.Flags]
public enum UnitType
{
    Sheep = 1 << 0, // 1
    Wolf = 1 << 1,  // 2
    Barn = 1 << 2,   // 4
    Obstacle = 1 << 3,   // 6
    Dog = 1 << 4   // 8
}


namespace Pathfinding._Scripts.Units {
    public class Unit : MonoBehaviour {
        [SerializeField] private SpriteRenderer _renderer;
        public NodeBase _actualNode;
        public Sprite _sprite;
        public bool _isNpc =  true;
        public int _movements;
        public UnitType _unitType;
        public LayerMask _canWalkLayerMask;
        public bool _canAttack;
        public UnitType _canAttackEnums;

        public void Init(Sprite sprite) {
            _renderer.sprite = sprite;
        }

        public bool CanAttackUnit(UnitType unitType)
        {
            return (_canAttackEnums & unitType) == unitType;
        }
    }
}
