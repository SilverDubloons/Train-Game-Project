using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MovingObjects : MonoBehaviour
{
    public MovingObject[] movingObjects;
	public Dictionary<string, MovingObject> mo = new Dictionary<string, MovingObject>();
	
	public static MovingObjects instance;
	
	public void SetupInstance()
	{
		instance = this;
		SetupMovingObjects();
	}
	
	private void SetupMovingObjects()
	{
		for(int i = 0; i < movingObjects.Length; i++)
		{
			mo.Add(movingObjects[i].referenceName, movingObjects[i]);
			movingObjects[i].SetupLocationsDictionary();
		}
	}
}
