using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
public class EnemyIntentUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private Label label;
    private EnemyIntent enemyIntent;
    private List<TooltipData> tooltipDatas;
    public float SetupIntentUI(EnemyIntent newEnemyIntent, RectTransform newParent, EnemyInGame enemyInGame)
    {   // returns desired width
        SetVisibility(true);
        rt.SetParent(newParent);
        enemyIntent = newEnemyIntent;
        image.sprite = newEnemyIntent.icon;
        tooltipDatas = newEnemyIntent.tooltipDatas;
        switch (enemyIntent.GetIntentType())
        {
            case IntentType.Attack:
                EnemyIntentAttack enemyIntentAttack = (EnemyIntentAttack)newEnemyIntent;
                label.gameObject.SetActive(true);
                label.ChangeText(enemyIntentAttack.GetDamage(enemyInGame).ToString());
            break;
            case IntentType.Shield:
                EnemyIntentShield enemyIntentShield = (EnemyIntentShield)newEnemyIntent;
                label.gameObject.SetActive(true);
                label.ChangeText(enemyIntentShield.GetMagnitude(enemyInGame).ToString());
                break;
            default:
                label.gameObject.SetActive(false);
            break;
        }
        return GetIntentWidth();
    }
    public float GetIntentWidth()
    {
        float intentWidth = 10f;
        if (label.gameObject.activeSelf)
        {
            intentWidth += 2f + label.GetPreferredValuesString(9001f).x;
        }
        return intentWidth;
    }
    public void SetPosition(float x)
    { 
        rt.anchoredPosition = new Vector2(x, 0);
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
    public void RetireIntent()
    {
        EnemyIntents.instance.RetireEnemyIntentUI(this);
    }
}
