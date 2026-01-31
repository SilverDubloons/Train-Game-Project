using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Image image;
    private CombatSpace currentSpace;
    private int maxHealth;
    private int currentHealth;
    public static Player instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public void SetPlayerPosition(CombatSpace space)
    {
        currentSpace = space;
        space.PlacePlayerInSpace(this);
    }
    public void SetParent(RectTransform newParent, CombatSpace space)
    {
        rt.SetParent(newParent);
        rt.anchoredPosition = Vector2.zero;
    }
    public CombatSpace GetCurrentSpace()
    { 
        return currentSpace;
    }
    public void SetMaxHealth(int newMaxHealth)
    { 
        maxHealth = newMaxHealth;
    }
    public void SetCurrentHealth(int newCurrentHealth)
    { 
        currentHealth = newCurrentHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        Logger.instance.Log($"Player took {damage} damage and is now at {currentHealth}/{maxHealth} HP");
    }
}