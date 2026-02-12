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
    public Color crosshairColor = Color.white;
    [SerializeField] public EnemyAbility[] abilities;
    [SerializeField] public EnemyBehavior behavior;
}
