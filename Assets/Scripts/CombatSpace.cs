using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class CombatSpace : MonoBehaviour
{
    [SerializeField] private ButtonPlus buttonPlus;
    [SerializeField] private GameObject visibilityObject;
    public RectTransform rt;
    [SerializeField] private RectTransform characterParent;
    // [SerializeField] private GameObject slectableObject;
    [SerializeField] private PingPongMovement[] pingPongMovements;
    public Vector2Int gridPosition;
    public EnemyInGame occupyingEnemy;
    private bool targetable;
    private List<EnemyIntentAttack> currentIntentAttacks = new List<EnemyIntentAttack>();
    public void SetVisibility(bool visible)
    {
        if (!visible)
        {
            this.name = "Unused";
        }
        visibilityObject.SetActive(visible);
    }
    public void SetInteractability(bool interactable)
    {
        buttonPlus.SetButtonEnabled(interactable);
    }
    public void SetPosition(Vector2 position)
    {
        rt.anchoredPosition = position;
    }
    public bool CanPlaceEnemy()
    {
        return occupyingEnemy == null;
    }
    public void PlaceEnemyInSpace(EnemyInGame enemy, bool moveToSpace, bool setAsParent = false)
    {
        occupyingEnemy = enemy;
        if(enemy == null)
        {
            Logger.instance.Error("CombatSpace.PlaceEnemyInSpace: enemy is null");
            return;
        }
        enemy.SetCurrentCombatSpace(this, moveToSpace, setAsParent ? characterParent : null);
    }
    public void PlacePlayerInSpace(Player player)
    {
        player.SetParent(characterParent, this);
    }
    public void SetTargetable(bool newTargetableState, bool aiming)
    {
        targetable = newTargetableState;
        // slectableObject.SetActive(newTargetableState);
        SetHighlight(targetable ? 2 : 0, r.i.themeManager.GetColorFromCurrentTheme(UIElementType.targetableSpacePlayer));
        if (occupyingEnemy != null)
        {
            occupyingEnemy.SetVisibilityOfLimbCrosshairs(aiming);
        }
    }
    public void SetTargetableByEnemy(int numberOfAttacks)
    {
        SetHighlight(numberOfAttacks, r.i.themeManager.GetColorFromCurrentTheme(UIElementType.targetableSpaceEnemy));
    }
    private void SetHighlight(int numberOfHighligts, Color newColor)
    {
        for (int i = 0; i < pingPongMovements.Length; i++)
        {
            if (i < numberOfHighligts)
            {
                pingPongMovements[i].Setup(i / numberOfHighligts, newColor);
            }
            else
            {
                pingPongMovements[i].Deactivate();
            }
        }
    }
    public bool IsTargetable()
    {
        return occupyingEnemy != null;
    }
    public bool CanTargetCurrently()
    {
        return targetable && occupyingEnemy != null;
    }
    public void SetHighlightOfEnemyInSpace(bool highlight)
    { 
        if(!targetable || occupyingEnemy == null)
        {
            return;
        }
        occupyingEnemy.SetHighlightOfAllLimbs(highlight);
    }
    public EnemyInGame GetOccupyingEnemy()
    {
        return occupyingEnemy;
    }
    public void RemoveEnemyFromSpace()
    { 
        occupyingEnemy = null;
    }
    public void Click()
    {
        CombatArea.instance.SetPlayerPosition(this);
        HandArea.instance.HandPlayed();
    }
    public bool EnemyInSpace()
    {
        return occupyingEnemy != null;
    }
    public RectTransform GetRectTransform()
    {
        return rt;
    }
    public void AttackMissedHere()
    {
        // in case I want to add a little animation
    }
    public void ResetCurrentIntentAttacks()
    {
        currentIntentAttacks.Clear();
    }
    public void AddEnemyIntentAttack(EnemyIntentAttack enemyIntentAttack)
    {
        currentIntentAttacks.Add(enemyIntentAttack);
    }
    public void EnemyIntentsDetermined()
    {
        SetHighlight(currentIntentAttacks.Count, r.i.themeManager.GetColorFromCurrentTheme(UIElementType.targetableSpaceEnemy));
    }
    public void RemoveEnemyIntentAtack(EnemyIntentAttack enemyIntentAttack)
    { 
        if(!currentIntentAttacks.Contains(enemyIntentAttack))
        {
            Logger.instance.Warning($"Attempted to remove enemyIntentAttack {enemyIntentAttack.intentName} from {gridPosition} but it was not in currentIntentAttacks");
            return;
        }
        currentIntentAttacks.Remove(enemyIntentAttack);
        EnemyIntentsDetermined();
    }
}
