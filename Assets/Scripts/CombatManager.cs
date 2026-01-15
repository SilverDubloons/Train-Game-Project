using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private List<EnemyInGame> enemiesInGame = new List<EnemyInGame>();
    public List<EnemyInGame> currentEnemiesInGame = new List<EnemyInGame>();
    public static CombatManager instance;
    public bool inCombat = false;
    public void SetupInstance()
    {
        instance = this;
    }
    public void SetupCombat(Encounter encounter)
    {
        inCombat = true;
        currentEnemiesInGame.Clear();
        CombatArea.instance.SetupBoard(encounter.boardSize);
        for (int i = 0; i < encounter.enemies.Length; i++)
        {
            EnemyInGame newEnemyInGame;
            if (enemiesInGame.Count > i)
            {
                newEnemyInGame = enemiesInGame[i];
            }
            else
            {
                newEnemyInGame = Instantiate(r.i.enemyInGamePrefab).GetComponent<EnemyInGame>();
                enemiesInGame.Add(newEnemyInGame);
            }
            newEnemyInGame.SetupEnemyInGame(encounter.enemies[i]);
            CombatManager.instance.currentEnemiesInGame.Add(newEnemyInGame);
            CombatArea.instance.GetSpawnSpaceForEnemy(encounter.enemies[i]).PlaceEnemyInSpace(newEnemyInGame);
        }
        for (int i = encounter.enemies.Length; i < enemiesInGame.Count; i++)
        {
            enemiesInGame[i].gameObject.SetActive(false);
        }
        HandArea.instance.StartDrawCards(true);
        CombatArea.instance.SetPlayerPosition(new Vector2Int(RNG.instance.combat.Range(0, encounter.boardSize.x), 0));
    }
    public void HighlightEnemyAttacks()
    {
        for (int i = 0; i < enemiesInGame.Count; i++)
        {

        }
    }
}
