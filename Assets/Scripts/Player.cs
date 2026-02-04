using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Image combatImage;
    [SerializeField] private Label healthLabel;
    [SerializeField] private Image healthImage;
    [SerializeField] private Label shieldLabel;
    [SerializeField] private Image shieldImage;
    private CombatSpace currentSpace;
    private int maxHealth;
    private int currentHealth;
    private int currentShield;
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
        UpdateHealthInterface();
    }
    public void SetCurrentHealth(int newCurrentHealth)
    {
        currentHealth = newCurrentHealth;
        UpdateHealthInterface();
    }
    public void SetShield(int newShield)
    {
        currentShield = newShield;
        UpdateShieldInterface();
    }
    public void AddShield(int shieldToAdd)
    {
        currentShield += shieldToAdd;
        UpdateShieldInterface();
    }
    public void TakeDamage(int damage)
    {
        if (currentShield > 0)
        {
            if (damage <= currentShield)
            {
                currentShield -= damage;
                damage = 0;
            }
            else
            {
                damage -= currentShield;
                currentShield = 0;
            }
            UpdateShieldInterface();
        }
        if (damage > 0)
        {
            currentHealth -= damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            UpdateHealthInterface();
        }
        Logger.instance.Log($"Player took {damage} damage and is now at {currentHealth}/{maxHealth} HP");
    }
    public void UpdateHealthInterface()
    {
        healthLabel.ChangeText($"{currentHealth}/{maxHealth}");
    }
    public void UpdateShieldInterface()
    {
        if (currentShield == 0)
        {
            shieldLabel.ChangeText("");
            shieldImage.gameObject.SetActive(false);
        }
        else
        {
            shieldLabel.ChangeText($"{currentShield}");
            shieldImage.gameObject.SetActive(true);
        }
    }
}