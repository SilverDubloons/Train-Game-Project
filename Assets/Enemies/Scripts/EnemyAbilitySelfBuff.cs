using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "EnemyAbilitySelfBuff", menuName = "Combat/EnemyAbilities/Enemy Ability Self Buff")]
public class EnemyAbilitySelfBuff : EnemyAbility
{
    public int magnitude;
    public Status status;
    public int GetMagnitude(EnemyInGame enemyInGame)
    {
        return magnitude;
    }
    public override List<TooltipData> GetTooltipDataList()
    {
        List<TooltipData> tooltipDataList = new List<TooltipData>();
        tooltipDataList.Add(new TooltipData(abilityName, UIElementType.tooltipName));
        tooltipDataList.Add(new TooltipData(r.i.interf.ConvertStatusToString(status), UIElementType.tooltipName));
        return tooltipDataList;
    }
    public override AbilityType GetAbilityType()
    {
        return AbilityType.SelfBuff;
    }
}