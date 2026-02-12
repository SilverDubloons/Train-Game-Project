using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
public class EnemyIntentSelfBuff : EnemyIntent
{
    public EnemyAbilitySelfBuff abilitySelfBuff;
    public override async Task ExecuteIntentAsync(EnemyInGame enemyInGame)
    { 
        executingIntent = true;
        enemyInGame.statusEffects.AddStatus(GetStatus(), GetMagnitude(enemyInGame));
        executingIntent = false;
    }
    public override IntentType GetIntentType()
    {
        return IntentType.SelfBuff;
    }
    public int GetMagnitude(EnemyInGame enemyInGame)
    {
        return abilitySelfBuff.GetMagnitude(enemyInGame);
    }
    public Status GetStatus()
    {
        return abilitySelfBuff.status;
    }
    public EnemyIntentSelfBuff(EnemyAbilitySelfBuff enemyAbilitySelfBuff)
    {
        abilitySelfBuff = enemyAbilitySelfBuff;
        intentName = enemyAbilitySelfBuff.abilityName;
        icon = enemyAbilitySelfBuff.icon;
        tooltipDatas = enemyAbilitySelfBuff.GetTooltipDataList();
        enemyAbility = enemyAbilitySelfBuff;
    }
}