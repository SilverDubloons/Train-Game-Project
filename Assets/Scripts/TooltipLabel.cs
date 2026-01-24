using UnityEngine;

public class TooltipLabel : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] Label label;
    [SerializeField] private ThemedImage themedImage;
    public Vector2 SetupTooltipLabel(TooltipData tooltipData, float yPos)
    {   // returns size of the tooltip
        visibilityObject.SetActive(true);
        rt.sizeDelta = new Vector2(r.i.interf.maxTooltipWidth - 6f, r.i.interf.referenceResolution.y);
        label.ChangeText(tooltipData.text);
        themedImage.SetUIElementType(tooltipData.uiElementType);
        rt.anchoredPosition = new Vector2(0, yPos);
        // Vector2 preferredSize = new Vector2(label.GetPreferredWidth(), label.GetPreferredHeight());
        Vector2 preferredSize = label.GetPreferredValuesString(rt.sizeDelta.x);
        rt.sizeDelta = preferredSize + new Vector2(6f, 4f); // add padding
        return rt.sizeDelta;
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
}
