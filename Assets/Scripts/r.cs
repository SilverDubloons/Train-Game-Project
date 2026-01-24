using System.Collections.Generic;
using UnityEngine;
// This is the service locator for the project. Called r for 'references'
public class r : MonoBehaviour
{
    public static r i;

    public Interface interf;
    public ThemeManager themeManager;
    public Enemy[] enemies;
    public Encounter[] encounters;
    public Tool[] tools;

    public GameObject limbInGamePrefab;
    public GameObject enemyInGamePrefab;
    public GameObject cardPrefab;
    public Dictionary<string, Enemy> enemyDictionary = new Dictionary<string, Enemy>();
    public Dictionary<string, Encounter> encounterDictionary = new Dictionary<string, Encounter>();
    public Dictionary<string, Tool> toolDictionary = new Dictionary<string, Tool>();
    public Canvas persistantCanvas;

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
        foreach (Encounter encounter in encounters)
        {
            encounterDictionary[encounter.encounterTag] = encounter;
        }
        foreach (Tool tool in tools)
        {
            toolDictionary[tool.toolTag] = tool;
        }
        i = this;
        DontDestroyOnLoad(transform.parent);
    }
}