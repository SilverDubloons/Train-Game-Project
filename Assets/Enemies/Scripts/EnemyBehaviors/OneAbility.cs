using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "OneAbility", menuName = "Combat/Enemy Behaviors/One Ability")]
public class OneAbility : EnemyBehavior
{
    // public abstract List<EnemyAction> GetActions(EnemyInGame enemyInGame, CombatSpace[,] combatSpaces, CombatSpace playerSpace);
    public override List<EnemyIntent> GetIntents(EnemyInGame enemyInGame)
    {
        List<EnemyIntent> intents = new List<EnemyIntent>();
        Enemy baseEnemy = enemyInGame.GetBaseEnemy();
        EnemyAbility bestAbility = null;
        // AttackAvailabilityData bestAttackAvailabilityData = new AttackAvailabilityData();
        for (int i = 0; i < baseEnemy.abilities.Length; i++)
        {
            EnemyAbility ability = baseEnemy.abilities[i];
            if (ability == null)
            {
                Logger.instance.Warning($"{enemyInGame.name} has a null ability");
                continue;
            }
            if (!ability.IsAvailable(enemyInGame))
            {
                continue;
            }
            bestAbility = ability;
            break;
        }
        
        if(bestAbility != null)
        {
            switch(bestAbility.GetAbilityType())
            {
                case AbilityType.Attack:
                    EnemyIntentAttack intent = new EnemyIntentAttack((EnemyAbilityAttack)bestAbility);
                    intents.Add(intent);
                    break;
            }
        }
        return intents;
    }
}
