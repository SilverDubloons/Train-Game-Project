using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class CardBurn : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private RectTransform cardParent;
    [SerializeField] private RectTransform maskParent;
    [SerializeField] private RectTransform fireParent;
    public void SetVisibility(bool isVisible)
    {
        visibilityObject.SetActive(isVisible);
    }
    public void StartCardBurn(Card card, RectTransform newParent)
    { 
        rt.SetParent(newParent);
        rt.localScale = Vector3.one;
        rt.anchoredPosition = card.GetRectTransform().anchoredPosition;
        card.GetRectTransform().SetParent(cardParent);
        int numberOfFires = UnityEngine.Random.Range(12, 16);
        for (int i = 0; i < numberOfFires; i++)
        { 
            FireSprite fireSprite = CardBurning.instance.GetFireSprite();
            int startingIndex = UnityEngine.Random.Range(0, CardBurning.instance.fireSprites.Length / CardBurning.fireSpriteCount) * CardBurning.fireSpriteCount;
            if(fireSprite == null)
            {
                Logger.instance.Log("CardBurn.StartCardBurn: fireSprite is null");
                continue;
            }
            fireSprite.StartAnimation(startingIndex, startingIndex + CardBurning.fireSpriteCount, 1f, fireParent);
        }
        StartCoroutine(BurnCardAnimation(card));
    }
    private IEnumerator BurnCardAnimation(Card card)
    {
        float t = 0;
        float animationDuration = 1f;
        Vector2 initialSize = new Vector2(44f, 88f);
        Vector2 terminalSize = new Vector2(44f, 0f);
        while (t < animationDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / animationDuration);
            maskParent.sizeDelta = Vector2.Lerp(initialSize, terminalSize, CardBurning.instance.fireSpriteCurve.Evaluate(normalizedTime));
            yield return null;
        }
        Destroy(card.gameObject);
        CardBurning.instance.DeactivateCardBurn(this);
    }
}
