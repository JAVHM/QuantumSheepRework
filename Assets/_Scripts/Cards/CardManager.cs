using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<CardSO> availableCards;
    public static CardManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    public CardSO GetRandomCard()
    {
        int randomIndex = Random.Range(0, availableCards.Count);
        CardSO selectedCard = availableCards[randomIndex];
        availableCards.RemoveAt(randomIndex);
        return selectedCard;
    }

    public void ReturnCard(CardSO card)
    {
        availableCards.Add(card);
    }
}
