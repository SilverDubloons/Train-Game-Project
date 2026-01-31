using UnityEngine;
using System.Collections;
public class ActionAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private UnityEngine.UI.Image image;
    public bool animating;
    public void SetVisibility(bool visibile)
    { 
        visibilityObject.SetActive(visibile);
    }
    public void StartAnimation(ActionAnimation actionAnimation, Vector2 location, RectTransform newParent)
    {
        SetParent(newParent);
        SetVisibility(true);
        rt.anchoredPosition = location;
        StartCoroutine(PlayAnimation(actionAnimation));
    }
    private IEnumerator PlayAnimation(ActionAnimation actionAnimation)
    {
        animating = true;
        float t = 0;
        int index = 0;
        float spriteTime = 1f / actionAnimation.framesPerSecond;
        image.sprite = actionAnimation.sprites[index];
        SoundManager.instance.PlaySound(actionAnimation.audioClip, actionAnimation.audioClipVolumeFactor, actionAnimation.actionAnimationTag);
        while (index <= actionAnimation.sprites.Length)
        {
            t += Time.deltaTime;
            if (t >= spriteTime)
            {
                t -= spriteTime;
                index++;
                if (index < actionAnimation.sprites.Length)
                {
                    image.sprite = actionAnimation.sprites[index];
                }
            }
            yield return null;
        }
        ActionAnimators.instance.RetireActionAnimator(this);
        animating = false;
    }
    public void SetParent(RectTransform newParent)
    { 
        rt.SetParent(newParent);
    }
}
