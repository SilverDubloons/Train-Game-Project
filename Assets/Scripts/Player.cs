using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public RectTransform rt;
    [SerializeField] private Image combatImage;
    [SerializeField] private Label healthLabel;
    [SerializeField] private Image healthImage;
    [SerializeField] private Label shieldLabel;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Label currencyLabel;
    private int maxHealth;
    private int currentHealth;
    private int currentShield;
    public int currency;
    public static Player instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public void SetCurrency(int newCurrencyValue)
    { 
        currency = newCurrencyValue;
        UpdateCurrencyInterface();
    }
    public void AddCurrency(int currencyToAdd)
    {
        currency += currencyToAdd;
        UpdateCurrencyInterface();
    }
    public void SubtractCurrency(int currencyToSubtract)
    {
        currency -= currencyToSubtract;
        UpdateCurrencyInterface();
    }
    public void SetParent(RectTransform newParent)
    {
        rt.SetParent(newParent);
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
    public void UpdateCurrencyInterface()
    {
        currencyLabel.ChangeText(currency.ToString());
    }
}