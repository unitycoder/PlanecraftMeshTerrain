using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// wrapped array: http://stackoverflow.com/questions/7862190/nearest-neighbor-operation-on-1d-array-elements

namespace planeCraft
{

	public class Chunk 
	{
		private Vector2[] atlas = new[]
		{
//			new Vector2(2,4), // 0 = grass
//			new Vector2(1,10), // 1 = .dirt
//			new Vector2(10,4), // 2 = stone
//			new Vector2(0,11), // 3 = rock?
			
			new Vector2(2,15), // 1 = .dirt
			new Vector2(1,6), // 0 = grass
			new Vector2(1,15), // 2 = stone
			new Vector2(3,14), // 3 = rock?
		};

		private int X;
		private int Z; // Y is up
		private int SIZE;
		private int[] HEIGHTS;
		private Vector3[] VERTICES;
		private Vector2[] UVS;
		private Color[] COLORS;
		private int[] TRIANGLES;
		private int[] TYPE; // 3D array? 0=empty , 0 = grass, 1 = dirt?

		
		// grid: no boundary check, put 1x1x1 borders?
		
		public Chunk(int x, int z, int size)
		{
			this.X = x;
			this.Z = z;
			this.SIZE = size; // size is fixed..no need variable..
			// fill grid here?
			this.HEIGHTS = new int[size*size];
			this.TYPE = new int[size*size*size];
			
			for (int i=0;i<size;i++)
			{
				for (int j=0;j<size;j++)
				{
					
					float p = Mathf.PerlinNoise(i*0.1f,j*0.1f)*8;
					
					this.HEIGHTS[i*size+j] = (int)p;
					
					for (int k=0;k<size;k++)
					{
						//this.TYPE[i + size * (j + size * k)] = 0;
						//if (k>=p
						this.TYPE[i + size * (j + size * k)] = (int)(Mathf.PerlinNoise(i*0.1f,j*0.1f)*3);
					}
				}
			}
		}
		
		public void randomize()
		{
			
			//Debug.Log(this.VERTICES.Length);
			for (int i=0;i<this.SIZE;i++)
			{
				for (int j=0;j<this.SIZE;j++)
				{
					//this.HEIGHTS[i*this.SIZE+j] = (int)(Mathf.PerlinNoise(Random.value*i*0.12f,Random.value*j*0.12f)*8);
//					if (j==4 && i==4)
//					{
//						this.HEIGHTS[i*this.SIZE+j] = 7;
//					}else{
//						this.HEIGHTS[i*this.SIZE+j] = 0;
//					}
					
					this.HEIGHTS[i*this.SIZE+j] = (int)(Mathf.PerlinNoise(Random.value*i*0.12f,Random.value*j*0.12f)*8);
					
					for (int k=0;k<this.SIZE;k++)
					{
						this.TYPE[i + this.SIZE * (j + this.SIZE * k)] = Mathf.RoundToInt(Random.value*3);
						//this.TYPE[i + this.SIZE * (k + this.SIZE * j)] = Mathf.RoundToInt(Random.value*2);
					}
					
				}
			}
		}

		public void randomize2()
		{
			
			//Debug.Log(this.VERTICES.Length);
			for (int i=0;i<this.SIZE;i++)
			{
				for (int j=0;j<this.SIZE;j++)
				{
					//this.HEIGHTS[i*this.SIZE+j] = (int)(Mathf.PerlinNoise(Random.value*i*0.12f,Random.value*j*0.12f)*8);
					if (j==4 && i==4)
					{
						this.HEIGHTS[i*this.SIZE+j] = 7;
					}else{
						this.HEIGHTS[i*this.SIZE+j] = 0;
					}
					
//					this.HEIGHTS[i*this.SIZE+j] = (int)(Mathf.PerlinNoise(Random.value*i*0.12f,Random.value*j*0.12f)*8);
					
					for (int k=0;k<this.SIZE;k++)
					{
						this.TYPE[i + this.SIZE * (j + this.SIZE * k)] = Mathf.RoundToInt(Random.value*3);
						//this.TYPE[i + this.SIZE * (k + this.SIZE * j)] = Mathf.RoundToInt(Random.value*2);
					}
					
				}
			}
		}

		
		// check if can move there
		public int canMove(Vector3 s,Vector3 t)
		{
			//Debug.Log("targetX:"+t.x);
			t = t/2*2;
			// getheight from that pos
			if (t.x>-1 && t.x<=this.SIZE-1 && t.z>-1 && t.z<=this.SIZE-1)
			{
				int hs = this.HEIGHTS[(int)(s.x*this.SIZE+s.z)];
				int ht = this.HEIGHTS[(int)(t.x*this.SIZE+t.z)];
				
				int dif = Mathf.Abs(hs-ht);
				//Debug.Log(Mathf.Abs(this.HEIGHTS[this.X*this.SIZE+this.Z]-h));
				//Debug.Log(dif);
				return (dif<2) ? (hs-ht) : -999;
			}else{
				//Debug.Log(t.x>-1);
			}
			return -999;
		}
		
