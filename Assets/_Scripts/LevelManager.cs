using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public CardSO _selectedCardSO;
    public int _dayCicle;
    public int _dayCicleCurrent;
    public static LevelManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        GameplayManager.onMouseDown += SetCard;
    }

    public void OnDestroy()
    {
        GameplayManager.onMouseDown -= SetCard;
    }

    public void UpdateDaycicle()
    {
        
    }

    public void SetCard(CardSO newCard)
    {
        _selectedCardSO = newCard;
    }
}
