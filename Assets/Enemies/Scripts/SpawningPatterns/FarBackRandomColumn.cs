using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
[CreateAssetMenu(fileName = "FarBackRandomColumn",menuName = "Combat/Spawning Patterns/Far Back Random Column")]
public class FarBackRandomColumn : SpawningPattern
{
    public override CombatSpace GetSpawnSpace(CombatSpace[,] availableSpaces)
    {
        // int currentY = boardSize.y - 1;
        int currentY = availableSpaces.GetLength(1) - 1;
        while(currentY >= 0)
        {
            List<CombatSpace> rowSpaces = new List<CombatSpace>();
            /*rowSpaces = availableSpaces.FindAll(space => space.gridPosition.y == currentY);
            rowSpaces.RemoveAll(space => !space.CanPlaceEnemy());*/
            for (int i = 0; i < availableSpaces.GetLength(0); i++)
            {
                if (availableSpaces[i, currentY].CanPlaceEnemy())
                { 
                    rowSpaces.Add (availableSpaces[i, currentY]);
                }
            }
            if (rowSpaces.Count > 0)
            {
                int randomIndex = Random.Range(0, rowSpaces.Count);
                return rowSpaces[randomIndex];
            }
            currentY--;
        }
        Logger.instance.Error("FarBackRandomColumn no spaces available for spawning.");
        return null;
    }
}
