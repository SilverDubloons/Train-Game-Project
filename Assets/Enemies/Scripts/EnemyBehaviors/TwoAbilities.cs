using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TwoAbilities", menuName = "Combat/Enemy Behaviors/Two Abilities")]
public class TwoAbilities : EnemyBehavior
{
    public override List<EnemyIntent> GetIntents(EnemyInGame enemyInGame)
    {
        List<EnemyIntent> intents = new List<EnemyIntent>();
        Enemy baseEnemy = enemyInGame.GetBaseEnemy();
        // EnemyAbility[] bestAbilities = new EnemyAbility[2];
        List<EnemyAbility> bestAbilities = new List<EnemyAbility>();
        for (int i = 0; i < baseEnemy.abilities.Length; i++)
        {
            EnemyAbility ability = baseEnemy.abilities[i];
            if (ability == null)
            {
                continue;
            }
            if (!ability.IsAvailable(enemyInGame))
            {
                continue;
            }
            bestAbilities.Add(ability);
        }
        for (int i = 0; i < bestAbilities.Count; i++)
        {
            if (bestAbilities[i] != null)
            {
                switch (bestAbilities[i].GetAbilityType())
                {
                    case AbilityType.Attack:
                        EnemyAbilityAttack enemyAbilityAttack = (EnemyAbilityAttack)bestAbilities[i];
                        EnemyIntentAttack enemyIntentAttack = new EnemyIntentAttack(enemyAbilityAttack);
                        intents.Add(enemyIntentAttack);
                    break;
                }
            }
        }
        return intents;
    }
}
