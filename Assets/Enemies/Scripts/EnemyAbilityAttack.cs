using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "EnemyAbilityAttack", menuName = "Combat/EnemyAbilities/Enemy Ability Attack")]
public class EnemyAbilityAttack : EnemyAbility
{
    public int damage;
    public override List<TooltipData> GetTooltipDataList()
    { 
        List<TooltipData> tooltipDataList = new List<TooltipData>();
        tooltipDataList.Add(new TooltipData(abilityName, UIElementType.tooltipName));
        tooltipDataList.Add(new TooltipData($"{damage.ToString()} Damage", UIElementType.tooltipDamage));
        return tooltipDataList;
    }
    public override AbilityType GetAbilityType()
    {
        return AbilityType.Attack;
    }
}