
using UnityEngine;
using System.Collections.Generic;
public class EnemyIntentAttack : EnemyIntent
{
    public EnemyAbility ability;
    public List<CombatSpace> affectedSpaces;
    public override IntentType GetIntentType()
    {
        return IntentType.Attack;
    }
}
