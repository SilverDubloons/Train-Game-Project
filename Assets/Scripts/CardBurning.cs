using UnityEngine;

public class CardBurning : MonoBehaviour
{
    [SerializeField] private GameObject cardBurnPrefab;
    [SerializeField] private GameObject fireSpritePrefab;
    [SerializeField] private RectTransform spareFireSpriteParent;
    [SerializeField] private RectTransform spareCardBurnParent;
    [SerializeField] private RectTransform cardBurnParent;
    public Sprite[] fireSprites;
    public AnimationCurve fireSpriteCurve;
    public static int fireSpriteCount = 10;

    public static CardBurning instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public void DeactivateFireSprite(FireSprite fireSprite)
    {
        fireSprite.transform.SetParent(spareFireSpriteParent);
        fireSprite.SetVisibility(false);
    }
    public void DeactivateCardBurn(CardBurn cardBurn)
    {
        cardBurn.transform.SetParent(spareFireSpriteParent);
        cardBurn.SetVisibility(false);
    }
    public FireSprite GetFireSprite()
    {
        FireSprite fireSprite;
        if(spareFireSpriteParent.childCount > 0)
        {
            Transform fireSpriteTransform = spareFireSpriteParent.GetChild(spareFireSpriteParent.childCount - 1);
            fireSprite = fireSpriteTransform.GetComponent<FireSprite>();
        }
        else
        {
            GameObject newFireSpriteObj = Instantiate(fireSpritePrefab);
            fireSprite = newFireSpriteObj.GetComponent<FireSprite>();
        }
        return fireSprite;
    }
    private CardBurn GetCardBurn()
    {
        CardBurn cardBurn;
        if(spareCardBurnParent.childCount > 0)
        {
            Transform cardBurnTransform = spareCardBurnParent.GetChild(spareCardBurnParent.childCount - 1);
            cardBurn = cardBurnTransform.GetComponent<CardBurn>();
        }
        else
        {
            GameObject newCardBurnObj = Instantiate(cardBurnPrefab, spareCardBurnParent);
            cardBurn = newCardBurnObj.GetComponent<CardBurn>();
        }
        return cardBurn;
    }
    public void StartCardBurn(Card card)
    {
        GetCardBurn().StartCardBurn(card, cardBurnParent);
    }
}
