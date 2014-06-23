using UnityEngine;
using System.Collections;

public class enemyManager : MonoBehaviour 
{

	private bool aggro = false; // if no aggro, move randomly or sleep
	private Vector3 target;
	
	void Start () 
	{
		// pick random target nearby
		target = transform.position+getRandomTarget();
		
		//Invoke ("setTarget",3);
		
	}
	
//	void Update () 
//	{
//	}

	void setTarget()
	{
		// check if aggro to player?
		// or move randomly
	}
	
	Vector3 getRandomTarget()
	{
		return new Vector3(rand(-5,5),0,rand(-5,5));
	}
	
	float rand(float min,float max)
	{
		return Random.Range(min,max);
	}
	
}
