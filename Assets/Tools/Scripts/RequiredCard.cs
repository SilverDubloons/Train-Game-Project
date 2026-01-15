using UnityEngine;
using UnityEngine.UI;

public class RequiredCard : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private Image requiredRankImage;
    [SerializeField] private Image requiredSuitImage;

    public void SetVisibility(bool visible)
    { 
        visibilityObject.SetActive(visible);
    }
}
