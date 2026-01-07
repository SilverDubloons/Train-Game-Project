using UnityEngine;

[CreateAssetMenu(fileName = "FarBackRandomColumn", menuName = "Combat/Encounter")]
public class Encounter : ScriptableObject
{
    public Enemy[] enemies;
    public Vector2Int boardSize;
}
