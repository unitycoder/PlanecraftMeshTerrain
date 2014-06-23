using System.Collections;
using UnityEngine;
using planeCraft;

namespace planeMain
{

class GridMoverAI : MonoBehaviour
{
	
	public Transform player;
	public bool aggro = false;
	public bool canClimb = true; // if can do up or down
	
    public float moveSpeed = 1f;
    private float gridSize = 1f;
    private enum Orientation
    {
        Horizontal,
        Vertical
    };
    private Orientation gridOrientation = Orientation.Horizontal;
    private bool allowDiagonals = false;
    private bool correctDiagonalSpeed = true;
    private Vector2 input = new Vector2(0,0);
    public static bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    private float t2;
    private float factor;
	private Vector3 offset = Vector3.zero; //new Vector3(0.5f,0.0f,0.5f);
	
    private float fireRate = 1.0f;
    private float nextFire = 0.0F;

    //private float noiseScale = 0.1f;

    public void Update()
    {
		
		
        if (!isMoving)
        {

            //input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			
			if (aggro)
			{				
				input = new Vector2(0,0);
				
				if ((int)transform.position.z<(int)player.position.z) input.y=1;
				if ((int)transform.position.z>(int)player.position.z) input.y=-1;
				
				if ((int)transform.position.x<(int)player.position.x) input.x=1;
				if ((int)transform.position.x>(int)player.position.x) input.x=-1;
				
			}else{ // move randomly
				
				// autofire change direction
				if (Time.time > nextFire)
				{
					nextFire = Time.time + fireRate;
					if (Random.value>0.5f)
					{
						input = new Vector2(Random.Range(-1,2),0);
					}else{
						input = new Vector2(0,Random.Range(-1,2));
					}
						
					//Debug.Log(input);
				}
			}
			
			
            if (!allowDiagonals)
            {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                {
                    input.y = 0;
                }
                else
                {
                    input.x = 0;
                }
            }

			//if (input != Vector2.zero)
			if (input.y != 0) // forward/backward
			{
					StartCoroutine(move(transform, System.Math.Sign(input.x),System.Math.Sign(input.y)));
			}else{
				
				if (input.x != 0) // strafe
				{
						StartCoroutine(move(transform, System.Math.Sign(input.x),System.Math.Sign(input.y)));
	//					StartCoroutine(rotate(System.Math.Sign(input.x)));
				}else{

					if (Input.GetKey("q")) // rotate
					{
							StartCoroutine(rotate(-1));
						
						//int blockY = WorldBuilder.c.getHeight((int)transform.position.x,(int)transform.position.z);
					//	Debug.Log(transform.position+":"+blockY);
					//	blockY = WorldBuilder.c.getHeight(transform.position);
					//	Debug.Log(transform.position+":"+blockY);
						
					}else{

						if (Input.GetKey("e")) // rotate
						{
								StartCoroutine(rotate(1));
						}
					}
				}
			}

        }
    }

