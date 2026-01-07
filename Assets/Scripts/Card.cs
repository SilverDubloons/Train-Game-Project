using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
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
    public void UpdateGraphics()
    {
        if (cardData.isSpecialCard)
        {
            detailImage.gameObject.SetActive(true);
            rankImage.gameObject.SetActive(false);
            bigSuitImage.gameObject.SetActive(false);
            detailImageRT.anchoredPosition = Vector2.zero;
            detailImage.color = Color.white;
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
            GameDeck.instance.DiscardCard(this.cardData);
        }
        if (addToDrawPileAtEnd)
        {
            GameDeck.instance.AddCardToDrawPile(this.cardData);
        }
        if (destroyAtEnd)
        {
            Destroy(gameObject);
        }
    }
    public void SetInteractability(bool interactable)
    { 
        canMove = interactable;
        scaleOnMouseOver.SetInteractability(interactable);
        tickOnMouseOver.SetInteractability(interactable);
    }
}
