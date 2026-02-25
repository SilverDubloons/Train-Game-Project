using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public RectTransform rt;
    [SerializeField] private GameObject selectionGlow;
    [SerializeField] private Image rankImage;
    [SerializeField] private RectTransform rankImageRT;
    [SerializeField] private Image bigSuitImage;
    [SerializeField] private RectTransform bigSuitImageRT;
    [SerializeField] private Image detailImage;
    [SerializeField] private RectTransform detailImageRT;
    [SerializeField] private Image front;
    [SerializeField] private Image back;
    [SerializeField] private Image xImage;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private ScaleOnMouseOver scaleOnMouseOver;
    [SerializeField] private TickOnMouseOver tickOnMouseOver;
    public CardData cardData;
    private bool moving = false;
    private bool canMove = false;
    private IEnumerator moveCoroutine;
    private bool faceUp = false;
    private IEnumerator flipCoroutine;
    private bool flipping = false;
    public void UpdateGraphics()
    {
        if (cardData.isSpecialCard)
        {
            detailImage.gameObject.SetActive(true);
            rankImage.gameObject.SetActive(false);
            bigSuitImage.gameObject.SetActive(false);
            detailImageRT.anchoredPosition = Vector2.zero;
            detailImage.color = Color.white;
            // get detail image
            name = cardData.specialCardType.ToString();
            return;
        }
        rankImage.gameObject.SetActive(true);
        detailImage.gameObject.SetActive(true);
        rankImageRT.anchoredPosition = new Vector2(-12f, 10f);
        bigSuitImage.gameObject.SetActive(true);
        bigSuitImageRT.anchoredPosition = new Vector2(-12f, -5f);
        detailImageRT.anchoredPosition = new Vector2(6f, 0);
        name = r.i.interf.ConvertRankAndSuitToString(cardData.rank, cardData.suit);
        int suitInt = r.i.interf.SuitToInt(cardData.suit);
        detailImage.sprite = r.i.interf.detailSprites[cardData.rank + suitInt * 13];
        detailImage.SetNativeSize();
        bigSuitImage.sprite = r.i.interf.suitSprites[suitInt];
        if (suitInt < 4)
        {
            rankImage.sprite = r.i.interf.rankSprites[cardData.rank];
            rankImage.color = r.i.interf.suitColors[suitInt];
            bigSuitImage.color = r.i.interf.suitColors[suitInt];
            if (cardData.rank <= 8 || cardData.rank == 12)
            {
                detailImage.color = r.i.interf.suitColors[suitInt];
            }
            else
            {
                detailImage.color = Color.white;
            }
        }
        else
        {
            rankImage.sprite = r.i.interf.rankSprites[cardData.rank + 13];
            rankImage.color = Color.white;
            bigSuitImage.color = Color.white;
            detailImage.color = Color.white;
        }
        rankImage.SetNativeSize();
    }
    public void UpdateCardData(CardData newCardData)
    {
        cardData = newCardData;
        UpdateGraphics();
    }
    public void SetVisibility(bool isVisible)
    {
        visibilityObject.SetActive(isVisible);
    }
    public void SetParent(RectTransform newParent)
    {
        rt.SetParent(newParent);
    }
    public void SetLocation(Vector2 newLocation)
    {
        rt.anchoredPosition = newLocation;
    }
    public RectTransform GetRectTransform()
    {
        return rt;
    }
    public void StartMove(Vector2 destination, Vector3 destinationRotation, bool canMoveAtEnd = true, bool destroyAtEnd = false, bool discardAtEnd = false, bool addToDrawPileAtEnd = false)
    {
        if(moving)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(destination, destinationRotation, canMoveAtEnd, destroyAtEnd, discardAtEnd, addToDrawPileAtEnd);
        StartCoroutine(moveCoroutine);
    }
    private IEnumerator MoveCoroutine(Vector2 destination, Vector3 destinationRotation, bool canMoveAtEnd, bool destroyAtEnd, bool discardAtEnd, bool addToDrawPileAtEnd)
    {
        moving = true;
        SetInteractability(false);
        float t = 0f;
        float moveDuration = 0.3f / Preferences.instance.gameSpeed;
        Vector2 startingLocation = rt.anchoredPosition;
        Quaternion startingRotationQ = rt.localRotation;
        Quaternion targetRotationQ = Quaternion.Euler(destinationRotation);
        while (t < moveDuration)
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0f, moveDuration);
            rt.anchoredPosition = Vector2.Lerp(startingLocation, destination, t / moveDuration);
            rt.localRotation = Quaternion.Slerp(startingRotationQ, targetRotationQ, t / moveDuration);
            yield return null;
        }
        moving = false;
        if(canMoveAtEnd)
        {
            SetInteractability(true);
        }
        if (discardAtEnd)
        {
            GameDeck.instance.AddCardToDiscardPile(this, this.cardData);
        }
        if (addToDrawPileAtEnd)
        {
            // Logger.instance.Log($"{name} addToDrawPileAtEnd");
            GameDeck.instance.AddCardToDrawPile(this);
        }
        if (destroyAtEnd)
        {
            GameDeck.instance.DisableCard(this);
        }
    }
    public void SetInteractability(bool interactable)
    { 
        canMove = interactable;
        scaleOnMouseOver.SetInteractability(interactable);
        tickOnMouseOver.SetInteractability(interactable);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        CheckCheatInput();
        if (canMove)
        {
            HandArea.instance.CardClickedOn(this);
        }
    }
    private void CheckCheatInput()
    {
        if (!Preferences.instance.cheatsOn)
        {
            return;
        }
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }
        if (keyboard.sKey.isPressed)
        {
            ChangeSuit(Suit.Spade);
        }
        if (keyboard.cKey.isPressed)
        {
            ChangeSuit(Suit.Club);
        }
        if (keyboard.hKey.isPressed)
        {
            ChangeSuit(Suit.Heart);
        }
        if (keyboard.dKey.isPressed)
        {
            ChangeSuit(Suit.Diamond);
        }
        if (keyboard.rKey.isPressed)
        {
            ChangeSuit(Suit.Rainbow);
        }
        if (keyboard.digit2Key.isPressed)
        {
            ChangeRank(0);
        }
        if (keyboard.digit3Key.isPressed)
        {
            ChangeRank(1);
        }
        if (keyboard.digit4Key.isPressed)
        {
            ChangeRank(2);
        }
        if (keyboard.digit5Key.isPressed)
        {
            ChangeRank(3);
        }
        if (keyboard.digit6Key.isPressed)
        {
            ChangeRank(4);
        }
        if (keyboard.digit7Key.isPressed)
        {
            ChangeRank(5);
        }
        if (keyboard.digit8Key.isPressed)
        {
            ChangeRank(6);
        }
        if (keyboard.digit9Key.isPressed)
        {
            ChangeRank(7);
        }
        if (keyboard.digit0Key.isPressed)
        {
            ChangeRank(8);
        }
        if (keyboard.jKey.isPressed)
        {
            ChangeRank(9);
        }
        if (keyboard.qKey.isPressed)
        {
            ChangeRank(10);
        }
        if (keyboard.kKey.isPressed)
        {
            ChangeRank(11);
        }
        if (keyboard.aKey.isPressed)
        {
            ChangeRank(12);
        }
    }
    private void ChangeRank(int newRank)
    {
        cardData.rank = newRank;
        UpdateGraphics();
        HandArea.instance.CardsInHandUpdated();
    }
    private void ChangeSuit(Suit newSuit)
    { 
        cardData.suit = newSuit;
        UpdateGraphics();
        HandArea.instance.CardsInHandUpdated();
    }
    public void CardSelected()
    {
        SoundManager.instance.PlayCardPickupSound();
        selectionGlow.gameObject.SetActive(true);
        rt.anchoredPosition = rt.anchoredPosition + r.i.interf.selectedCardOffset;
    }
    public void CardDeselected()
    {
        SoundManager.instance.PlayCardDropSound();
        selectionGlow.gameObject.SetActive(false);
        rt.anchoredPosition = rt.anchoredPosition - r.i.interf.selectedCardOffset;
    }
    public void CardPlayed()
    {
        SetInteractability(false);
        selectionGlow.gameObject.SetActive(false);
        StartFlip();
        GameDeck.instance.StartDiscardCard(this);
    }
    public void ReturnCardToDrawPile()
    {
        SetInteractability(false);
        selectionGlow.gameObject.SetActive(false);
        StartFlip();
        GameDeck.instance.ReturnCardToDrawPile(this);
    }
    public void SetFaceUp(bool newFaceUp)
    {
        faceUp = newFaceUp;
        front.gameObject.SetActive(faceUp);
        back.gameObject.SetActive(!faceUp);
    }
    public void StartFlip()
    {
        if(flipping)
        {
            StopCoroutine(flipCoroutine);
        }
        flipCoroutine = FlipCoroutine();
        StartCoroutine(flipCoroutine);
    }
    private IEnumerator FlipCoroutine()
    { 
        flipping = true;
        float t = 0f;
        float flipDuration = 0.1f;
        Vector3 originalScale = rt.localScale;
        Vector3 destinationScale = rt.localScale;
        destinationScale.x = 0;
        while (t < flipDuration)
        {
            t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0f, flipDuration);
            float normalizedTime = t / flipDuration;
            rt.localScale = Vector3.Lerp(originalScale, destinationScale, normalizedTime);
            yield return null;
        }
        t = 0;
        SetFaceUp(!faceUp);
        while (t < flipDuration)
        {
            t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0f, flipDuration);
            float normalizedTime = t / flipDuration;
            rt.localScale = Vector3.Lerp(destinationScale, originalScale, normalizedTime);
            yield return null;
        }
        flipping = false;
    }
}