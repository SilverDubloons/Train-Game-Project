using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private TooltipLabel[] tooltipLabels;
    [SerializeField] private GameObject visibilityObject;
    public static Tooltip instance;
    public void SetupInstance()
    {
        instance = this;
        SetVisibility(false);
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
    private Vector2 GetPivotForAlignment(TooltipAlignment alignment)
    {
        switch (alignment)
        {
            case TooltipAlignment.Left: return new Vector2(0, 0.5f);
            case TooltipAlignment.Right: return new Vector2(1, 0.5f);
            case TooltipAlignment.Top: return new Vector2(0.5f, 1);
            case TooltipAlignment.Bottom: return new Vector2(0.5f, 0);
            case TooltipAlignment.TopLeft: return new Vector2(0, 1);
            case TooltipAlignment.TopRight: return new Vector2(1, 1);
            case TooltipAlignment.BottomLeft: return new Vector2(0, 0);
            case TooltipAlignment.BottomRight: return new Vector2(1, 0);
            default: return new Vector2(0.5f, 0.5f);
        }
    }
    public void SetupTooltip(Vector2 location, TooltipAlignment alignment, List<TooltipData> tooltipDatas)
    {
        SetVisibility(true);
        rt.pivot = GetPivotForAlignment(alignment);
        rt.anchoredPosition = location;
        float currentY = -2f;
        float largestWidth = 0f;
        for (int i = 0; i < tooltipLabels.Length; i++)
        {
            if (i < tooltipDatas.Count)
            {
                Vector2 tooltipSize = tooltipLabels[i].SetupTooltipLabel(tooltipDatas[i], currentY);
                currentY -= tooltipSize.y + 2f;
                if(tooltipSize.x > largestWidth)
                {
                    largestWidth = tooltipSize.x;
                }
            }
            else
            {
                tooltipLabels[i].SetVisibility(false);
            }
        }
        rt.sizeDelta = new Vector2(largestWidth + 8f, Mathf.Abs(currentY) + 4f);
    }
}
public struct TooltipData
{
    public string text;
    public UIElementType uiElementType;
    public TooltipData(string text, UIElementType uiElementType)
    {
        this.text = text;
        this.uiElementType = uiElementType;
    }
}
public enum TooltipAlignment
{
    Center,
    Left,
    Right,
    Top,
    Bottom,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}