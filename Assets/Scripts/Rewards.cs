using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rewards : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private Scrollbar horizontalScrollbar;
    [SerializeField] private RectTransform spareRewardButtonParent;
    [SerializeField] private GameObject rewardButtonPrefab;
    public void SetupRewards(List<Reward> rewards)
    { 
        
    }
}
public abstract class Reward
{
    public RewardButton rewardButton;
    public abstract RewardType GetRewardType();
}
public class CardReward : Reward
{
    public List<CardData> cardsToChooseFrom;
    public override RewardType GetRewardType()
    {
        return RewardType.Card;
    }
}
public class CurrencyReward : Reward
{ 
    public int currency;
    public override RewardType GetRewardType()
    {
        return RewardType.Currency;
    }
}
public class ToolReward : Reward
{
    public Tool tool;
    public override RewardType GetRewardType()
    {
        return RewardType.Tool;
    }
}
public class BaubleReward : Reward
{
    public string baubleTag;
    public override RewardType GetRewardType()
    {
        return RewardType.Bauble;
    }
}
public enum RewardType
{ 
    Card,
    Currency,
    Tool,
    Bauble
}