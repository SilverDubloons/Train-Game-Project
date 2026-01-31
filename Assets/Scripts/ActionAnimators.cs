using System.Collections.Generic;
using UnityEngine;

public class ActionAnimators : MonoBehaviour
{
    [SerializeField] private RectTransform spareActionAnimatorsParent;
    [SerializeField] private RectTransform activeActionAnimatorsParent;
    [SerializeField] private GameObject actionAnimatorPrefab;
    [SerializeField] private ActionAnimation[] actionAnimationsArray;
    private Dictionary<string, ActionAnimation> actionAnimations = new Dictionary<string, ActionAnimation>();
    public static ActionAnimators instance;
    public void SetupInstance()
    {
        instance = this;
        for (int i = 0; i < actionAnimationsArray.Length; i++)
        {
            actionAnimations[actionAnimationsArray[i].actionAnimationTag] = actionAnimationsArray[i];
        }
    }
    public ActionAnimator StartActionAnimation(string actionAnimationTag, Vector2 location)
    {
        ActionAnimation actionAnimation = actionAnimations[actionAnimationTag];
        return StartActionAnimation(actionAnimation, location);
    }
    public ActionAnimator StartActionAnimation(ActionAnimation actionAnimation, Vector2 location)
    {
        ActionAnimator actionAnimator = GetActionAnimator();
        actionAnimator.StartAnimation(actionAnimation, location, activeActionAnimatorsParent);
        return actionAnimator;
    }
    public ActionAnimator GetActionAnimator()
    {
        if (spareActionAnimatorsParent.childCount > 0)
        { 
            return spareActionAnimatorsParent.GetChild(spareActionAnimatorsParent.childCount - 1).GetComponent<ActionAnimator>();
        }
        return Instantiate(actionAnimatorPrefab, activeActionAnimatorsParent).GetComponent<ActionAnimator>();
    }
    public void RetireActionAnimator(ActionAnimator actionAnimator)
    {
        actionAnimator.SetVisibility(false);
        actionAnimator.SetParent(spareActionAnimatorsParent);
    }
}
