using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {

	public Transform cam1;
	public Transform cam2;
	private bool cam = true;
	
	void Update () 
	{
		
		if (Input.GetKeyDown("c"))
		{
			
			cam=!cam;
			if (cam)
			{
				//Camera.main.transform.position = cam1.position;
				Camera.main.transform.parent = cam1;
				Camera.main.transform.localPosition = Vector3.zero;
				Camera.main.transform.localRotation = Quaternion.identity;
			}else{
	//			Camera.main.transform.position = cam2.position;
				//Camera.main.transform.rotation = Quaternion.identity;
				Camera.main.transform.parent = cam2;
				Camera.main.transform.localPosition = Vector3.zero;
				Camera.main.transform.localRotation = Quaternion.identity;
			}
		}
	}
}
