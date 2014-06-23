using UnityEngine;
using System.Collections;
//Assets/PlaneMineCraft/MobDamageControl.cs(14,58): error CS0246: The type or namespace name `GridMoverAI' could not be found. Are you missing a using directive or an assembly reference?
// needs namespace:
using planeMain;

public class MobDamageControl : MonoBehaviour 
{
	public float hp = 15.0f;
	public Transform explofx;
	public Transform drop;
	
	void ApplyDamage(float dmg)
	{
		// become aggro
		transform.root.gameObject.GetComponent<GridMoverAI>().aggro = true;
		transform.root.gameObject.GetComponent<GridMoverAI>().moveSpeed *= 2;
		
		
		hp-=dmg;
		if (hp<0)
		{
			// explo?
			Transform clone = Instantiate(explofx,transform.position,Quaternion.identity) as Transform;
			Destroy(clone,10);
			
			// TODO: create random drop?
			
			
			Destroy(gameObject.transform.parent.gameObject);
		}
	}
	
}
