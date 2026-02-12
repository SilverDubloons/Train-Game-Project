using System.Collections.Generic;
using UnityEngine;
public class StatusEffectsOnCharacter : MonoBehaviour
{
    //public Dictionary<Status, int> currentStatuses = new Dictionary<Status, int>();
    private List<CurrentStatus> currentStatuses = new List<CurrentStatus>();
    [SerializeField] private RectTransform statusEffectUIParent;
    [SerializeField] private EnemyInGame enemyInGame;
    [SerializeField] private bool isPlayer = false;
    public void ResetStatusEffects()
    {
        for (int i = 0; i < currentStatuses.Count; i++)
        {
            currentStatuses[i].statusEffectUI.RetireStatusEffect();
        }
        currentStatuses.Clear();
    }
    public void UpdateStatusUI()
    {
        float currentX = 0f;
        for (int i = 0; i < currentStatuses.Count; i++)
        {
            currentStatuses[i].statusEffectUI.SetPosition(currentX);
            currentX += currentStatuses[i].statusEffectUI.GetIntentWidth();
        }
        if (!isPlayer)
        {
            enemyInGame.UpdateStatusEffectsUI();
        }
    }
    public void AddStatus(Status status, int magnitude)
    {
        for (int i = 0; i < currentStatuses.Count; i++)
        {
            if (currentStatuses[i].status == status)
            {
                currentStatuses[i].AddMagnitude(magnitude);
                UpdateStatusUI();
                return;
            }
        }
        CurrentStatus newCurrentStatus = new CurrentStatus(status, magnitude, StatusEffects.instance.GetStatusEffectUI());
        newCurrentStatus.statusEffectUI.SetupStatusEffectUI(newCurrentStatus.status, newCurrentStatus.magnitude, statusEffectUIParent);
        currentStatuses.Add(newCurrentStatus);
        UpdateStatusUI();
    }
    public int GetStatusMagnitude(Status status)
    {
        for (int i = 0; i < currentStatuses.Count; i++)
        {
            if (currentStatuses[i].status == status)
            {
                return currentStatuses[i].magnitude;
            }
        }
        return 0;
    }
    public bool CharacterHasAtLeastOneStatusEffect()
    { 
        return currentStatuses.Count > 0;
    }
    public void CharactersTurnHasEnded()
    { 
    
    }
    public void CharactersTurnHasStarted()
    { 
    
    }
}
public class CurrentStatus
{
    public Status status;
    public int magnitude; // could be duration or effect, depending on status
    public StatusEffectUI statusEffectUI;
    public void AddMagnitude(int magnitudeToAdd)
    {
        magnitude += magnitudeToAdd;
        statusEffectUI.UpdateMagnitude(magnitude);
    }
    public CurrentStatus(Status status, int magnitude, StatusEffectUI statusEffectUI)
    { 
        this.status = status;
        this.magnitude = magnitude;
        this.statusEffectUI = statusEffectUI;
    }
}