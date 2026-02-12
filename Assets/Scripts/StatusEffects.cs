using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    [SerializeField] private StatusIcon[] statusIcons;
    [SerializeField] private RectTransform spareStatusEffectUIParent;
    private Dictionary<Status, Sprite> statusIconDictionary;
    public GameObject statusEffectUIPrefab;
    public static StatusEffects instance;
    public void SetupInstance()
    {
        instance = this;
        SetupStatusIconDictionary();
    }
    public void SetupStatusIconDictionary()
    {
        statusIconDictionary = new Dictionary<Status, Sprite>();
        for (int i = 0; i < statusIcons.Length; i++)
        {
            statusIconDictionary[statusIcons[i].status] = statusIcons[i].icon;
        }
    }
    public Sprite GetStatusSprite(Status status)
    {
        return statusIconDictionary[status];
    }
    public StatusEffectUI GetStatusEffectUI()
    {
        if (spareStatusEffectUIParent.childCount > 0)
        { 
            return spareStatusEffectUIParent.GetChild(spareStatusEffectUIParent.childCount - 1).GetComponent<StatusEffectUI>();
        }
        return Instantiate(statusEffectUIPrefab, spareStatusEffectUIParent).GetComponent<StatusEffectUI>();
    }
    public void RetireStatusEffectUI(StatusEffectUI statusEffectUI)
    {
        statusEffectUI.SetVisibility(false);
        statusEffectUI.rt.SetParent(spareStatusEffectUIParent);
    }
}
[System.Serializable]
public struct StatusIcon
{
    public Status status;
    public Sprite icon;
}
public enum Status
{
    DamageBonus,
    ShieldBonus,
    Poison
}