using UnityEngine;

public class EnemyIntentMove : EnemyIntent
{
    public DirectionToMove directionToMove;
    public override IntentType GetIntentType()
    {
        return IntentType.Move;
    }
}
