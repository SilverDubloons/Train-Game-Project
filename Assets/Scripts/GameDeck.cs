using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;

public class GameDeck : MonoBehaviour
{
    private List<CardData> drawPile = new List<CardData>();
    private List<Card> cardsInHand = new List<Card>();
    private List<Card> cardsBeingDiscarded = new List<Card>();
    private List<CardData> discardPile = new List<CardData>();
    private List<Card> cardsReturningToDrawPile = new List<Card>();
    private Card topDeckCard = null;
    public List<Card> spareCards = new List<Card>();
    public RectTransform spareCardsParent;

    public static GameDeck instance;
    public void SetupInstance()
    { 
        instance = this;
    }
    public static void Shuffle<T>(List<T> list, RandomNumbers randomNumbers)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = randomNumbers.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public void ShuffleDrawPile(RandomNumbers randomNumbers)
    {
        if(cardsReturningToDrawPile.Count > 0)
        {
            Logger.instance.Warning("Shuffling cards back into draw pile that are still moving: " + cardsReturningToDrawPile.Count);
        }
        Shuffle(drawPile, randomNumbers);
    }
    public void CreateStandardDeck()
    {
        for (int i = 0; i < 52; i++)
        { 
            CardData newCardData = new CardData(i % 13, (Suit)(i / 13));
            drawPile.Add(newCardData);
        }
    }
    public int GetCardsInDrawPileCount()
    {
        int count = 0;
        count += drawPile.Count;
        if(topDeckCard != null)
        {
            count++;
        }
        return count;
    }
    public int GetNumberOfStandardCardsInHand()
    {
        int count = 0;
        foreach (Card card in cardsInHand)
        {
            if(!card.cardData.isSpecialCard)
            {
                count++;
            }
        }
        return count;
    }
    public Card DrawTopCardOfDeck()
    {
        if(GetCardsInDrawPileCount() == 0)
        {
            Logger.instance.Warning("Tried to draw a card from an empty deck!");
            return null;
        }
        if (topDeckCard != null)
        {
            return topDeckCard;
        }
        if(spareCards.Count > 0)
        {
            Card cardToReturn = spareCards[spareCards.Count - 1];
            cardToReturn.SetVisibility(true);
            cardToReturn.UpdateCardData(drawPile[drawPile.Count - 1]);
            drawPile.RemoveAt(drawPile.Count - 1);
            spareCards.RemoveAt(spareCards.Count - 1);
            cardsInHand.Add(cardToReturn);
            return cardToReturn;
        }
        Card newCard = Instantiate(r.i.cardPrefab, spareCardsParent).GetComponent<Card>();
        newCard.SetVisibility(true);
        newCard.UpdateCardData(drawPile[drawPile.Count - 1]);
        drawPile.RemoveAt(drawPile.Count - 1);
        cardsInHand.Add(newCard);
        return newCard;
    }
    public void UpdateTopDeckCard()
    { 
    
    }
    public void DiscardCard(CardData cardToDiscard)
    {
        discardPile.Add(cardToDiscard);
    }
    public void AddCardToDrawPile(CardData cardToAdd)
    {
        drawPile.Add(cardToAdd);
    }
}
