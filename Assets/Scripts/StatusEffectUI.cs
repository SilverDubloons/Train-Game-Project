using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatusEffectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private Label label;
    private List<TooltipData> tooltipDatas;
    public void SetupStatusEffectUI(Status status, int magnitude, RectTransform newParent)
    {
        SetVisibility(true);
        rt.SetParent(newParent);
        image.sprite = StatusEffects.instance.GetStatusSprite(status);
        label.SetVisibility(true);
        label.ChangeText(magnitude.ToString());
        tooltipDatas = r.i.interf.ConvertStatusToTooltipDatas(status);
    }
    public float UpdateMagnitude(int newMagnitude)
    {
        label.ChangeText(newMagnitude.ToString());
        return GetIntentWidth();
    }
    public float GetIntentWidth()
    {
        float intentWidth = 12f;
        intentWidth += label.GetPreferredValuesString(9001f).x;
        return intentWidth;
    }
    public void SetVisibility(bool visibile)
    {
        visibilityObject.SetActive(visibile);
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Vector2 tooltipPosition = r.i.interf.GetCanvasPositionOfRectTransform(rt, GameManager.instance.gameplayCanvas) + new Vector2(0, rt.sizeDelta.y / 2 + 2f);
        Tooltip.instance.SetupTooltip(tooltipPosition, TooltipAlignment.Bottom, tooltipDatas);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Tooltip.instance.SetVisibility(false);
    }
    public void RetireStatusEffect()
    {
        StatusEffects.instance.RetireStatusEffectUI(this);
    }
    public void SetPosition(float x)
    { 
        rt.anchoredPosition = new Vector2 (x, 0);
    }
}