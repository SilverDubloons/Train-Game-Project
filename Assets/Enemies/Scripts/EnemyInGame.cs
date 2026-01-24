using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemyInGame : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private RectTransform limbsParent;
    private string enemyTag;
    private string enemyName;
    private int maxHealth;
    private int currentHealth;
    private List<LimbInGame> limbInGames = new List<LimbInGame>();
    private List<LimbInGame> currentLimbInGames = new List<LimbInGame>();
    private Vector2 spriteCenter;
    private Vector2 spriteTotalSize;
    private CombatSpace currentCombatSpace;
    
    public void SetupEnemyInGame(Enemy enemy)
    {
        name = enemy.enemyName;
        enemyTag = enemy.enemyTag;
        enemyName = enemy.enemyName;
        maxHealth = enemy.maxHealth;
        currentHealth = maxHealth;
        currentLimbInGames.Clear();
        for (int i = 0; i < enemy.limbs.Length; i++)
        {
            LimbInGame newLimbInGame;
            if (limbInGames.Count > i)
            {
                newLimbInGame = limbInGames[i];
            }
            else
            {
                newLimbInGame = Instantiate(r.i.limbInGamePrefab, limbsParent).GetComponent<LimbInGame>();
                limbInGames.Add(newLimbInGame);
            }
            currentLimbInGames.Add(newLimbInGame);
            newLimbInGame.SetupFromLimb(enemy.limbs[i], enemy, this);
        }
        for(int i = enemy.limbs.Length; i < limbInGames.Count; i++)
        {
            limbInGames[i].SetVisibility(false);
        }
        spriteCenter = enemy.spriteCenter;
        spriteTotalSize = enemy.totalSize;
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
    public void SetParent(RectTransform newParent, CombatSpace combatSpace)
    { 
        rt.SetParent(newParent);
        float widthScale = r.i.interf.maxEnemySize.x / spriteTotalSize.x;
        float heightScale = r.i.interf.maxEnemySize.y / spriteTotalSize.y;
        float scale = Mathf.Min(widthScale, heightScale, 1f);
        rt.localScale = new Vector3(scale, scale, 1f);
        rt.anchoredPosition = new Vector2(-spriteCenter.x * scale, -spriteCenter.y * scale);
        currentCombatSpace = combatSpace;
    }
    public void SetVisibilityOfLimbCrosshairs(bool visible)
    {
        for (int i = 0; i < limbInGames.Count; i++)
        {
            limbInGames[i].SetVisibilityOfCrosshair(visible);
        }
    }
    public void SetHighlightOfAllLimbs(bool highlight)
    {
        for (int i = 0; i < currentLimbInGames.Count; i++)
        {
            currentLimbInGames[i].SetHighlightLimb(highlight);
        }
    }
    public CombatSpace GetCurrentCombatSpace()
    {
        return currentCombatSpace;
    }
    public string GetEnemyName()
    {
        return enemyName;
    }
    public void ApplyToolEffect(ToolInGame toolInGame, bool aiming = false, LimbInGame targetLimb = null)
    {
        int damage = toolInGame.GetDamage(currentCombatSpace, this, aiming, targetLimb);
        if(damage > 0)
        {
            if (aiming && targetLimb != null)
            {
                TakeLimbDamage(targetLimb, damage);
            }
            TakeDamage(damage);
        }
    }
    public void TakeLimbDamage(LimbInGame limb, int damage)
    {
        limb.TakeDamage(damage);
    }
    public void TakeDamage(int damage)
    { 
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            CombatManager.instance.EnemyDefeated(this);
        }
    }
}
