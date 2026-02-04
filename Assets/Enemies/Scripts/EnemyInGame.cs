using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

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
    private string enemyTag;
    private string enemyName;
    private int maxHealth;
    private int currentHealth;
    private List<LimbInGame> limbInGames = new List<LimbInGame>();
    private List<LimbInGame> currentLimbInGames = new List<LimbInGame>();
    private Vector2 spriteCenter;
    private Vector2 spriteTotalSize;
    private CombatSpace currentCombatSpace;
    private Enemy baseEnemy;
    private List<EnemyIntent> intents = new List<EnemyIntent>();
    public bool executingTurn = false;
    bool executingAction = false;
    public void SetupEnemyInGame(Enemy enemy)
    {
        SetVisibility(true);
        baseEnemy = enemy;
        name = enemy.enemyName;
        enemyTag = enemy.enemyTag;
        enemyName = enemy.enemyName;
        maxHealth = enemy.maxHealth;
        currentHealth = maxHealth;
        executingTurn = false;
        executingAction = false;
        intents.Clear();
        UpdateHealthbar();
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
        for (int i = enemy.limbs.Length; i < limbInGames.Count; i++)
        {
            limbInGames[i].SetVisibility(false);
        }
        spriteCenter = enemy.spriteCenter;
        spriteTotalSize = enemy.totalSize;
        rt.SetParent(CombatArea.instance.looseCharactersParent);
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
    public void SetCurrentCombatSpace(CombatSpace combatSpace, bool firstTimeSetup, RectTransform newParent = null)
    {
        if (newParent != null)
        {
            // Logger.instance.Log($"Setting parent of {GetBasicInfo()} to {newParent}");
            rt.SetParent(newParent);
            rt.anchoredPosition = Vector2.zero;
        }
        else
        {
            rt.anchoredPosition = r.i.interf.GetCanvasPositionOfRectTransform(combatSpace.GetRectTransform(), GameManager.instance.gameplayCanvas);
        }
        if (firstTimeSetup)
        {
            float widthScale = r.i.interf.maxEnemySize.x / spriteTotalSize.x;
            float heightScale = r.i.interf.maxEnemySize.y / spriteTotalSize.y;
            float scale = Mathf.Min(widthScale, heightScale, 1f);
            // rt.localScale = new Vector3(scale, scale, 1f);
            rt.localScale = new Vector3(1f, 1f, 1f);
            limbsParent.localScale = new Vector3(scale, scale, 1f);
            // rt.anchoredPosition = new Vector2(-spriteCenter.x * scale, -spriteCenter.y * scale);
            limbsParent.anchoredPosition = new Vector2(-spriteCenter.x * scale, -spriteCenter.y * scale);
        }
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
        if (damage > 0)
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
            RemoveAlIntents();
            CombatManager.instance.EnemyDefeated(this);
        }
        UpdateHealthbar();
    }
    public void RemoveAlIntents()
    {
        for (int i = 0; i < intents.Count; i++)
        {
            switch (intents[i].GetIntentType())
            {
                case IntentType.Attack:
                    EnemyIntentAttack enemyIntentAttack = (EnemyIntentAttack)intents[i];
                    for (int j = 0; j < enemyIntentAttack.affectedColumns.Length; j++)
                    {
                        CombatArea.instance.RemoveEnemyIntent(currentCombatSpace.gridPosition.x, enemyIntentAttack.affectedColumns[j], enemyIntentAttack);
                    }
                break;
            }
        }
    }
    public void UpdateHealthbar()
    {
        float healthbarSize = healthbarRt.rect.width - 4;
        float percentageHealth = (float)currentHealth / (float)maxHealth;
        healthbarMask.padding = new Vector4(0, 0, healthbarSize * (1f - percentageHealth), 0);
        healthbarLabel.ChangeText($"{currentHealth}/{maxHealth}");
        statsParentRt.sizeDelta = new Vector2(statsParentRt.sizeDelta.x, 26f);
    }
    public void DetermineIntents()
    {
        intents = baseEnemy.behavior.GetIntents(this, CombatArea.instance.GetCurrentCombatSpaces(), CombatArea.instance.GetPlayerSpace());
        for (int i = 0; i < intents.Count; i++)
        {
            EnemyIntentUI enemyIntentUI = CombatManager.instance.GetEnemyIntentUI();
            intents[i].enemyIntentUI = enemyIntentUI;
            if (enemyIntentUI == null)
            {
                Logger.instance.Log("EnemyIntentUI is null");
            }
            if (intents[i] == null)
            {
                Logger.instance.Log($"intents[{i}] is null");
            }
            enemyIntentUI.SetupIntentUI(intents[i], i, intentsUIParentRt);
            switch (intents[i].GetIntentType())
            {
                case IntentType.Attack:
                    EnemyIntentAttack enemyIntentAttack = (EnemyIntentAttack)intents[i];
                    // Logger.instance.Log($"{GetBasicInfo()} attack intent: {intents[i].intentName}");
                    for (int j = 0; j < enemyIntentAttack.affectedColumns.Length; j++)
                    {
                        CombatArea.instance.HighlightEnemyAttack(currentCombatSpace.gridPosition.x, enemyIntentAttack.affectedColumns[j], enemyIntentAttack);
                    }
                    break;
                case IntentType.Move:
                    EnemyIntentMove enemyIntentMove = (EnemyIntentMove)intents[i];
                    // Logger.instance.Log($"{GetBasicInfo()} move intent: {enemyIntentMove.directionToMove}");
                    break;
            }
        }
    }
    public string GetBasicInfo()
    {
        if (currentCombatSpace == null)
        {
            return $"{enemyName} at (null)";
        }
        return $"{enemyName} at {currentCombatSpace.gridPosition}";
    }
    public Enemy GetBaseEnemy()
    {
        return baseEnemy;
    }
    public bool EnemyMeetsLimbRequirements(LimbRequirement[] limbRequirements)
    {
        foreach (LimbRequirement requirement in limbRequirements)
        {
            int numberOfLimbsMet = 0;
            bool requirementMet = false;
            foreach (LimbInGame limbInGame in currentLimbInGames)
            {
                if (limbInGame.IsOfType(requirement.limbTag))
                {
                    numberOfLimbsMet++;
                    if (numberOfLimbsMet >= requirement.numberRequired)
                    {
                        requirementMet = true;
                        break;
                    }
                }
            }
            if (!requirementMet)
            {
                return false;
            }
        }
        return true;
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
        for (int i = 0; i < intents.Count; i++)
        {
            switch (intents[i].GetIntentType())
            {
                case IntentType.Attack:
                    EnemyIntentAttack enemyIntentAttack = (EnemyIntentAttack)intents[i];
                    StartAttack(enemyIntentAttack);
                break;
                case IntentType.Move:
                    EnemyIntentMove enemyIntentMove = (EnemyIntentMove)intents[i];
                    CombatSpace spaceToMoveTo = CombatArea.instance.GetRelativeSpace(currentCombatSpace, enemyIntentMove.directionToMove);
                    if (spaceToMoveTo != null && !spaceToMoveTo.EnemyInSpace())
                    {
                        StartMove(spaceToMoveTo);
                    }
                    else
                    {
                        Logger.instance.Log($"{GetBasicInfo()} can't move {enemyIntentMove.directionToMove}");
                    }
                break;
            }
            while (executingAction)
            {
                yield return null;
            }
            intents[i].enemyIntentUI.RemoveIntent();
            for (int j = i + 1; j < intents.Count; j++)
            {
                intents[j].enemyIntentUI.MoveLeft();
            }
        }
        executingTurn = false;
    }
    private void StartMove(CombatSpace destination)
    {
        currentCombatSpace.RemoveEnemyFromSpace();
        StartCoroutine(MoveToCombatSpace(destination));
    }
    private IEnumerator MoveToCombatSpace(CombatSpace destinationSpace)
    {
        executingAction = true;
        // rt.SetParent(CombatArea.instance.movingCharactersParent);
        Vector2 origin = rt.anchoredPosition;
        Vector2 destination = r.i.interf.GetCanvasPositionOfRectTransform(destinationSpace.GetRectTransform(), GameManager.instance.gameplayCanvas);
        // destination += new Vector2(-spriteCenter.x * rt.localScale.x, -spriteCenter.y * rt.localScale.y);
        float t = 0;
        float moveTime = 1f;
        while (t < moveTime)
        {
            t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0f, moveTime);
            float normalizedTime = t / moveTime;
            rt.anchoredPosition = Vector2.Lerp(origin, destination, r.i.interf.animationCurve.Evaluate(normalizedTime));
            yield return null;
        }
        destinationSpace.PlaceEnemyInSpace(this, false);
        executingAction = false;
    }
    public void SetParent(RectTransform newParent)
    {
        // Logger.instance.Log($"Setting parent of {GetBasicInfo()} to {newParent}");
        rt.SetParent(newParent);
    }
    public void StartAttack(EnemyIntentAttack enemyIntentAttack)
    {
        StartCoroutine(Attack(enemyIntentAttack));
    }
    private IEnumerator Attack(EnemyIntentAttack enemyIntentAttack)
    {
        executingAction = true;
/*        float t = 0;
        float attackTime = 1f;
        while (t < attackTime)
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0f, attackTime);
            yield return null;
        }*/
        List<ActionAnimator> actionAnimators = new List<ActionAnimator>();
        for (int i = 0; i < enemyIntentAttack.affectedColumns.Length; i++)
        {
            Vector2Int targetArea = new Vector2Int(currentCombatSpace.gridPosition.x + enemyIntentAttack.affectedColumns[0], 0);
            if (!CombatArea.instance.IsPositionInCombatArea(targetArea))
            {
                continue;
            }
            CombatSpace targetSpace = CombatArea.instance.GetCombatSpaceAtPosition(targetArea);
            if (enemyIntentAttack.actionAnimation != null)
            {
                ActionAnimator actionAnimator = ActionAnimators.instance.StartActionAnimation(enemyIntentAttack.actionAnimation, r.i.interf.GetCanvasPositionOfRectTransform(targetSpace.rt, GameManager.instance.gameplayCanvas));
                actionAnimators.Add(actionAnimator);
            }
            if (targetSpace == CombatArea.instance.GetPlayerSpace())
            {
                Player.instance.TakeDamage(enemyIntentAttack.damage);
            }
        }
        while (ActionAnimatorInListStillRunning(actionAnimators))
        { 
            yield return null;
        }
        executingAction = false;
    }
    private bool ActionAnimatorInListStillRunning(List<ActionAnimator> actionAnimators)
    {
        for (int i = 0; i < actionAnimators.Count; i++)
        {
            if (actionAnimators[i].animating)
            {
                return true;
            }
        }
        return false;
    }
}
