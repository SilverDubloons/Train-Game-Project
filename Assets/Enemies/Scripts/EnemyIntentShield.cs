using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
public class EnemyIntentShield : EnemyIntent
{
    public EnemyAbilityShield abilityShield;
    public override async Task ExecuteIntentAsync(EnemyInGame enemyInGame)
    {
        executingIntent = true;
        enemyInGame.AddShield(GetMagnitude(enemyInGame));
        executingIntent = false;
    }
    public override IntentType GetIntentType()
    {
        return IntentType.Shield;
    }
    public int GetMagnitude(EnemyInGame enemyInGame)
    {
        return abilityShield.GetMagnitude(enemyInGame);
    }
    public EnemyIntentShield(EnemyAbilityShield enemyAbilityShield)
    {
        abilityShield = enemyAbilityShield;
        intentName = enemyAbilityShield.abilityName;
        icon = enemyAbilityShield.icon;
        tooltipDatas = enemyAbilityShield.GetTooltipDataList();
        enemyAbility = enemyAbilityShield;
    }
}