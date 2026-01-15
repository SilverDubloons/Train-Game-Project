using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public int GetMaxHandSize()
    {
        return 15;
    }
    public int GetMaxGapInStraights()
    {
        return 0;
    }
    public bool GetCanStraightsWrap()
    {
        return false;
    }
}
