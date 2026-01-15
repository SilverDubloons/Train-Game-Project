using UnityEngine;
using System.Collections;
using Unity.Loading;

public class GameplayStart : MonoBehaviour
{
    [SerializeField] private CombatArea combatArea;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private MovingObjects movingObjects;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private RNG rng;
    [SerializeField] private GameDeck gameDeck;
    [SerializeField] private HandArea handArea;
    [SerializeField] private Tools tools;
    void Awake()
    {
        combatArea.SetupInstance();
        combatManager.SetupInstance();
        movingObjects.SetupInstance();
        gameManager.SetupInstance();
        rng.SetupInstance();
        gameDeck.SetupInstance();
        handArea.SetupInstance();
        tools.SetupInstance();

        gameDeck.CreateStandardDeck();
        bool loadingGame = false;
        if (loadingGame)
        {
            // rng.RestoreState(savedSeed, savedRngCallCount);
        }
        else
        {
            rng.InitializeSeed(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        }
        MovingObjects.instance.mo["GameplayMenu"].TeleportTo("OffScreen");
        MovingObjects.instance.mo["GameplayMenu"].StartMove("OnScreen");
        MovingObjects.instance.mo["CombatArea"].TeleportTo("OffScreen");
        MovingObjects.instance.mo["DrawPile"].TeleportTo("OffScreen");
        StartCoroutine(WaitAndStart());
    }
    private IEnumerator WaitAndStart()
    {
        yield return null;
        TransitionStinger.instance.sceneLoaded = true;
    }
}
