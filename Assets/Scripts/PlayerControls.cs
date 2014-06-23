using UnityEngine;
using System.Collections;

namespace planeMain
{
	
public class PlayerControls : MonoBehaviour 
{
	
	// zapper
	public Transform digFX;
	public GUITexture guiHP;
	public GUITexture guiFirePower;
	
	private float firePower = 128;
	private float maxfirePower = 128;
	
	public int hp = 10;
	public int maxhp = 10;
	public float GUITextureOffset = 24;
	
	public Transform cannon;
	public Transform cannonShooter;
	public Transform missile;
	
	private bool cannonReady = true;
	
	private float zapperTime = 0.5f;
	public static bool zapper = false;
	private float t = 0.0f;
	Vector3 offset = new Vector3(0.5f,0.0f,0.5f);
	private LineRenderer lineRenderer;
	
	// Use this for initialization
	void Start () {
		lineRenderer = gameObject.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// cannon
		if (cannon)
		{
			//Vector3 t = Camera.main.ViewportToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000.0f));
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			float damping = 3.0f;
			 //target;
			//int zDistance = 10;

			Vector2 mousePos = Input.mousePosition;
			//float cameraDif = Camera.main.transform.position.y - cannon.position.y;
			//if (mousePos.y<0.5f) mousePos.y = 0.5f;
			Vector3 target = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 50));
			if (target.y<-18) target.y = -18;
//			Debug.Log(target);
			Quaternion rotation = Quaternion.LookRotation(target - cannon.position);
			cannon.rotation = Quaternion.Slerp(cannon.rotation, rotation, Time.deltaTime * damping);
				
			
			//cannon.LookAt(ray.GetPoint(100));
			
			//Debug.DrawRay(cannon.position, ray.GetPoint(100), Color.red);
			
			// shoot, if reloaded
			if (Input.GetMouseButtonDown(0) && cannonReady)
			{
				Transform clone = Instantiate(missile,cannonShooter.position,cannon.rotation) as Transform;
				clone.rigidbody.velocity = cannon.forward.normalized*(firePower*0.1f);
				Destroy(clone.gameObject,5);
			}
			
			float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
	
			if (scrollWheel!=0)
			{
				firePower = Mathf.Clamp(firePower+scrollWheel*25,0,maxfirePower);
				guiFirePower.pixelInset = new Rect(4,0,16,firePower);
				guiFirePower.color = Color.Lerp(Color.green, Color.red, firePower/maxfirePower);
			}
			
		}
		
		
		
		// digging
		if (Input.GetKeyDown("space"))
		{
			if (!zapper && !GridMoveBoxWorld.isMoving)
			{
				
				int b = WorldBuilder.c.canMove(transform.position-offset,transform.position+transform.forward-offset);
				//Debug.Log(b);
				if (b<0) // is there something ahead
				{
					StartCoroutine(zap());
					updateHPGui();
					
					
					//guiHP.pixelInset.x -= GUITextureOffset;
					
				}
				
			}
		}
	}
	
	void updateHPGui()
	{
		guiHP.pixelInset = new Rect(guiHP.pixelInset.x-GUITextureOffset,0,256,32);
	}
	
    public IEnumerator zap()
    {
        zapper = true;
		t = 0.0f;
		//Vector3 r1 = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y+(90.0f*d),transform.eulerAngles.z);
		//Quaternion origrot = transform.rotation;
		
		//Debug.DrawLine(transform.position-offset,(transform.position-offset+transform.forward),Color.green, 10.0f);
		
		Transform clone = Instantiate(digFX,transform.position+transform.forward,Quaternion.identity) as Transform;
		
		while (t < zapperTime)
		{
			t += Time.deltaTime * 1.0f;
			lineRenderer.SetPosition(0, transform.position+transform.forward*0.5f);
			lineRenderer.SetPosition(1, transform.position+transform.forward+Random.onUnitSphere*1.5f);
			yield return null;
		}
		
		//Debug.Log(clone.name);
		Destroy(clone.gameObject);
		
		// destroy block
		//WorldBuilder.c.destroyBlock(transform.position-offset+transform.forward);
//		Debug.Log("pos:"+transform.position);
		
		lineRenderer.SetPosition(0, Vector3.zero);
		lineRenderer.SetPosition(1, Vector3.zero);

		
		//Debug.Log(transform.position-offset+transform.forward);
		
		//WorldBuilder.c.destroyBlock(transform.position-offset+transform.forward);
		WorldBuilder.c.destroyBlock(transform.position+transform.forward-offset);
		
		WorldBuilder.rebuildMesh();
		
		zapper = false;
		
		
	}

	
}

}

