using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
public abstract class EnemyAbility : ScriptableObject
{
    public string abilityName;
    public string abilityTag;
    public Sprite icon;
    public int cooldown; // 0 = no cooldown, 1 = one turn, etc
    public bool startsOnCooldown;
    public LimbRequirement[] limbRequirements;
    public abstract List<TooltipData> GetTooltipDataList();
    public abstract AbilityType GetAbilityType();
    public ActionAnimation actionAnimation;
    public bool GetLimbRequirementsMet(EnemyInGame enemyInGame)
    {
        if (enemyInGame.EnemyMeetsLimbRequirements(limbRequirements))
        {
            return true;
        }
        return false;
    }
    public bool IsAvailable(EnemyInGame enemyInGame)
    {
        // Logger.instance.Log($"Checking if {abilityName} is available for {enemyInGame.name}");
        if (!enemyInGame.EnemyMeetsLimbRequirements(limbRequirements))
        {
            // Logger.instance.Log("Limb requirements not met");
            return false;
        }
        if (!enemyInGame.IsAbilityOffCooldown(this))
        {
            // Logger.instance.Log("Ability is not off cooldown");
            return false;
        }
        // Logger.instance.Log("Ability available!");
        return true;
    }
}
[System.Serializable]
public struct LimbRequirement
{
    public LimbTag limbTag;
    public int numberRequired; // e.g., 2 arms
}
public enum AbilityType
{
    Attack,
    SelfBuff,
    Shield,
    SelfHealLimb,
    SelfHeal,
    HealOther,
    HealOtherLimb
}