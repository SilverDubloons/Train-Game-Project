using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    // private List<EnemyInGame> enemiesInGame = new List<EnemyInGame>();
    public List<EnemyInGame> currentEnemiesInGame = new List<EnemyInGame>();
    public static CombatManager instance;
    public bool inCombat = false;
    public int combatTurn;
    private ToolInGame targetingTool = null;
    private bool targeting = false;
    private bool aiming = false;
    private IEnumerator targetingCoroutine = null;
    private LimbInGame currentLimbMouseOver = null;
    [SerializeField] private ButtonPlus endTurnButton;
    [SerializeField] private RectTransform spareEnemyInGameParent;
    public RectTransform enemiesParent;
    public void SetupInstance()
    {
        instance = this;
        SetCanEndTurn(false);
    }
    public void SetCanEndTurn(bool canEndTurn)
    {
        endTurnButton.SetButtonEnabled(canEndTurn);
    }
    public EnemyInGame GetEnemyInGame()
    {
        if (spareEnemyInGameParent.childCount > 0)
        { 
            return spareEnemyInGameParent.GetChild(spareEnemyInGameParent.childCount - 1).GetComponent<EnemyInGame>();
        }
        return Instantiate(r.i.enemyInGamePrefab, enemiesParent).GetComponent<EnemyInGame>();
    }
    public void SetupCombat(Encounter encounter)
    {
        SetCanEndTurn(false);
        inCombat = true;
        combatTurn = 0;
        currentEnemiesInGame.Clear();
        for (int i = 0; i < encounter.enemies.Length; i++)
        {
            EnemyInGame newEnemyInGame = GetEnemyInGame();
            newEnemyInGame.SetupEnemyInGame(encounter.enemies[i]);
            currentEnemiesInGame.Add(newEnemyInGame);
        }
        HandArea.instance.StartDrawCards(true);
        DetermineEnemyIntents();
    }
    public void DetermineEnemyIntents()
    {
        // CombatArea.instance.ResetEnemiesTargeting();
        for (int i = 0; i < currentEnemiesInGame.Count; i++)
        {
            currentEnemiesInGame[i].DetermineIntents();
        }
        // CombatArea.instance.EnemyIntentsDetermined();
    }
    public void StartEnemyTurn()
    {
        StartCoroutine(ExecuteEnemyIntents());
    }
    public IEnumerator ExecuteEnemyIntents()
    {
        for (int i = 0; i < currentEnemiesInGame.Count; i++)
        {
            currentEnemiesInGame[i].StartTurn();
            while (currentEnemiesInGame[i].executingTurn)
            {
                yield return null;
            }
        }
        Logger.instance.Log("Enemy turn finished");
        combatTurn++;
        DetermineEnemyIntents();
        HandArea.instance.StartDrawCards();
        Player.instance.SetShield(0);
    }
    public void SetTargetingTool(ToolInGame newTargetingTool, bool toolIsAiming)
    {
        Logger.instance.Log($"Setting targeting tool to {newTargetingTool.baseTool.toolName}, aiming: {toolIsAiming}");
        if (targeting)
        { 
            StopCoroutine(targetingCoroutine);
        }
        targetingTool = newTargetingTool;
        aiming = toolIsAiming;
        targetingCoroutine = WaitForTarget();
        StartCoroutine(targetingCoroutine);
    }
    public IEnumerator WaitForTarget()
    {
        targeting = true;
        bool mouseClicked = false;
        Vector2 toolPos = r.i.interf.GetCanvasPositionOfRectTransform(targetingTool.rt, GameManager.instance.gameplayCanvas);
        TargetingArrows.instance.SetPosition(toolPos);
        TargetingArrows.instance.SetVisibility(true);
        while (!mouseClicked)
        {
            Vector2 mousePos = r.i.interf.GetMousePosition();
            TargetingArrows.instance.SetTarget(mousePos);
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Logger.instance.Log("Mouse clicked");
                mouseClicked = true;
                targeting = false;
                targetingTool.EndTargetPreview();
                ToolInGame toolMouseIsOver = Tools.instance.GetToolInGameMouseIsOver();
                if (toolMouseIsOver != null)
                {
                    toolMouseIsOver.PreviewSelectableTargets();
                }
                if (currentLimbMouseOver != null)
                {
                    EnemyLimbMouseClick(currentLimbMouseOver, currentLimbMouseOver.parentEnemyInGame, targetingTool);
                    HandArea.instance.HandPlayed();
                    currentLimbMouseOver = null;
                }
                aiming = false;
                targetingTool = null;
                TargetingArrows.instance.SetVisibility(false);
            }
            yield return null;
        }
    }
    public void ApplySelfTool(ToolInGame toolInGame)
    {
        switch (toolInGame.baseTool.toolTag)
        {
            case "Shield":
                Player.instance.AddShield(toolInGame.GetImpact());
            break;
        }
        HandArea.instance.HandPlayed();
    }
    public bool IsTargetingWithTool(ToolInGame tool)
    {
        return targeting && targetingTool == tool;
    }
    public bool IsTargeting()
    {
        return targeting;
    }
    public List<EnemyInGame> GetOtherEnemiesTargetableByToolGivenCurrentTarget(ToolInGame toolInGame, EnemyInGame currentTarget)
    {
        List<EnemyInGame> targetableEnemies = new List<EnemyInGame>();
        return targetableEnemies;
    }
    public void EnemyLimbMouseOver(LimbInGame limbInGame, EnemyInGame enemyInGame)
    {
        if (!targeting || targetingTool == null)
        {
            // Logger.instance.Log("EnemyLimbMouseOver called but not targeting");
            return;
        }
        bool targetingBrokenLimb = aiming && limbInGame.IsDestroyed();
        currentLimbMouseOver = limbInGame;
        if (aiming && !targetingBrokenLimb)
        {
            // Logger.instance.Log($"Highlighting limb {limbInGame.limbName} of {enemyInGame.GetEnemyName()}");
            limbInGame.SetHighlightLimb(true);
        }
        else
        {
            enemyInGame.SetHighlightOfAllLimbs(true);
        }
        List<EnemyInGame> otherTargetedEnemies = GetOtherEnemiesTargetableByToolGivenCurrentTarget(targetingTool, enemyInGame);
        // Logger.instance.Log($"Also highlighting {otherTargetedEnemies.Count} other enemies targetable by {targetingTool.baseTool.toolName}");
        for (int i = 0; i < otherTargetedEnemies.Count; i++)
        {
            otherTargetedEnemies[i].SetHighlightOfAllLimbs(true);
        }
    }
    public void EnemyLimbMouseExit(LimbInGame limbInGame, EnemyInGame enemyInGame)
    {
        if (!targeting || targetingTool == null)
        {
            return;
        }
        bool targetingBrokenLimb = aiming && limbInGame.IsDestroyed();
        currentLimbMouseOver = null;
        if (aiming && !targetingBrokenLimb)
        {
            limbInGame.SetHighlightLimb(false);
        }
        else
        {
            enemyInGame.SetHighlightOfAllLimbs(false);
        }
        List<EnemyInGame> otherTargetedEnemies = GetOtherEnemiesTargetableByToolGivenCurrentTarget(targetingTool, enemyInGame);
        for (int i = 0; i < otherTargetedEnemies.Count; i++)
        {
            otherTargetedEnemies[i].SetHighlightOfAllLimbs(false);
        }
    }
    public void EnemyLimbMouseClick(LimbInGame limbInGame, EnemyInGame enemyInGame, ToolInGame toolInGame)
    {
        Logger.instance.Log($"Targeted {limbInGame.limbName} of {enemyInGame.GetEnemyName()} with {targetingTool.baseTool.toolName}, aiming={aiming},");
        if (aiming)
        {
            limbInGame.SetHighlightLimb(false);
        }
        else
        {
            enemyInGame.SetHighlightOfAllLimbs(false);
        }
        ApplyToolEffectToEnemy(enemyInGame, toolInGame, aiming, limbInGame);
        List<EnemyInGame> otherAffectedEnemies = GetOtherEnemiesTargetableByToolGivenCurrentTarget(toolInGame, enemyInGame);
        for(int i = 0; i < otherAffectedEnemies.Count; i++)
        {
            otherAffectedEnemies[i].SetHighlightOfAllLimbs(false);
            ApplyToolEffectToEnemy(otherAffectedEnemies[i], toolInGame);
        }
    }
    public void ApplyToolEffectToEnemy(EnemyInGame enemyInGame, ToolInGame toolInGame, bool aiming = false, LimbInGame limbInGame = null)
    {
        enemyInGame.ApplyToolEffect(toolInGame, aiming, limbInGame);
        Logger.instance.Log($"Applying effect of {toolInGame.baseTool.toolName} to {enemyInGame.GetEnemyName()}, aiming: {aiming}, limbInGame: {limbInGame}");
    }
    public void EnemyDefeated(EnemyInGame defeatedEnemy)
    { 
        currentEnemiesInGame.Remove(defeatedEnemy);
        defeatedEnemy.SetParent(spareEnemyInGameParent);
        // enemiesInGame.Add(defeatedEnemy);
        defeatedEnemy.SetVisibility(false);
    }
    public void ClickEndTurn()
    {
        SetCanEndTurn(false);
        HandArea.instance.TurnEnded();
        StartEnemyTurn();
    }
    public void CleanupCombat()
    {
        for (int i = currentEnemiesInGame.Count - 1; i >= 0; i--)
        {
            currentEnemiesInGame[i].RemoveAlIntents();
            currentEnemiesInGame[i].statusEffects.ResetStatusEffects();
            currentEnemiesInGame[i].SetShield(0);
            EnemyDefeated(currentEnemiesInGame[i]);
        }
        currentEnemiesInGame.Clear();
        targetingTool = null;
        targeting = false;
        aiming = false;
        SetCanEndTurn(false);
        TargetingArrows.instance.SetVisibility(false);
        currentLimbMouseOver = null;
        inCombat = false;
        // CombatArea.instance.EndTargetPreview();
        // CombatArea.instance.SetMovableSpaces(null);
        // CombatArea.instance.ResetEnemiesTargeting();
    }
}
