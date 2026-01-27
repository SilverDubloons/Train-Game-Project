using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public abstract class EnemyAbility : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public int cooldown;
    public LimbRequirement[] limbRequirements;
    //public abstract bool IsAvailable(EnemyInGame enemy);
    public abstract List<TooltipData> GetTooltipDataList();
    public abstract AbilityType GetAbilityType();
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
    Self
}