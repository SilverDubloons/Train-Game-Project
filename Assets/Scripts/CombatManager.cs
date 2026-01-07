using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private List<EnemyInGame> enemiesInGame = new List<EnemyInGame>();
    public List<EnemyInGame> currentEnemiesInGame = new List<EnemyInGame>();
    public static CombatManager instance;
    public void SetupInstance()
    {
        instance = this;
    }
    public void SetupCombat(Encounter encounter)
    {
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
    }
}
