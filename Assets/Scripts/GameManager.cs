using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Canvas gameplayCanvas;
    public Camera gameplayCamera;
    public static GameManager instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public int GetMaxHandSize()
    {
        int maxHandSize = 7;
        // maxHandSize += Baubles.instance.GetImpactInt("IncreaseHandSize");
        // maxHandSize += MetaProgression.instance.GetImpactInt("IncreaseHandSize");
        return maxHandSize;
    }
    public int GetMaxGapInStraights()
    {
        return 0;
    }
    public bool GetCanStraightsWrap()
    {
        return false;
    }
    public int GetMaxTools()
    {
        return 8;
    }
}
