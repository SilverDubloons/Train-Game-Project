using UnityEngine;
using UnityEngine.EventSystems;

public class LimbInGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public RectTransform rt;
    public RectTransform crosshairRt;
    public UnityEngine.UI.Image crosshairImage;
    public GameObject visibilityObject;
    public UnityEngine.UI.Image image;
    public string limbName;
    public LimbTag[] limbTags;
    public int maxHealth;
    public int currentHealth;
    public EnemyInGame parentEnemyInGame;
    private void Start()
    {
        image.alphaHitTestMinimumThreshold = 0.1f;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatManager.instance.EnemyLimbMouseOver(this, parentEnemyInGame);
    }
    public void SetHighlightLimb(bool highlight)
    {
        if (IsDestroyed())
        {
            image.color = Color.darkRed;
            return;
        }
        if (highlight)
        {
            image.color = r.i.themeManager.GetColorFromCurrentTheme(UIElementType.enemyMouseOver);
        }
        else
        {
            image.color = Color.white;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CombatManager.instance.EnemyLimbMouseExit(this, parentEnemyInGame);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // Handle limb click
    }
    public void SetupFromLimb(Limb limb, Enemy enemy, EnemyInGame parentEnemy)
    {
        limbName = limb.limbName;
        limbTags = limb.limbTags;
        image.sprite = limb.sprite;
        rt.sizeDelta = limb.size;
        rt.anchoredPosition = limb.location;
        crosshairRt.anchoredPosition = Vector3.zero;
        crosshairImage.color = enemy.crosshairColor;
        parentEnemyInGame = parentEnemy;
        maxHealth = limb.maxHealth;
        currentHealth = limb.startingHealth;
        name = parentEnemy.name + "_" + limbName;
    }
    public void SetVisibility(bool visible)
    {
        visibilityObject.SetActive(visible);
    }
    public void SetVisibilityOfCrosshair(bool visible)
    { 
        crosshairRt.gameObject.SetActive(visible);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            image.color = Color.darkRed;
        }
    }
    public bool IsDestroyed()
    {
        return currentHealth <= 0;
    }
}