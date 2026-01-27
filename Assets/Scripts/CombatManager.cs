using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    private List<EnemyInGame> enemiesInGame = new List<EnemyInGame>();
    public List<EnemyInGame> currentEnemiesInGame = new List<EnemyInGame>();
    public static CombatManager instance;
    public bool inCombat = false;
    private ToolInGame targetingTool = null;
    private bool targeting = false;
    private bool aiming = false;
    private IEnumerator targetingCoroutine = null;
    private LimbInGame currentLimbMouseOver = null;
    [SerializeField] private ButtonPlus endTurnButton;
    public void SetupInstance()
    {
        instance = this;
    }
    public void SetCanEndTurn(bool canEndTurn)
    {
        endTurnButton.SetButtonEnabled(canEndTurn);
    }
    public void SetupCombat(Encounter encounter)
    {
        SetCanEndTurn(false);
        inCombat = true;
        currentEnemiesInGame.Clear();
        CombatArea.instance.SetupBoard(encounter.boardSize);
        for (int i = 0; i < encounter.enemies.Length; i++)
        {
            EnemyInGame newEnemyInGame;
            if (enemiesInGame.Count > i)
            {
                newEnemyInGame = enemiesInGame[i];
            }
            else
            {
                newEnemyInGame = Instantiate(r.i.enemyInGamePrefab).GetComponent<EnemyInGame>();
                enemiesInGame.Add(newEnemyInGame);
            }
            newEnemyInGame.SetupEnemyInGame(encounter.enemies[i]);
            CombatManager.instance.currentEnemiesInGame.Add(newEnemyInGame);
            CombatArea.instance.GetSpawnSpaceForEnemy(encounter.enemies[i]).PlaceEnemyInSpace(newEnemyInGame);
        }
        for (int i = encounter.enemies.Length; i < enemiesInGame.Count; i++)
        {
            enemiesInGame[i].gameObject.SetActive(false);
        }
        HandArea.instance.StartDrawCards(true);
        CombatArea.instance.SetPlayerPosition(new Vector2Int(RNG.instance.combat.Range(0, encounter.boardSize.x), 0));
        DetermineEnemyIntents();
    }
    public void DetermineEnemyIntents()
    {
        for (int i = 0; i < currentEnemiesInGame.Count; i++)
        {
            currentEnemiesInGame[i].DetermineIntents();
        }
    }
    public void StartEnemyTurn()
    {
        currentEnemiesInGame.Sort((a, b) =>
        {
            Vector2Int aPos = a.GetCurrentCombatSpace().gridPosition;
            Vector2Int bPos = b.GetCurrentCombatSpace().gridPosition;
            if (aPos.y != bPos.y)
            { 
                return aPos.y.CompareTo(bPos.y);
            }
            return aPos.x.CompareTo(bPos.x);
        });
        StartCoroutine(ExecuteEnemyIntents());
    }
    public IEnumerator ExecuteEnemyIntents()
    {
        for (int i = 0; i < currentEnemiesInGame.Count; i++)
        {
            yield return null;
        }
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
                aiming = false;
                targetingTool.EndTargetPreview();
                ToolInGame toolMouseIsOver = Tools.instance.GetToolInGameMouseIsOver();
                if (toolMouseIsOver != null)
                {
                    toolMouseIsOver.PreviewSelectableTargets();
                }
                if (currentLimbMouseOver != null)
                {
                    EnemyLimbMouseClick(currentLimbMouseOver, currentLimbMouseOver.parentEnemyInGame, targetingTool, currentLimbMouseOver.parentEnemyInGame.GetCurrentCombatSpace());
                    HandArea.instance.HandPlayed();
                }
                targetingTool = null;
                TargetingArrows.instance.SetVisibility(false);
            }
            yield return null;
        }
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
        Vector2Int currentTargetPos = currentTarget.GetCurrentCombatSpace().gridPosition;
        Vector2Int currentBoardSize = CombatArea.instance.currentBoardSize;
        if (toolInGame.GetToolTargetStyle() == ToolTargetStyle.EntireColumn)
        {
            int leftX = Mathf.Max(currentTargetPos.x - toolInGame.GetAdjacentColumnsTarget(), 0);
            int rightX = Mathf.Min(currentTargetPos.x + toolInGame.GetAdjacentColumnsTarget(), currentBoardSize.x - 1);
            for (int x = leftX; x <= rightX; x++)
            {
                for (int y = 1; y < currentBoardSize.y; y++)
                {
                    {
                        CombatSpace currentSpace = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(x, y));
                        EnemyInGame occupyingEnemy = currentSpace.GetOccupyingEnemy();
                        if (occupyingEnemy != null && !targetableEnemies.Contains(occupyingEnemy) && currentTarget != occupyingEnemy)
                        {
                            targetableEnemies.Add(occupyingEnemy);
                        }
                    }
                }
            }
        }
        int aoe = toolInGame.GetAreaOfEffect();
        if (aoe <= 0)
        {
            return targetableEnemies;
        }
        int leftXaoe = Mathf.Max(currentTargetPos.x - aoe, 0);
        int rightXaoe = Mathf.Min(currentTargetPos.x + aoe, CombatArea.instance.currentBoardSize.x - 1);
        for(int x = leftXaoe; x <= rightXaoe; x++)
        {
            for(int y = currentTargetPos.y - aoe; y <= currentTargetPos.y + aoe; y++)
            {
                Vector2Int aoePosition = new Vector2Int(x, y);
                if (!CombatArea.instance.IsPositionInCombatArea(aoePosition))
                {
                    continue;
                }
                if (y <= 0) // player only row, skip
                {
                    continue;
                }
                if(Mathf.Abs(currentTargetPos.x - x) + Mathf.Abs(currentTargetPos.y - y) > aoe)
                {
                    continue;
                }
                CombatSpace currentSpace = CombatArea.instance.GetCombatSpaceAtPosition(new Vector2Int(x, y));
                EnemyInGame aoeEnemy = currentSpace.GetOccupyingEnemy();
                if (aoeEnemy != null && !targetableEnemies.Contains(aoeEnemy) && currentTarget != aoeEnemy)
                {
                    targetableEnemies.Add(aoeEnemy);
                }
            }
        }
        return targetableEnemies;
    }
    public void EnemyLimbMouseOver(LimbInGame limbInGame, EnemyInGame enemyInGame)
    {
        if (!targeting || targetingTool == null)
        {
            // Logger.instance.Log("EnemyLimbMouseOver called but not targeting");
            return;
        }
        if (!enemyInGame.GetCurrentCombatSpace().CanTargetCurrently())
        {
            return;
        }
        bool targetingBrokenLimb = aiming && limbInGame.IsDestroyed();
        currentLimbMouseOver = limbInGame;
        if (aiming && !targetingBrokenLimb)
        {
            Logger.instance.Log($"Highlighting limb {limbInGame.limbName} of {enemyInGame.GetEnemyName()}");
            limbInGame.SetHighlightLimb(true);
        }
        else
        {
            enemyInGame.SetHighlightOfAllLimbs(true);
        }
        List<EnemyInGame> otherTargetedEnemies = GetOtherEnemiesTargetableByToolGivenCurrentTarget(targetingTool, enemyInGame);
        Logger.instance.Log($"Also highlighting {otherTargetedEnemies.Count} other enemies targetable by {targetingTool.baseTool.toolName}");
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
        if (!enemyInGame.GetCurrentCombatSpace().CanTargetCurrently())
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
    public void EnemyLimbMouseClick(LimbInGame limbInGame, EnemyInGame enemyInGame, ToolInGame toolInGame, CombatSpace combatSpace)
    {
        Logger.instance.Log($"Targeted {limbInGame.limbName} of {enemyInGame.GetEnemyName()} with {targetingTool.baseTool.toolName}");
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
        Logger.instance.Log($"Applying effect of {toolInGame.baseTool.toolName} to {enemyInGame.GetEnemyName()} in space {enemyInGame.GetCurrentCombatSpace()}, aiming: {aiming}, limbInGame: {limbInGame}");
    }
    public void EnemyDefeated(EnemyInGame defeatedEnemy)
    { 
        defeatedEnemy.GetCurrentCombatSpace().RemoveEnemyFromSpace();
        currentEnemiesInGame.Remove(defeatedEnemy);
        enemiesInGame.Add(defeatedEnemy);
        defeatedEnemy.SetVisibility(false);
    }
    public void ClickEndTurn()
    {
        SetCanEndTurn(false);
        HandArea.instance.TurnEnded();
        StartEnemyTurn();
    }
}
