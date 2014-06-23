using UnityEngine;
using System.Collections;

namespace planeMain
{

public class missileManager : MonoBehaviour 
{
	public Transform explo;
	public float exploRadius = 5;
	
	private Vector3 projectedPosition;
	private Vector3 previousPosition;
	
	void Start()
	{
		previousPosition = transform.position;
	}
	
	void LateUpdate () 
	{
		// check for collision on terrain
		
		float dist = Vector3.Distance(transform.position,previousPosition);
		//Vector3 dir = (previousPosition-transform.position).normalized;
		
		Vector3 newpos = CalcFuturePos();
		Debug.DrawLine(transform.position,newpos,Color.red,3);
		
		if (transform.position.y>0)
		{
			//int map = WorldBuilder.c.canMove(transform.position,newpos+new Vector3(0,0.5f,0));
			int blockY = WorldBuilder.c.getHeight(transform.position);
			
			if (transform.position.y<blockY)
			{
				//Debug.Log(transform.position+":"+blockY);
				// check nearby collisions
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, exploRadius);
				int i = 0;
				while (i < hitColliders.Length) {
					float explodist = Vector3.Distance(hitColliders[i].transform.position,transform.position);
					hitColliders[i].SendMessage("ApplyDamage", 1+explodist, SendMessageOptions.DontRequireReceiver);
					i++;
				}
				
				
				Transform clone = Instantiate(explo,transform.position,Quaternion.identity) as Transform;
				Destroy(gameObject);
				Destroy(clone,10);
			}
		}else{
			Destroy(gameObject);
		}
		//previousPosition = transform.position;
		
	}
	
	// TODO: check direct collider collisions (enemies)
    void OnCollisionEnter(Collision collision) 
	{
		//Debug.Log("hit something:"+collision.gameObject.tag);
		if (collision.gameObject.tag == "mobs")
		{
			collision.gameObject.SendMessage("ApplyDamage", 10.0F, SendMessageOptions.DontRequireReceiver);
		}
		
    }
	
	private float secondsInAdvance = 0.1f;
    private int framesInAdvance = 1;
     
    bool useFrameCounter = true;
     
    Vector3 CalcFuturePos ()
	{
		Vector3 finalPos  = transform.position;
		Vector3 velocity = rigidbody.velocity;
		 
		if(useFrameCounter) {
		velocity *= Time.deltaTime;
		velocity *= framesInAdvance;
		 
		finalPos += velocity;
		}
		 
		//else {
		//velocity *= secondsInAdvance;
		//finalPos += velocity;
		//}
		 
		return finalPos;
    }
	
}

}