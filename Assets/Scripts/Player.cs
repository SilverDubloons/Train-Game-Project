using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Image image;
    private CombatSpace currentSpace;
    private int maxHealth;
    private int currentHealth;

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
}