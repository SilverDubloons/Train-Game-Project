using System.Collections.Generic;
using UnityEngine;
// [CreateAssetMenu(fileName = "EnemyAbilityAttack", menuName = "Combat/EnemyAbilities/Enemy Ability Attack")]
[CreateAssetMenu(fileName = "Enemy", menuName = "Combat/Enemy")]
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
    public bool prefersColumnForMovement; // when true, enemy will try to get into appropriate column first, when false, enemy will prefer to get into appropriate row first (more dangerous)
}