		public int getType(Vector3 t)
		{
			return this.TYPE[(int)(t.x + this.SIZE * (t.z + this.SIZE * t.y))];
		}
		
		public void setType(Vector3 t, int type)
		{
			this.TYPE[(int)(t.x + this.SIZE * (t.z + this.SIZE * t.y))] = type;
		}
		
		public void setHeight(Vector3 t,int change)
		{
			this.HEIGHTS[(int)(t.x*this.SIZE+t.z)]+=change;
		}
		
		
		public int getHeight(Vector3 p)
		{
			p = new Vector3(Mathf.Clamp(p.x,0,this.SIZE),0,Mathf.Clamp(p.z,0,this.SIZE));
			return this.HEIGHTS[(int)(p.x)*this.SIZE+(int)p.z];
		}
		
		public int getHeight(int x,int z)
		{
			x = Mathf.Clamp(x,0,this.SIZE-1);
			z = Mathf.Clamp(z,0,this.SIZE-1);
			return this.HEIGHTS[x*this.SIZE+z];
		}		
		
		public int destroyBlock(Vector3 t)
		{
			int wasType = getType(t);
			
			//Debug.Log("dest:"+(int)t.x+", "+(int)t.y+", "+(int)t.z);
			
			// move blocks down
			//Debug.Log((int)t.y);
			for (int i=(int)t.y;i<getHeight(t);i++)
			{
			//	Debug.Log("orig:"+ getType(new Vector3(t.x, i, t.z)) +" new:"+getType(new Vector3(t.x, i+1, t.z)));
			//Debug.Log("set:"+i+" get:"+(i+1));
				setType(new Vector3(t.x, i, t.z), getType(new Vector3(t.x, i+1, t.z)) );
			}

			setHeight(t,-1);
			

				
			// getheight from that pos
			//if (t.x>=0 && t.x<=this.SIZE-1 && t.z>=0 && t.z<=this.SIZE-1)
			//{
			
			// loop above items (or allow making holes?)
			
			// drop above blocks downward
				rebuildVertices();
			
			
				//int ht = this.HEIGHTS[(int)(t.x*this.SIZE+t.z)];
				//int dif = Mathf.Abs(hs-ht);
				//Debug.Log(Mathf.Abs(this.HEIGHTS[this.X*this.SIZE+this.Z]-h));
				return wasType;
		}


		
		// check if neighbours block view
		private Color getLightAmount(int x,int y,int z)
		{
			
			// check bounds
			int h0 = y; //getHeight(x,z);
			int h1 = getHeight(x,z+1);
			int h2 = getHeight(x+1,z);
			int h3 = getHeight(x,z-1);
			int h4 = getHeight(x-1,z);
			
			float h = 1.0f;
			
			if (h0<h1) h*=1.0f-(Mathf.Clamp( h1-h0,0.0f,4.0f )/4*0.50f);
			if (h0<h2) h*=1.0f-(Mathf.Clamp( h2-h0,0.0f,4.0f )/4*0.50f);
			if (h0<h3) h*=1.0f-(Mathf.Clamp( h3-h0,0.0f,4.0f )/4*0.50f);
			if (h0<h4) h*=1.0f-(Mathf.Clamp( h4-h0,0.0f,4.0f )/4*0.50f);		
			
			//Debug.Log(1.0f-(float)(h1-h0)/this.SIZE);
			//Debug.Log(h);
			
			//if (h0<h1) h*=0.80f;
//			if (h0<h2) h*=0.80f;
//			if (h0<h3) h*=0.80f;
//			if (h0<h4) h*=0.80f;
			
			//float h = ((h0-h1)+(h0-h2)+(h0-h3)+(h0-h4))/8.0f+0.75f;
			
			//Debug.Log("h0:"+h0+" h1:"+h1+" h2:"+h2+" h3:"+h3+" h4:"+h4+ "=h:"+h);
			
			return new Color(h,h,h,1);
		}
		
		
		// rebuild whole mesh
		public void rebuildVertices()
		{
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<Vector2> uvs = new List<Vector2>();
			List<Color> colors = new List<Color>();
			
			int vertexIndex = 0;
			
			//Vector3[] vertices = new Vector3[this.SIZE*this.SIZE];
			// precalc all possible solutions? (based on height or neighbour heights?)
			
			for (int x=0;x<this.SIZE;x++)
			{
				for (int z=0;z<this.SIZE;z++)
				{
					// case by height? later optimize..
					
					int y = this.HEIGHTS[x*this.SIZE+z];
					
					//if (height == 0)
					//{
					
							// top plane
							vertexIndex = vertices.Count;
						
							vertices.Add(new Vector3(x, y, z));
							vertices.Add(new Vector3(x, y, z+1));
							vertices.Add(new Vector3(x+1, y, z+1));
							vertices.Add(new Vector3(x+1, y, z));
						
							triangles.Add(vertexIndex);
							triangles.Add(vertexIndex+1);
							triangles.Add(vertexIndex+2);
						
							triangles.Add(vertexIndex+2);
							triangles.Add(vertexIndex+3);
							triangles.Add(vertexIndex);
							
							Color l = getLightAmount(x,y,z);
							colors.Add(l);
							colors.Add(l);
							colors.Add(l);
							colors.Add(l);

							
							// using atlas						
//							uvs.Add(new Vector2 (0, 0));
//							uvs.Add(new Vector2 (0, 1));
//							uvs.Add(new Vector2 (1, 1));
//							uvs.Add(new Vector2 (1, 0));

							// top

							int t = this.TYPE[x + this.SIZE * (z + this.SIZE * y)];
							uvs.Add((new Vector2 (0, 0)+atlas[t])*0.0625f);
							uvs.Add((new Vector2 (0, 1)+atlas[t])*0.0625f);
							uvs.Add((new Vector2 (1, 1)+atlas[t])*0.0625f);
							uvs.Add((new Vector2 (1, 0)+atlas[t])*0.0625f);
							
//							uvs.Add(new Vector2 (t*0.0625f, t*0.0625f));
//							uvs.Add(new Vector2 (t*0.0625f, (t+1)*0.0625f));
//							uvs.Add(new Vector2 ((t+1)*0.0625f, (t+1)*0.0625f));
//							uvs.Add(new Vector2 ((t+1)*0.0625f, t*0.0625f));
							
							//
//							left(x) = (x - 1) % M
//							right(x) = (x + 1) % M
//							above(x) = (x - M) % (M * N)
//							below(x) = (x + M) % (M * N)
							
							//side planes downward, check neighbours, todo optimize..
							// check north
							if (z<this.SIZE-1)
							{
								int hdif = y-this.HEIGHTS[x*this.SIZE+(z+1)];
								if (hdif>0)
								{
									for (int yy=0;yy<hdif;yy++)
									{
										// add north
										vertexIndex = vertices.Count;
										vertices.Add(new Vector3(x, y-1-yy, z+1));
										vertices.Add(new Vector3(x+1, y-1-yy, z+1));
										vertices.Add(new Vector3(x+1, y-yy, z+1));
										vertices.Add(new Vector3(x, y-yy, z+1));
										triangles.Add(vertexIndex);
										triangles.Add(vertexIndex+1);
										triangles.Add(vertexIndex+2);
										triangles.Add(vertexIndex+2);
										triangles.Add(vertexIndex+3);
										triangles.Add(vertexIndex);

										l = getLightAmount(x,y-yy,z)*0.8f; // side modifier, less light TODO: based on light dir
										colors.Add(l);
										colors.Add(l);
										colors.Add(l);
										colors.Add(l);

										
										//~ uvs.Add(new Vector2 (0, 0));
										//~ uvs.Add(new Vector2 (0, 1));
										//~ uvs.Add(new Vector2 (1, 1));
										//~ uvs.Add(new Vector2 (1, 0));
										
										t = this.TYPE[x + this.SIZE * ((z) + this.SIZE * (y-yy))];
										uvs.Add((new Vector2 (0, 0)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (0, 1)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (1, 1)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (1, 0)+atlas[t])*0.0625f);
									}
								}
							}
							
							if (z>0)
							{
								int hdif = y-this.HEIGHTS[x*this.SIZE+(z-1)];
								if (hdif>0)
								{
									for (int yy=0;yy<hdif;yy++)
									{
										// add south
										vertexIndex = vertices.Count;
										vertices.Add(new Vector3(x, y-1-yy, z));
										vertices.Add(new Vector3(x, y-yy, z));
										vertices.Add(new Vector3(x+1, y-yy, z));
										vertices.Add(new Vector3(x+1, y-1-yy , z));
										triangles.Add(vertexIndex);
										triangles.Add(vertexIndex+1);
										triangles.Add(vertexIndex+2);
										triangles.Add(vertexIndex+2);
										triangles.Add(vertexIndex+3);
										triangles.Add(vertexIndex);
										
										l = getLightAmount(x,y-yy,z)*0.8f; // side modifier, less light TODO: based on light dir
										colors.Add(l);
										colors.Add(l);
										colors.Add(l);
										colors.Add(l);

										
										//~ uvs.Add(new Vector2 (0, 0));
										//~ uvs.Add(new Vector2 (0, 1));
										//~ uvs.Add(new Vector2 (1, 1));
										//~ uvs.Add(new Vector2 (1, 0));										
										t = this.TYPE[x + this.SIZE * ((z) + this.SIZE * (y-yy))];
										uvs.Add((new Vector2 (0, 0)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (0, 1)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (1, 1)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (1, 0)+atlas[t])*0.0625f);
									}
								}
							}
							
							if (x<this.SIZE-1)
							{
								int hdif = y-this.HEIGHTS[(x+1)*this.SIZE+(z)];
								if (hdif>0)
								{
									for (int yy=0;yy<hdif;yy++)
									{
										// add east
										vertexIndex = vertices.Count;
										vertices.Add(new Vector3(x+1, y-1-yy, z));
										vertices.Add(new Vector3(x+1, y-yy, z));
										vertices.Add(new Vector3(x+1, y-yy, z+1));
										vertices.Add(new Vector3(x+1, y-1-yy, z+1));
										triangles.Add(vertexIndex);
										triangles.Add(vertexIndex+1);
										triangles.Add(vertexIndex+2);
										triangles.Add(vertexIndex+2);
										triangles.Add(vertexIndex+3);
										triangles.Add(vertexIndex);

										l = getLightAmount(x,y-yy,z)*0.8f; // side modifier, less light TODO: based on light dir
										colors.Add(l);
										colors.Add(l);
										colors.Add(l);
										colors.Add(l);


										
/* 										uvs.Add(new Vector2 (0, 0));
 * 										uvs.Add(new Vector2 (0, 1));
 * 										uvs.Add(new Vector2 (1, 1));
 * 										uvs.Add(new Vector2 (1, 0));										
 */
										t = this.TYPE[(x) + this.SIZE * (z + this.SIZE * (y-yy))];
										uvs.Add((new Vector2 (0, 0)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (0, 1)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (1, 1)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (1, 0)+atlas[t])*0.0625f);
									}
								}
							}
							
							if (x>0)
							{
								int hdif = y-this.HEIGHTS[(x-1)*this.SIZE+(z)];
								if (hdif>0)
								{
									for (int yy=0;yy<hdif;yy++)
									{
										// add west
										vertexIndex = vertices.Count;
										vertices.Add(new Vector3(x, y-1-yy, z+1));
										vertices.Add(new Vector3(x, y-yy, z+1));
										vertices.Add(new Vector3(x, y-yy, z));
										vertices.Add(new Vector3(x, y-1-yy , z));
										triangles.Add(vertexIndex);
										triangles.Add(vertexIndex+1);
										triangles.Add(vertexIndex+2);
										triangles.Add(vertexIndex+2);
										triangles.Add(vertexIndex+3);
										triangles.Add(vertexIndex);
										
										l = getLightAmount(x,y-yy,z)*0.8f; // side modifier, less light TODO: based on light dir
										colors.Add(l);
										colors.Add(l);
										colors.Add(l);
										colors.Add(l);

										
										//~ uvs.Add(new Vector2 (0, 0));
										//~ uvs.Add(new Vector2 (0, 1));
										//~ uvs.Add(new Vector2 (1, 1));
										//~ uvs.Add(new Vector2 (1, 0));	
										
										t = this.TYPE[(x) + this.SIZE * (z + this.SIZE * (y-yy))];
										uvs.Add((new Vector2 (0, 0)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (0, 1)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (1, 1)+atlas[t])*0.0625f);
										uvs.Add((new Vector2 (1, 0)+atlas[t])*0.0625f);
									}
								}
							}
							
							
							
							
							
						/*
					}else{ // we have heights
						
						// todo, check neighbour height.. if same or higher, no need to add that side
						
						for (int y=0;y<height;y++)
						{
							// build block sides (top is added after this!
							
							
							// add north
							vertexIndex = vertices.Count;
							vertices.Add(new Vector3(x, y, z+1));
							vertices.Add(new Vector3(x+1, y, z+1));
							vertices.Add(new Vector3(x+1, y+1, z+1));
							vertices.Add(new Vector3(x, y+1 , z+1));
							triangles.Add(vertexIndex);
							triangles.Add(vertexIndex+1);
							triangles.Add(vertexIndex+2);
							triangles.Add(vertexIndex+2);
							triangles.Add(vertexIndex+3);
							triangles.Add(vertexIndex);					
							uvs.Add(new Vector2 (0, 0));
							uvs.Add(new Vector2 (0, 1));
							uvs.Add(new Vector2 (1, 1));
							uvs.Add(new Vector2 (1, 0));
							
							// add east
							vertexIndex = vertices.Count;
							vertices.Add(new Vector3(x+1, y, z));
							vertices.Add(new Vector3(x+1, y + 1, z));
							vertices.Add(new Vector3(x+1, y + 1, z+1));
							vertices.Add(new Vector3(x+1, y , z+1));
							triangles.Add(vertexIndex);
							triangles.Add(vertexIndex+1);
							triangles.Add(vertexIndex+2);
							triangles.Add(vertexIndex+2);
							triangles.Add(vertexIndex+3);
							triangles.Add(vertexIndex);					
							uvs.Add(new Vector2 (0, 0));
							uvs.Add(new Vector2 (0, 1));
							uvs.Add(new Vector2 (1, 1));
							uvs.Add(new Vector2 (1, 0));
							
							
						}*/
						
						// cap top
						
						
					//}
					//Debug.Log("#"+(i*this.SIZE+j));
				}
			}			
			
			this.VERTICES = vertices.ToArray();
			this.UVS = uvs.ToArray();
			this.TRIANGLES = triangles.ToArray();
			this.COLORS = colors.ToArray();
			
			// create new vertices table
			//return vertices.ToArray();
			
		}
		
		
		public Vector3[] getVertices()
		{
			return this.VERTICES;
		}	
		
		public Vector2[] getUVs()
		{
			return this.UVS;
		}	
		
		public int[] getTriangles()
		{
			return this.TRIANGLES;
		}			
		
		public Color[] getColors()
		{
			return this.COLORS;
		}	

	}
}