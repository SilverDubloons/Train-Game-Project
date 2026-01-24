using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class ToolInGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private Image toolImage;
    [SerializeField] private Label toolNameLabel;
    [SerializeField] private Label handStyleLabel;
    // [SerializeField] private RectTransform requiredCardsParent;
    [SerializeField] private ButtonPlus buttonPlus;
    private HandType handStyle;
    private int cardsRequired;
    private ToolTargetStyle toolTargetStyle;
    private int adjacentColumnsTarget = 0; // 1 means player can target column plus one column on each side etc
    private int areaOfEffectRadius = 0; // 1 means target plus adjacent tiles in all 4 cardinal directions
    private int damage;
    private ToolSpecialTag[] toolSpecialTags;
    private ToolInGame baseToolInGame;
    public Tool baseTool;
    private bool isUsable;
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
    public void SetInteractability(bool interactable)
    {
        buttonPlus.SetButtonEnabled(interactable);
    }
    public void SetupNewToolInGame(Tool newBaseTool)
    {
        baseToolInGame = null;
        baseTool = newBaseTool;
        toolImage.sprite = newBaseTool.toolIcon;
        toolNameLabel.ChangeText(newBaseTool.toolName);
        handStyleLabel.ChangeText(newBaseTool.GetHandStyleString());
        handStyle = baseTool.handStyle;
        cardsRequired = baseTool.cardsRequired;
        toolTargetStyle = baseTool.toolTargetStyle;
        adjacentColumnsTarget = baseTool.adjacentColumnsTarget;
        areaOfEffectRadius = baseTool.areaOfEffectRadius;
        damage = baseTool.damage;
        toolSpecialTags = baseTool.toolSpecialTags;
        isUsable = false;
        name = "ToolInGame_" + newBaseTool.toolName;
    }
    public void SetupFromToolInGame(ToolInGame toolInGame)
    {
        baseToolInGame = toolInGame;
        baseTool = toolInGame.baseTool;
        toolImage.sprite = toolInGame.toolImage.sprite;
        toolNameLabel.ChangeText(toolInGame.toolNameLabel.GetText());
        handStyleLabel.ChangeText(toolInGame.handStyleLabel.GetText());
        handStyle = baseToolInGame.handStyle;
        cardsRequired = baseToolInGame.cardsRequired;
        toolTargetStyle = baseToolInGame.toolTargetStyle;
        adjacentColumnsTarget = baseToolInGame.adjacentColumnsTarget;
        areaOfEffectRadius = baseToolInGame.areaOfEffectRadius;
        damage = baseToolInGame.damage;
        toolSpecialTags = baseToolInGame.toolSpecialTags;
        isUsable = true;
        name = "UsableToolInGame_" + toolInGame.baseTool.toolName;
    }
    public void Click()
    {
        if (isUsable)
        {
            bool aiming = HasSpecialTag(ToolSpecialTag.AlwaysAim);
            CombatManager.instance.SetTargetingTool(this, aiming);
        }
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        PreviewSelectableTargets();
        DisplayTooltip(r.i.interf.GetCanvasPositionOfRectTransform(rt, GameManager.instance.gameplayCanvas) + new Vector2(0, rt.sizeDelta.y / 2 + 4f), TooltipAlignment.Bottom);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        EndTargetPreview();
        Tooltip.instance.SetVisibility(false);
    }
    public void PreviewSelectableTargets()
    {
        if (CombatManager.instance.IsTargeting())
        {
            return;
        }
        bool aiming = HasSpecialTag(ToolSpecialTag.AlwaysAim);
        CombatArea.instance.PreviewSelectableTargets(toolTargetStyle, adjacentColumnsTarget, aiming);
    }
    public void EndTargetPreview()
    {
        if (CombatManager.instance.IsTargeting())
        {
            return;
        }
        CombatArea.instance.EndTargetPreview();
    }
    public int GetDamage(CombatSpace affectedSpace, EnemyInGame affectedEnemy, bool aiming = false, LimbInGame targetedLimb = null)
    {
        int totalDamage = damage;
        if (HasSpecialTag(ToolSpecialTag.DoubleDamageFrontRow) && affectedSpace.gridPosition.y == 1)
        {
            totalDamage *= 2;
        }
        if (HasSpecialTag(ToolSpecialTag.DoubleDamageBackRow) && affectedSpace.gridPosition.y == CombatArea.instance.currentBoardSize.y - 1)
        {
            totalDamage *= 2;
        }
        return totalDamage;
    }
    public int GetAreaOfEffect()
    {
        return areaOfEffectRadius;
    }
    public bool HasSpecialTag(ToolSpecialTag toolSpecialTag)
    {
        return toolSpecialTags.Contains(toolSpecialTag);
    }
    public int GetAdjacentColumnsTarget()
    {
        return adjacentColumnsTarget;
    }
    public int GetAreaOfEffectRadius()
    {
        return areaOfEffectRadius;
    }
    public ToolTargetStyle GetToolTargetStyle()
    {
        return toolTargetStyle;
    }
    public string GetToolTargetStyleString()
    {
        switch (toolTargetStyle)
        {
            case ToolTargetStyle.FirstInColumn:
                return "First in Column";
            case ToolTargetStyle.LastInColumn:
                return "Last in Column";
            case ToolTargetStyle.EntireColumn:
                return "Entire Column";
            case ToolTargetStyle.AnyInColumn:
                return "Any in Column";
            case ToolTargetStyle.Self:
                return "Self";
            default:
                return "GetToolTargetStyleString ERROR";
        }
    }
    public string GetToolSpecialTagString(ToolSpecialTag toolSpecialTag)
    { 
        switch(toolSpecialTag)
        {
            case ToolSpecialTag.AlwaysAim:
                return "Aim";
            case ToolSpecialTag.DoubleDamageFrontRow:
                return "Double Damage Front Row";
            case ToolSpecialTag.DoubleDamageBackRow:
                return "Double Damage Back Row";
            default:
                return "GetToolSpecialTagString ERROR";
        }
    }
    private void DisplayTooltip(Vector2 position, TooltipAlignment alignment)
    {
        List<TooltipData> tooltipDatas = new List<TooltipData>();
        if (damage != 0)
        {
            tooltipDatas.Add(new TooltipData($"{damage} Damage", UIElementType.tooltipDamage));
        }
        tooltipDatas.Add(new TooltipData(GetToolTargetStyleString(), UIElementType.tooltipTargetStyle));
        if(adjacentColumnsTarget > 0)
        {
            tooltipDatas.Add(new TooltipData($"{adjacentColumnsTarget} Range", UIElementType.tooltipSpecial));
        }
        if(areaOfEffectRadius > 0)
        {
            tooltipDatas.Add(new TooltipData($"{areaOfEffectRadius} AOE", UIElementType.tooltipSpecial));
        }
        for(int i = 0; i < toolSpecialTags.Length; i++)
        {
            tooltipDatas.Add(new TooltipData(GetToolSpecialTagString(toolSpecialTags[i]), UIElementType.tooltipSpecial));
        }
        Tooltip.instance.SetupTooltip(position, alignment, tooltipDatas);
    }
}
