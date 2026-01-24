using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;

public class GameDeck : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image drawPileCardBackImage;
    [SerializeField] private Label drawPileCountLabel;
    [SerializeField] private UnityEngine.UI.Image discardPileCardBackImage;
    [SerializeField] private Label discardPileCountLabel;
    [SerializeField] private Vector2 discardPileLocation;
    [SerializeField] private RectTransform discardingCardsParent;
    private List<CardData> drawPile = new List<CardData>();
    private List<Card> cardsInHand = new List<Card>();
    private List<Card> cardsBeingDiscarded = new List<Card>();
    private List<CardData> discardPile = new List<CardData>();
    private List<Card> cardsReturningToDrawPile = new List<Card>();
    private Card topDeckCard = null;
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
        DrawPileUpdated();
        DiscardPileUpdated();
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
    public Card GetNewCard()
    {
        if (spareCardsParent.childCount > 0)
        {
            return spareCardsParent.GetChild(spareCardsParent.childCount - 1).GetComponent<Card>();
        }
        else
        {
            Card newCard = Instantiate(r.i.cardPrefab, spareCardsParent).GetComponent<Card>();
            return newCard;
        }
    }
    public void DisableCard(Card card)
    { 
        card.SetVisibility(false);
        card.SetParent(spareCardsParent);
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
        Card cardToReturn = GetNewCard();
        cardToReturn.SetVisibility(true);
        cardToReturn.UpdateCardData(drawPile[drawPile.Count - 1]);
        drawPile.RemoveAt(drawPile.Count - 1);
        DrawPileUpdated();
        cardsInHand.Add(cardToReturn);
        return cardToReturn;
    }
    public void UpdateTopDeckCard()
    { 
    
    }
    public void AddCardToDiscardPile(Card card, CardData cardToDiscard)
    {
        cardsBeingDiscarded.Remove(card);
        discardPile.Add(cardToDiscard);
        DiscardPileUpdated();
    }
    public void AddCardToDrawPile(CardData cardToAdd)
    {
        drawPile.Add(cardToAdd);
    }
    public void DrawPileUpdated()
    {
        int drawPileCount = GetCardsInDrawPileCount();
        drawPileCountLabel.ChangeText(drawPileCount.ToString());
        drawPileCardBackImage.gameObject.SetActive(drawPile.Count > 0);
        // update deck preview if it's on screen
    }
    public void DiscardPileUpdated()
    {
        int discardPileCount = discardPile.Count;
        discardPileCountLabel.ChangeText(discardPileCount.ToString());
        discardPileCardBackImage.gameObject.SetActive(discardPileCount > 0);
        // update discard pile preview if it's on screen
    }
    public void RemoveCardFromCardsInHand(Card card)
    {
        cardsInHand.Remove(card);
    }
    public void StartDiscardCard(Card card)
    {
        cardsInHand.Remove(card);
        card.SetParent(discardingCardsParent);
        cardsBeingDiscarded.Add(card);
        card.StartMove(discardPileLocation, new Vector3(0, 0, 0), false, true, true, false);
    }
}
