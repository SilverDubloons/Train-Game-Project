using System.Collections.Generic;
using UnityEngine;
// This is the service locator for the project. Called r for 'references'
public class r : MonoBehaviour
{
    public static r i;

    [SerializeField] public Interface interf;
    [SerializeField] public ThemeManager themeManager;
    [SerializeField] public Enemy[] enemies;
    public GameObject limbInGamePrefab;
    public GameObject enemyInGamePrefab;
    public GameObject cardPrefab;
    public Dictionary<string, Enemy> enemyDictionary = new Dictionary<string, Enemy>();

    public void SetupInstance()
    {
        if (i != null && i != this)
        {
            Destroy(gameObject);
            return;
        }
        foreach (Enemy enemy in enemies)
        {
            enemyDictionary[enemy.enemyTag] = enemy;
        }
        i = this;
        DontDestroyOnLoad(transform.parent);
    }
}