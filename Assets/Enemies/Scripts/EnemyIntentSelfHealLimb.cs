using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
public class EnemyIntentSelfHealLimb : EnemyIntent
{
    public EnemyAbilitySelfHealLimb abilitySelfHealLimb;
    public override async Task ExecuteIntentAsync(EnemyInGame enemyInGame)
    {
        executingIntent = true;
        LimbInGame limbToHeal = enemyInGame.GetBestLimbToHeal();
        if (limbToHeal != null)
        {
            limbToHeal.Heal(GetHealAmout(enemyInGame));
        }
        executingIntent = false;
    }
    public override IntentType GetIntentType()
    {
        return IntentType.SelfHealLimb;
    }
    public int GetHealAmout(EnemyInGame enemyInGame)
    {
        return abilitySelfHealLimb.GetHealAmount(enemyInGame);
    }
    public EnemyIntentSelfHealLimb(EnemyAbilitySelfHealLimb enemyAbilitySelfHealLimb)
    {
        abilitySelfHealLimb = enemyAbilitySelfHealLimb;
        intentName = enemyAbilitySelfHealLimb.abilityName;
        icon = enemyAbilitySelfHealLimb.icon;
        tooltipDatas = enemyAbilitySelfHealLimb.GetTooltipDataList();
        enemyAbility = enemyAbilitySelfHealLimb;
    }
}