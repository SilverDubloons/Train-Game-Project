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
        return 4;
    }
}
