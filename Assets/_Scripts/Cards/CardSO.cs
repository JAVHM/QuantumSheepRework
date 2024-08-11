using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "Card", order = 1)]
public class CardSO : ScriptableObject
{
    public Vector2 movement;
    public int multiplierMin;
    public int multiplierMax;
    public Sprite sprite;
}
