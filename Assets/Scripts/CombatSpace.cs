using UnityEngine;
using UnityEngine.UI;

public class CombatSpace : MonoBehaviour
{
    [SerializeField] private ButtonPlus buttonPlus;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private RectTransform rt;
    [SerializeField] private RectTransform characterParent;
    [SerializeField] private GameObject slectableObject;
    public Vector2Int gridPosition;
    public EnemyInGame occupyingEnemy;
    private bool targetable;
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
    public void PlaceEnemyInSpace(EnemyInGame enemy)
    {
        occupyingEnemy = enemy;
        if(enemy == null)
        {
            Logger.instance.Error("CombatSpace.PlaceEnemyInSpace: enemy is null");
            return;
        }
        enemy.SetParent(characterParent, this);
    }
    public void PlacePlayerInSpace(Player player)
    {
        player.SetParent(characterParent, this);
    }
    public void SetTargetable(bool newTargetableState, bool aiming)
    {
        targetable = newTargetableState;
        slectableObject.SetActive(newTargetableState);
        if (occupyingEnemy != null)
        {
            occupyingEnemy.SetVisibilityOfLimbCrosshairs(aiming);
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
}
