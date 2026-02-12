using UnityEngine;
using System.Collections.Generic;

public abstract class EnemyBehavior : ScriptableObject
{
    public abstract List<EnemyIntent> GetIntents(EnemyInGame enemyInGame);
}
