using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
[CreateAssetMenu(fileName = "EnemyAbilityHealLimb", menuName = "Combat/EnemyAbilities/Enemy Ability Heal Limb")]
public class EnemyAbilitySelfHealLimb : EnemyAbility
{
    public int healAmount;
    public int GetHealAmount(EnemyInGame enemyInGame)
    {
        return healAmount;
    }
    public override List<TooltipData> GetTooltipDataList()
    {
        List<TooltipData> tooltipDataList = new List<TooltipData>();
        tooltipDataList.Add(new TooltipData(abilityName, UIElementType.tooltipName));
        return tooltipDataList;
    }
    public override AbilityType GetAbilityType()
    {
        return AbilityType.SelfHealLimb;
    }
}