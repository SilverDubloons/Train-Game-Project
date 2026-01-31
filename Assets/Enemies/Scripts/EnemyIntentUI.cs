using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
public class EnemyIntentUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private UnityEngine.UI.Image image;
    private EnemyIntent enemyIntent;
    private List<TooltipData> tooltipDatas;

    public void SetupIntentUI(EnemyIntent newEnemyIntent, int index, RectTransform newParent)
    {
        SetVisibility(true);
        rt.SetParent(newParent);
        rt.anchoredPosition = new Vector2(index * 10f, 0);
        enemyIntent = newEnemyIntent;
        image.sprite = newEnemyIntent.icon;
        tooltipDatas = newEnemyIntent.tooltipDatas;
        switch (enemyIntent.GetIntentType())
        {
            case IntentType.Attack:

            break;
            case IntentType.Move:

            break;
        }
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
    public void RemoveIntent()
    {
        CombatManager.instance.RetireEnemyIntentUI(this);
    }
    public void MoveLeft()
    {
        rt.anchoredPosition = rt.anchoredPosition - new Vector2(rt.sizeDelta.x + 2f, 0);
    }
}
