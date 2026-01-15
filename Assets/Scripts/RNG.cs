using UnityEngine;
using System;

public class RNG : MonoBehaviour
{
    public RandomNumbers shuffle;	// for shuffling deck
    public RandomNumbers shop;		// for determining items available in shop
    public RandomNumbers starting;	// for start of game, such as adding random cards to player deck
    public RandomNumbers combat;

    public static RNG instance;

    public void SetupInstance()
    {
        instance = this;
    }
    public void InitializeSeed(int newSeed)
    { 
        shuffle.ChangeSeed(newSeed);
        shop.ChangeSeed(newSeed);
        starting.ChangeSeed(newSeed);
        combat.ChangeSeed(newSeed);
    }
    public void LoadCallCountsFromString(int seed, string callCountsString)
    {
        string[] callCountsData = callCountsString.Split('%', StringSplitOptions.RemoveEmptyEntries);
        shuffle.RestoreState(seed, int.Parse(callCountsData[0]));
        shop.RestoreState(seed, int.Parse(callCountsData[1]));
        starting.RestoreState(seed, int.Parse(callCountsData[2]));
        combat.RestoreState(seed, int.Parse(callCountsData[2]));
    }
    public String GetCallCountsAsString()
    {
        return $"{shuffle.GetCurrentCallCount()}%{shop.GetCurrentCallCount()}%{starting.GetCurrentCallCount()}%{shop.GetCurrentCallCount()}";
    }
}
