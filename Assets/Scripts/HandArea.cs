using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.XR;
using UnityEngine;

public class HandArea : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private RectTransform handCardsParent;
    [SerializeField] private Vector2 drawPileLocation;
    [SerializeField] private ButtonPlus sortByRankButton;
    [SerializeField] private ButtonPlus sortBySuitButton;
    private int alwaysSortType = 0; // 0 = none, 1 = rank, 2 = suit
    private readonly float timeBetweenDraws = 0.2f;
    private bool drawingCards = false;
    private IEnumerator drawCardsCoroutine;
    private int siblingIndexOfLooseCard = -1;
    public static HandArea instance;
    private List<Card> selectedCards = new List<Card>();
    public void SetupInstance()
    {
        instance = this;
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
    public void SetInteractability(bool interactable)
    {
        // Implement interactability logic if needed
    }
    public void StartDrawCards(bool shuffleFirst = false)
    { 
        if(drawingCards)
        {
            StopCoroutine(drawCardsCoroutine);
        }
        drawCardsCoroutine = DrawCardsCoroutine(shuffleFirst);
        StartCoroutine(drawCardsCoroutine);
    }
    private IEnumerator DrawCardsCoroutine(bool shuffleFirst)
    {
        drawingCards = true;
        if (shuffleFirst)
        {
            GameDeck.instance.ShuffleDrawPile(RNG.instance.shuffle);
        }
        while ((GameDeck.instance.GetCardsInDrawPileCount() > 0) && GameDeck.instance.GetNumberOfStandardCardsInHand() < GameManager.instance.GetMaxHandSize())
        {
            SoundManager.instance.PlayCardPickupSound();
            Card topDeckCard = GameDeck.instance.DrawTopCardOfDeck();
            topDeckCard.SetParent(handCardsParent);
            topDeckCard.SetLocation(drawPileLocation);
            topDeckCard.GetRectTransform().SetSiblingIndex(0);
            topDeckCard.SetFaceUp(false);
            topDeckCard.StartFlip();
            ReorganizeHand();
            yield return new WaitForSeconds(timeBetweenDraws / Preferences.instance.gameSpeed);
        }
        GameDeck.instance.UpdateTopDeckCard();
        CombatManager.instance.SetCanEndTurn(true);
        drawingCards = false;
    }
    private void ReorganizeHand()
    {
        if (alwaysSortType != 0)
        {
            SortHand(alwaysSortType);
        }
        float distanceBetweenCards = 50f;
        float handAreaWidth = rt.rect.width;
        int visualCardsInHand = handCardsParent.childCount;
        if (siblingIndexOfLooseCard >= 0)
        {
            visualCardsInHand++;
        }
        if (visualCardsInHand * distanceBetweenCards > handAreaWidth)
        {
            distanceBetweenCards = handAreaWidth / visualCardsInHand;
        }
        for (int i = 0; i < visualCardsInHand; i++)
        {
            float xDestination = (visualCardsInHand - 1) * (distanceBetweenCards / 2f) - (visualCardsInHand - i - 1) * distanceBetweenCards;
            float yDestination = 0;
            Vector2 destination = new Vector2(xDestination, yDestination);
            Vector3 destinationRotation = Vector3.zero;
            handCardsParent.GetChild((i < siblingIndexOfLooseCard || siblingIndexOfLooseCard < 0) ? i : i - 1).GetComponent<Card>().StartMove(destination + rt.anchoredPosition, destinationRotation);
        }
    }
    private void SortHand(int sortType)
    {
        if (sortType != alwaysSortType)
        {
            ChangeAlwaysSortType(0);
        }
        List<Card> cardsInHand = GetCardsInHand();
        if (sortType == 1) // Sort by rank
        {
            cardsInHand.Sort((cardA, cardB) =>
            {
                if (cardA.cardData.isSpecialCard && cardB.cardData.isSpecialCard)
                {
                    if (cardA.cardData.specialCardType == cardB.cardData.specialCardType)
                    {
                        return cardA.GetRectTransform().anchoredPosition.x.CompareTo(cardB.GetRectTransform().anchoredPosition.x);
                    }
                    else
                    {
                        return cardA.cardData.specialCardType.CompareTo(cardB.cardData.specialCardType);
                    }
                }
                else if (cardA.cardData.isSpecialCard != cardB.cardData.isSpecialCard)
                {
                    return cardA.cardData.isSpecialCard ? 1 : -1;
                }
                else
                {
                    int rankComparison = cardA.cardData.rank.CompareTo(cardB.cardData.rank);
                    if (rankComparison != 0)
                    {
                        return rankComparison;
                    }
                    else if (cardA.cardData.suit == cardB.cardData.suit)
                    {
                        return cardA.GetRectTransform().anchoredPosition.x.CompareTo(cardB.GetRectTransform().anchoredPosition.x);
                    }
                    else
                    {
                        return r.i.interf.SuitOrderDictionary[cardA.cardData.suit].CompareTo(r.i.interf.SuitOrderDictionary[cardB.cardData.suit]);
                    }
                }
            });
        }
        else if(sortType == 2) // Sort by suit
        {
            cardsInHand.Sort((cardA, cardB) =>
            {
                if (cardA.cardData.isSpecialCard && cardB.cardData.isSpecialCard)
                {
                    if (cardA.cardData.specialCardType == cardB.cardData.specialCardType)
                    {
                        return cardA.GetRectTransform().anchoredPosition.x.CompareTo(cardB.GetRectTransform().anchoredPosition.x);
                    }
                    else
                    {
                        return cardA.cardData.specialCardType.CompareTo(cardB.cardData.specialCardType);
                    }
                }
                else if (cardA.cardData.isSpecialCard != cardB.cardData.isSpecialCard)
                {
                    return cardA.cardData.isSpecialCard ? 1 : -1;
                }
                else
                {
                    int suitComparison = r.i.interf.SuitOrderDictionary[cardA.cardData.suit].CompareTo(r.i.interf.SuitOrderDictionary[cardB.cardData.suit]);
                    if (suitComparison != 0)
                    {
                        return suitComparison;
                    }
                    else if (cardA.cardData.rank == cardB.cardData.rank)
                    {
                        return cardA.GetRectTransform().anchoredPosition.x.CompareTo(cardB.GetRectTransform().anchoredPosition.x);
                    }
                    else
                    {
                        return cardA.cardData.rank.CompareTo(cardB.cardData.rank);
                    }
                }
            });
        }
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.SetSiblingIndex(i);
        }
    }
    private void ChangeAlwaysSortType(int sortType)
    {
        alwaysSortType = sortType;
        sortByRankButton.ChangeSpecialState(sortType == 1 ? true : false);
        sortBySuitButton.ChangeSpecialState(sortType == 2 ? true : false);
    }
    private List<Card> GetCardsInHand()
    {
        // Depending on future needs, we could just use GameDeck's cardsInHand list
        List<Card> cardsInHand = new List<Card>();
        foreach (Transform child in handCardsParent)
        {
            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                cardsInHand.Add(card);
            }
        }
        return cardsInHand;
    }
    public bool CanSelectCards()
    { // this will return false if transitioning or using an ability, etc
        return true;
    }
    public void CardClickedOn(Card card)
    {
        if (!CanSelectCards())
        {
            return;
        }
        if (selectedCards.Contains(card))
        {            
            card.CardDeselected();
            selectedCards.Remove(card);
        }
        else
        {
            card.CardSelected();
            selectedCards.Add(card);
        }
        SelectedCardsUpdated();
    }
    private void SelectedCardsUpdated()
    {
        Tools.instance.DeterminePlayableTools(selectedCards);
        List<CombatSpace> movableSpaces = GameManager.instance.GetSpacesPlayerCanMoveToByHand(selectedCards);
        CombatArea.instance.SetMovableSpaces(movableSpaces);
    }
    public void HandPlayed()
    { 
        for(int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].CardPlayed();
        }
        SoundManager.instance.PlayCardSlideSound();
        selectedCards.Clear();
        ReorganizeHand();
        SelectedCardsUpdated();
    }
    public void TurnEnded()
    {
        List<Card> cardsInHand = GetCardsInHand();
        if (cardsInHand.Count > 0)
        {
            SoundManager.instance.PlayCardSlideSound();
        }
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].CardPlayed();
        }
        ReorganizeHand();
        SelectedCardsUpdated();
    }
}
