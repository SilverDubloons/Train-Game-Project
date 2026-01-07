using UnityEngine;
using UnityEngine.EventSystems;

public class LimbInGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public RectTransform rt;
    public GameObject visibilityObject;
    public UnityEngine.UI.Image image;
    public string limbName;
    public LimbTag[] limbTags;
    private void Start()
    {
        image.alphaHitTestMinimumThreshold = 0.1f;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = r.i.themeManager.GetColorFromCurrentTheme(UIElementType.enemyMouseOver);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // Handle limb click, e.g., select limb or show details
    }
    public void SetupFromLimb(Limb limb)
    {
        limbName = limb.limbName;
        limbTags = limb.limbTags;
        image.sprite = limb.sprite;
        rt.sizeDelta = limb.size;
        rt.anchoredPosition = limb.location;
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
}