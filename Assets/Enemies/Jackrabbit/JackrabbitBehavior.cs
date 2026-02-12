using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "JackrabbitBehavior", menuName = "Combat/Enemy Behaviors/Jackrabbit Behavior")]
public class JackrabbitBehavior : EnemyBehavior
{
    public override List<EnemyIntent> GetIntents(EnemyInGame enemyInGame)
    {
        List<EnemyIntent> intents = new List<EnemyIntent>();
        Enemy baseEnemy = enemyInGame.GetBaseEnemy();
        if (baseEnemy.abilities[2].IsAvailable(enemyInGame))
        {
            EnemyIntentSelfBuff intentSelfBuff = new EnemyIntentSelfBuff((EnemyAbilitySelfBuff)baseEnemy.abilities[2]);
            intents.Add(intentSelfBuff);
            if (baseEnemy.abilities[1].IsAvailable(enemyInGame))
            {
                EnemyIntentShield intentShield = new EnemyIntentShield((EnemyAbilityShield)baseEnemy.abilities[1]);
                intents.Add(intentShield);
            }
        }
        else
        {
            if (baseEnemy.abilities[0].IsAvailable(enemyInGame))
            {
                EnemyIntentAttack intent = new EnemyIntentAttack((EnemyAbilityAttack)baseEnemy.abilities[0]);
                intents.Add(intent);
            }
            else if (baseEnemy.abilities[3].IsAvailable(enemyInGame))
            {
                LimbInGame limbToHeal = enemyInGame.GetBestLimbToHeal();
                if (limbToHeal != null)
                { 
                    EnemyIntentSelfHealLimb intent = new EnemyIntentSelfHealLimb((EnemyAbilitySelfHealLimb)baseEnemy.abilities[3]);
                    intents.Add(intent);
                }
            }
        }
        if (intents.Count == 0 && baseEnemy.abilities[1].IsAvailable(enemyInGame))
        {
            EnemyIntentShield intent = new EnemyIntentShield((EnemyAbilityShield)baseEnemy.abilities[1]);
            intents.Add(intent);
        }
        return intents;
    }
}