    public IEnumerator rotate(int d)
    {
		
		if (!PlayerControls.zapper) 
		{
			
			isMoving = true;
			t = 0.0f;
			Vector3 r1 = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y+(90.0f*d),transform.eulerAngles.z);
			Quaternion origrot = transform.rotation;
			while (t < 90.0f)
			{
				t += Time.deltaTime * (moveSpeed / gridSize) * 30.0f;
				transform.rotation = Quaternion.RotateTowards(origrot, Quaternion.Euler(r1), t);
				//transform.RotateAround (transform.position+new Vector3(0.5f,0.0f,0.5f), Vector3.up, 90*d);
				//transform.RotateAround (transform.position, Vector3.up, d);
				yield return null;
			}
			
			isMoving = false;
		}
		
	}
	

	
    public IEnumerator move(Transform transform, int dx, int dy)
    {
		
		
		if (!PlayerControls.zapper) 
		{
			isMoving = true;
			startPosition = transform.position;
			t = 0;
			t2 = 0;

			if (gridOrientation == Orientation.Horizontal)
			{
		//		transform.LookAt(endPosition);
				//Debug.Log(dx);
				
	//            endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,startPosition.y, startPosition.z + System.Math.Sign(input.y) * gridSize);
				Vector3 movedir;// = (dx==1 || dy==1)  ?  transform.forward : transform.right; 
				if (dx!=0)
				{
					movedir = transform.right*dx; 
				}else{
					movedir = transform.forward*dy; 
				}
				endPosition = startPosition+ movedir * gridSize; //+new Vector3(System.Math.Sign(input.x) * gridSize,0,System.Math.Sign(input.y) * gridSize);
	//			endPosition = startPosition+movedir * gridSize * dy; //+new Vector3(System.Math.Sign(input.x) * gridSize,0,System.Math.Sign(input.y) * gridSize);
					
					
				//endPosition = startPosition+new Vector3(dx,0,dy) * gridSize; //+new Vector3(System.Math.Sign(input.x) * gridSize,0,System.Math.Sign(input.y) * gridSize);
	//			Debug.Log(transform.forward);
			}
			else
			{
				endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * gridSize,
				startPosition.y + System.Math.Sign(input.y) * gridSize, startPosition.z);
			}

			if (allowDiagonals && correctDiagonalSpeed && input.x != 0 && input.y != 0)
			{
				factor = 0.7071f;
			}
			else
			{
				factor = 1f;
			}
			
			
			// can we move there?
			int boxMove = WorldBuilder.c.canMove(startPosition-offset,endPosition-offset);
			if (boxMove!=-999)
			{
				

				// if boxmove != 0, then we should move first up or down?
				
				if (boxMove==0)
				{
					// normal move
					while (t < 1f)
					{
						t += Time.deltaTime * (moveSpeed / gridSize) * factor;
						transform.position = Vector3.Lerp(startPosition, endPosition, t);
						yield return null;
					}
					
					// fix decimals
				//	Debug.Log("B: "+transform.position.x);
					transform.position = endPosition/2*2;
				//	Debug.Log("A: "+transform.position.x);
					
				}else{ // moving up or down
					
					// first move up
					
					if (boxMove<0)
					{
					//	Debug.Log("first up");
						
						while (t < 1f)
						{
							t += Time.deltaTime * (moveSpeed / gridSize) * factor;
							transform.position = Vector3.Lerp(startPosition, startPosition-new Vector3(0,boxMove,0), t);
							yield return null;
						}
						
						transform.position = startPosition-new Vector3(0,boxMove,0);
						startPosition = transform.position;
						
						//t=0;
						// then move to target
						while (t2 < 1f)
						{
							t2 += Time.deltaTime * (moveSpeed / gridSize) * factor;
							transform.position = Vector3.Lerp(startPosition, endPosition-new Vector3(0,boxMove,0), t2);
							yield return null;
						}
						
						transform.position = endPosition-new Vector3(0,boxMove,0);
						
					}else{ // first move forward
						
					//	Debug.Log("first forward");
						
						while (t2 < 1f)
						{
							t2 += Time.deltaTime * (moveSpeed / gridSize) * factor;
							transform.position = Vector3.Lerp(startPosition, endPosition, t2);
							yield return null;
						}
						
						transform.position = endPosition;
						startPosition = endPosition;
						
						// then down or up
						while (t < 1f)
						{
							t += Time.deltaTime * (moveSpeed / gridSize) * factor;
							transform.position = Vector3.Lerp(startPosition, startPosition-new Vector3(0,boxMove,0), t);
							yield return null;
						}
						
						//Debug.Log("a "+transform.position.x);
						transform.position = startPosition-new Vector3(0,boxMove,0);
					//	Debug.Log("b "+transform.position.x);
						
					}
				}
			//}else{ // cannot go there
			//	Debug.Log("cannot go there:"+(endPosition-offset));
			}
			isMoving = false;
			//Invoke("release", 0.1f);

			yield return 0;
		}
		yield return 0;
    }
	
	void release()
	{
		isMoving = false;
	}
	
} // class

} //ns