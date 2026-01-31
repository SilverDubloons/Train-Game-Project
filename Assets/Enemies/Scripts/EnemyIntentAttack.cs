using UnityEngine;
using System.Collections.Generic;
public class EnemyIntentAttack : EnemyIntent
{
    public EnemyAbility ability;
    public int[] affectedColumns; // relative to enemy position, eg 0 is enemy column, -1 is one to the left, 1 is one to the right
    public int damage;
    public ActionAnimation actionAnimation;
    public override IntentType GetIntentType()
    {
        return IntentType.Attack;
    }
}

// remember to cancel intents if the player moves an enemy's row to one that the intended attack does not support
