using UnityEngine;
using System.Collections;

namespace planeMain
{

public class enemySpawner : MonoBehaviour 
{

	public Transform player;
	public Transform mobprefab;
	private int count = 0;
	public int maxcount = 10;
	public float spawnDelay = 30;
	
	void Start () 
	{
		Invoke("spawn",3);

	}
	
	void spawn()
	{
		if (count<maxcount)
		{
		// pick random location
			Vector3 target = getRandomTarget(WorldBuilder.areasize);
			//Debug.Log(target);
			//int h = WorldBuilder.c.getHeight(target);
			int h = WorldBuilder.c.getHeight(target);
		
			Transform clone = Instantiate(mobprefab,target+new Vector3(0,h,0),Quaternion.identity) as Transform;
			//Assets/PlaneMineCraft/enemySpawner.cs(33,31): error CS0119: Expression denotes a `method group', where a `variable', `value' or `type' was expected
//			clone.GetComponent<GridMoverAI>.player = player;
			clone.GetComponent<GridMoverAI>().player = player;
			Invoke("spawn",spawnDelay);
			
		}else{
			// repeat after delay
			Invoke("spawn",spawnDelay);
		}
	}
	
	Vector3 getRandomTarget(int range)
	{
		return new Vector3(rand(0,range),0,rand(0,range));
	}
	
	int rand(float min,float max)
	{
		return Mathf.RoundToInt(Random.Range(min,max));
	}
	

}

}