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
    [SerializeField] private TargetingArrows targetingArrows;
    [SerializeField] private CardBurning cardBurning;
    [SerializeField] private Player player;
    [SerializeField] private ActionAnimators actionAnimators;
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
        targetingArrows.SetupInstance();
        cardBurning.SetupInstance();
        player.SetupInstance();
        actionAnimators.SetupInstance();
        
        bool loadingGame = false;
        if (loadingGame)
        {
            // rng.RestoreState(savedSeed, savedRngCallCount);
        }
        else
        {
            gameDeck.CreateStandardDeck();
            rng.InitializeSeed(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            player.SetMaxHealth(60);
            player.SetCurrentHealth(60);
        }
        MovingObjects.instance.mo["GameplayMenu"].TeleportTo("OffScreen");
        MovingObjects.instance.mo["GameplayMenu"].StartMove("OnScreen");
        MovingObjects.instance.mo["CombatArea"].TeleportTo("OffScreen");
        MovingObjects.instance.mo["DrawPile"].TeleportTo("OffScreen");
        MovingObjects.instance.mo["DiscardPile"].TeleportTo("OffScreen");
        MovingObjects.instance.mo["EndTurnButtonBackdrop"].TeleportTo("OffScreen");
        r.i.persistantCanvas.worldCamera = GameManager.instance.gameplayCamera;
        StartCoroutine(WaitAndStart());
    }
    private IEnumerator WaitAndStart()
    {
        yield return null;
        TransitionStinger.instance.sceneLoaded = true;
    }
}
