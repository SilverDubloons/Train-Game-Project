using System.Collections;
using UnityEngine;

public class FireSprite : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private UnityEngine.UI.Image image;
    private Vector2 initialAnchoredPosition;
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
    public void StartAnimation(int startingIndex, int endingIndex, float animationTime, RectTransform newParent)
    {
        SetParent(newParent);
        rt.anchoredPosition = new Vector2(UnityEngine.Random.Range(-24, 25), 0);
        initialAnchoredPosition = rt.anchoredPosition;
        StartCoroutine(FireAnimation(startingIndex, endingIndex, animationTime));
        StartCoroutine(MoveBackAndForth(animationTime));
    }
    private IEnumerator FireAnimation(int startingIndex, int endingIndex, float animationTime)
    {
        float t = 0;
        animationTime = animationTime / 2;
        while (t < animationTime)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / animationTime);
            int currentIndex = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(startingIndex, endingIndex, normalizedTime)), 0, CardBurning.fireSpriteCount - 1);
            image.sprite = CardBurning.instance.fireSprites[currentIndex];
            yield return null;
        }
        t = 0;
        while (t < animationTime)
        {
            t += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(t / animationTime);
            int currentIndex = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(endingIndex, startingIndex, normalizedTime)), 0, CardBurning.fireSpriteCount - 1);
            image.sprite = CardBurning.instance.fireSprites[currentIndex];
            yield return null;
        }
        CardBurning.instance.DeactivateFireSprite(this);
    }
    private IEnumerator MoveBackAndForth(float animationTime)
    { 
        Vector2Int currentPos = new Vector2Int();
        bool increasing = UnityEngine.Random.value > 0.5f;
        int animationSteps = 10;
        float stepDuration = animationTime / animationSteps;
        int index = 0;
        float t = UnityEngine.Random.Range(0, stepDuration);
        int maxDistance = 1;
        while (index < animationSteps)
        {
            while (t < stepDuration)
            {
                t += Time.deltaTime;
                yield return null;
            }
            currentPos += new Vector2Int(increasing ? 1 : -1, 0);
            rt.anchoredPosition = initialAnchoredPosition + currentPos;
            index++;
            if (Mathf.Abs(currentPos.x) == maxDistance)
            { 
                increasing = !increasing;
            }
        }
    }
    public void SetParent(RectTransform newParent)
    {
        rt.SetParent(newParent);
        rt.localScale = Vector3.one;
    }
}
