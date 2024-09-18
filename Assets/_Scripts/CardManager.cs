using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<CardSO> availableCards;

    public void InitializeCards(CardSO[] cards)
    {
        if (availableCards == null)
        {
            availableCards = new List<CardSO>(cards);
        }
    }

    public CardSO GetRandomCard()
    {
        if (availableCards.Count == 0)
        {
            return null; // Si no quedan cartas disponibles, retorna null
        }

        int randomIndex = Random.Range(0, availableCards.Count);
        CardSO selectedCard = availableCards[randomIndex];
        availableCards.RemoveAt(randomIndex);
        return selectedCard;
    }

    public void ReturnCard(CardSO card)
    {
        if (!availableCards.Contains(card))
        {
            availableCards.Add(card);
        }
    }
}
