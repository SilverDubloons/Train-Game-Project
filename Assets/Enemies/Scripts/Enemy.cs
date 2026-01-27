using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
    public string enemyTag;
    public string enemyName;
    public int maxHealth; 
    [SerializeField] public Limb[] limbs;
    public Vector2 spriteCenter;
    public Vector2 totalSize;
    [SerializeField] public SpawningPattern spawningPattern;
    public Color crosshairColor = Color.white;
    public CombatSpace GetSpawnSpace(CombatSpace[,] availableSpaces)
    {
        return spawningPattern.GetSpawnSpace(availableSpaces);
    }
    [SerializeField] public EnemyAbility[] abilities;
    [SerializeField] public EnemyBehavior behavior;
    public bool canMoveDiagonally;
}
