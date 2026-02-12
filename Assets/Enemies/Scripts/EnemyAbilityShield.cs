using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;
[CreateAssetMenu(fileName = "EnemyAbilityShield", menuName = "Combat/EnemyAbilities/Enemy Ability Shield")]
public class EnemyAbilityShield : EnemyAbility
{
    public int magnitude;
    public override List<TooltipData> GetTooltipDataList()
    {
        List<TooltipData> tooltipDataList = new List<TooltipData>();
        tooltipDataList.Add(new TooltipData("Shield", UIElementType.tooltipName));
        return tooltipDataList;
    }
    public override AbilityType GetAbilityType()
    {
        return AbilityType.Shield;
    }
    public int GetMagnitude(EnemyInGame enemyInGame)
    {
        int totalMagnitude = magnitude;
        totalMagnitude += enemyInGame.statusEffects.GetStatusMagnitude(Status.ShieldBonus);
        return totalMagnitude;
    }
}