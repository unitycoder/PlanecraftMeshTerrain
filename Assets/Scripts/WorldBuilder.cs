using UnityEngine;
using System.Collections;
using planeCraft;

namespace planeMain
{

public class WorldBuilder : MonoBehaviour 
{

	public static Mesh mesh;
	public Transform player;
	public static Chunk c;
	public static int areasize = 88;
	Vector3 offset = new Vector3(0.5f,0.0f,0.5f);
	
	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter>().mesh;
		
		c = new Chunk(0,0,areasize);
		c.rebuildVertices();
		rebuildMesh();
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if (Input.GetKeyDown("1"))
		{
			c.randomize();
			c.rebuildVertices();
			rebuildMesh();
			
			// drop our player to ground level
			player.position = new Vector3(player.position.x,c.getHeight(player.position-offset),player.position.z);
			
		}
		
		if (Input.GetKeyDown("2"))
		{
			c.randomize2();
			c.rebuildVertices();
			rebuildMesh();
			
			// drop our player to ground level
			player.position = new Vector3(player.position.x,c.getHeight(player.position-offset),player.position.z);
			
		}
	
	}
	
	public static void rebuildMesh()
	{
		mesh.Clear();
		mesh.vertices = c.getVertices();
		mesh.uv = c.getUVs();
		mesh.colors = c.getColors();
		mesh.triangles = c.getTriangles();
		mesh.RecalculateNormals();
		// tangents. optimize--?
		mesh.RecalculateBounds();
	}
	
}

}