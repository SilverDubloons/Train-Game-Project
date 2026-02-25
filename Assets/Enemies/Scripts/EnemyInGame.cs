using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class EnemyInGame : MonoBehaviour
{
    public RectTransform rt;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private RectTransform limbsParent;
    // [SerializeField] private RectTransform healthbarMask;
    [SerializeField] private RectMask2D healthbarMask;
    [SerializeField] private RectTransform healthbarRt;
    [SerializeField] private RectTransform statsParentRt;
    [SerializeField] private RectTransform intentsUIParentRt;
    [SerializeField] private RectTransform statusEffectsUIParentRt;
    [SerializeField] private GameObject statsVisibiltyObject;
    [SerializeField] private Label healthbarLabel;
    [SerializeField] private GameObject shieldVisibilityObject;
    [SerializeField] private Label shieldLabel;
    public StatusEffectsOnCharacter statusEffects;
    private string enemyTag;
    private string enemyName;
    private int maxHealth;
    private int currentHealth;
    private int currentShield;
    private List<LimbInGame> limbInGames = new List<LimbInGame>();
    private List<LimbInGame> currentLimbInGames = new List<LimbInGame>();
    private Vector2 spriteCenter;
    private Vector2 spriteTotalSize;
    private Enemy baseEnemy;
    private List<EnemyIntent> intents = new List<EnemyIntent>();
    public bool executingTurn = false;
    public Dictionary<EnemyAbility, int> turnAbilityWasLastUsed = new Dictionary<EnemyAbility, int>();
    
    public void SetupEnemyInGame(EncounterEnemy encounterEnemy)
    {
        SetVisibility(true);
        baseEnemy = encounterEnemy.enemy;
        name = baseEnemy.enemyName;
        enemyTag = baseEnemy.enemyTag;
        enemyName = baseEnemy.enemyName;
        maxHealth = baseEnemy.maxHealth;
        currentHealth = maxHealth;
        currentShield = 0;
        executingTurn = false;
        // intents.Clear();
        UpdateHealthbar();
        UpdateShield();
        currentLimbInGames.Clear();
        for (int i = 0; i < baseEnemy.limbs.Length; i++)
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
            newLimbInGame.SetupFromLimb(baseEnemy.limbs[i], baseEnemy, this);
        }
        for (int i = baseEnemy.limbs.Length; i < limbInGames.Count; i++)
        {
            limbInGames[i].SetVisibility(false);
        }
        spriteCenter = baseEnemy.spriteCenter;
        spriteTotalSize = baseEnemy.totalSize;
        rt.SetParent(CombatManager.instance.enemiesParent);
        rt.localPosition = encounterEnemy.spawnPosition;
        turnAbilityWasLastUsed.Clear();
        for (int i = 0; i < baseEnemy.abilities.Length; i++)
        {
            if (baseEnemy.abilities[i].startsOnCooldown)
            {
                turnAbilityWasLastUsed[baseEnemy.abilities[i]] = -1;
            }
        }
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
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
    public string GetEnemyName()
    {
        return enemyName;
    }
    public void ApplyToolEffect(ToolInGame toolInGame, bool aiming = false, LimbInGame targetLimb = null)
    {
        int damage = toolInGame.GetDamage(this, aiming, targetLimb);
        if (damage > 0)
        {
            if (aiming && targetLimb != null)
            {
                TakeLimbDamage(targetLimb, damage);
            }
            TakeDamage(damage);
        }
    }
    public void TakeLimbDamage(LimbInGame limbInGame, int damage)
    {
        limbInGame.TakeDamage(damage);
    }
    public void LimbDestroyed(LimbInGame limbInGame)
    {
        var intentsToRemove = intents.Where(intent =>
            !intent.enemyAbility.GetLimbRequirementsMet(this)).ToList();
        if (intentsToRemove.Count > 0)
        {
            foreach (var intent in intentsToRemove)
            {
                intent.enemyIntentUI.RetireIntent();
                intents.Remove(intent);
            }
            UpdateIntentsUI();
        }
    }
    public void TakeDamage(int damage)
    {
        if (currentShield > 0)
        {
            if (currentShield >= damage)
            {
                currentShield -= damage;
                damage = 0;
            }
            else
            {
                damage -= currentShield;
                currentShield = 0;
            }
            UpdateShield();
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RemoveAlIntents();
            statusEffects.ResetStatusEffects();
            CombatManager.instance.EnemyDefeated(this);
        }
        UpdateHealthbar();
    }
    public void RemoveAlIntents()
    {
        for (int i = 0; i < intents.Count; i++)
        {
            intents[i].enemyIntentUI.RetireIntent();
        }
        intents.Clear();
    }
    public void UpdateShield()
    {
        if (currentShield > 0)
        {
            shieldVisibilityObject.SetActive(true);
            shieldLabel.ChangeText(currentShield.ToString());
        }
        else
        {
            shieldVisibilityObject.SetActive(false);
        }
    }
    public void AddShield(int shieldToAdd)
    {
        currentShield += shieldToAdd;
        UpdateShield();
    }
    public void SetShield(int newShieldValue)
    { 
        currentShield = newShieldValue;
        UpdateShield();
    }
    public void UpdateIntentsUI()
    {
        float intentUIx = 0;
        for (int i = 0; i < intents.Count; i++)
        {
            EnemyIntentUI enemyIntentUI = intents[i].enemyIntentUI;
            enemyIntentUI.SetPosition(intentUIx);
            if (i < intents.Count - 1)
            {
                float intentWidth = enemyIntentUI.GetIntentWidth();
                intentUIx += intentWidth;
            }
        }
    }
    public void UpdateStatusEffectsUI()
    {
        bool atLeastOneStatusEffect = statusEffects.CharacterHasAtLeastOneStatusEffect();
        statsParentRt.sizeDelta = new Vector2(statsParentRt.sizeDelta.x, atLeastOneStatusEffect ? 36f : 26f);
    }
    public void UpdateHealthbar()
    {
        float healthbarSize = healthbarRt.rect.width - 4;
        float percentageHealth = (float)currentHealth / (float)maxHealth;
        healthbarMask.padding = new Vector4(0, 0, healthbarSize * (1f - percentageHealth), 0);
        healthbarLabel.ChangeText($"{currentHealth}/{maxHealth}");
        UpdateStatusEffectsUI();
    }
    public void DetermineIntents()
    {
        intents = baseEnemy.behavior.GetIntents(this);
        for (int i = 0; i < intents.Count; i++)
        {
            EnemyIntentUI enemyIntentUI = EnemyIntents.instance.GetEnemyIntentUI();
            enemyIntentUI.SetupIntentUI(intents[i], intentsUIParentRt, this);
            intents[i].enemyIntentUI = enemyIntentUI;
        }
        UpdateIntentsUI();
    }
    public string GetBasicInfo()
    {
        return $"{enemyName}";
    }
    public Enemy GetBaseEnemy()
    {
        return baseEnemy;
    }
    public bool EnemyMeetsLimbRequirements(LimbRequirement[] limbRequirements)
    {
        foreach (LimbRequirement requirement in limbRequirements)
        {
            if (!EnemyMeetsLimbRequirement(requirement))
            {
                return false;
            }
        }
        return true;
    }
    public bool EnemyMeetsLimbRequirement(LimbRequirement limbRequirement)
    {
        int numberOfLimbsMet = 0;
        foreach (LimbInGame limbInGame in currentLimbInGames)
        {
            if (limbInGame.IsOfType(limbRequirement.limbTag) && !limbInGame.IsDestroyed())
            {
                numberOfLimbsMet++;
                if (numberOfLimbsMet >= limbRequirement.numberRequired)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void StartTurn()
    {
        if (intents.Count == 0)
        {
            return;
        }
        StartCoroutine(ExecuteTurn());
    }
    private IEnumerator ExecuteTurn()
    {
        executingTurn = true;
        currentShield = 0; // this should be an animation
        UpdateShield();
        while (intents.Count > 0)
        {
            _ = intents[0].ExecuteIntentAsync(this);
            turnAbilityWasLastUsed[intents[0].enemyAbility] = CombatManager.instance.combatTurn;
            while (intents[0].executingIntent)
            {
                yield return null;
            }
            intents[0].enemyIntentUI.RetireIntent();
            intents.RemoveAt(0);
            UpdateIntentsUI();
        }
        executingTurn = false;
    }
    public void SetParent(RectTransform newParent)
    {
        rt.SetParent(newParent);
    }
    public bool IsAbilityOffCooldown(EnemyAbility enemyAbility)
    {
        if (enemyAbility.cooldown == 0)
        {
            return true;
        }
        if (!turnAbilityWasLastUsed.ContainsKey(enemyAbility))
        {
            return true;
        }
        if (CombatManager.instance.combatTurn - turnAbilityWasLastUsed[enemyAbility] > enemyAbility.cooldown)
        {
            return true;
        }
        return false;
    }
    public List<LimbInGame> GetListOfDamagedLimbs() // returns null if all limbs fine
    {
        List<LimbInGame> damagedLimbs = new List<LimbInGame>();
        for (int i = 0; i < limbInGames.Count; i++)
        {
            if (limbInGames[i].GetMissingHealth() > 0)
            {
                damagedLimbs.Add(limbInGames[i]);
            }
        }
        return damagedLimbs;
    }
    public LimbInGame GetBestLimbToHeal()
    {
        List<LimbInGame> damagedLimbs = GetListOfDamagedLimbs();
        if (damagedLimbs == null || damagedLimbs.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < baseEnemy.abilities.Length; i++)
        {
            for (int j = 0; j < baseEnemy.abilities[i].limbRequirements.Length; j++)
            {
                if (!EnemyMeetsLimbRequirement(baseEnemy.abilities[i].limbRequirements[j]))
                {
                    for (int k = 0; k < damagedLimbs.Count; j++)
                    {
                        if (damagedLimbs[k].LimbCouldFulfilLimbRequirement(baseEnemy.abilities[i].limbRequirements[j]))
                        { 
                            return damagedLimbs[k];
                        }
                    }
                }
            }
        }
        damagedLimbs.Sort((x, y) =>
        {
            return x.GetMissingHealth() - y.GetMissingHealth();
        });
        return damagedLimbs[0];
    }
}
