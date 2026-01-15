using UnityEngine;
using UnityEngine.UI;

public class CombatSpace : MonoBehaviour
{
    [SerializeField] private ButtonPlus buttonPlus;
    [SerializeField] private GameObject visibilityObject;
    [SerializeField] private RectTransform rt;
    [SerializeField] private RectTransform characterParent;
    public Vector2Int gridPosition;
    public EnemyInGame occupyingEnemy;
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
    public void SetTargetable(bool targetable)
    { 
    
    }
    public bool IsTargetable()
    {
        return occupyingEnemy != null;
    }
}
