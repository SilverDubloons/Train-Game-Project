using UnityEngine;
using System.Collections.Generic;

public abstract class SpawningPattern : ScriptableObject
{
    public abstract CombatSpace GetSpawnSpace(CombatSpace[,] availableSpaces);
}