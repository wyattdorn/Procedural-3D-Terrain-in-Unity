using UnityEngine;
using System.Collections;
using System.Linq;

public class terrain : MonoBehaviour {

	public string seed;
	[Range(0.1f, 1f)]
	public float roughness = 0.5f;

	System.Random rng;
	float[,] heightmap;
	TerrainData td;
	int hash;
	int max;

	int size_x = 1025; //Map will always be a square, size_z represents both lenght and width

	Terrain bit;

	System.Random pseudoRandom;
		
	void Start () {
		max = size_x - 1;
		heightmap = new float[size_x, size_x];
		bit = GetComponent<Terrain>();
		bit.terrainData.heightmapResolution = size_x;
		generateHeights ();
	}

	void generateHeights() {
		rng = new System.Random();
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				heightmap[x,y] = 0.5f;
			}
		}
		Divide (size_x);
		//fractals();
		//heightmap[500,300] = 1.0f;
		bit.terrainData.SetHeights (0, 0, heightmap);
	}

	void fractals() {
		int half = 2;
		int center = size_x / 2;
		heightmap [size_x / half, size_x / half] = 1.0f;
		for (int x = 1; x < 3; x++) {
			heightmap [center+center/half, center+center/half] = 1.0f;
			heightmap [center+center/half, center-center/half] = 1.0f;
			heightmap [center-center/half, center+center/half] = 1.0f;
			heightmap [center-center/half, center-center/half] = 1.0f;
			half *= 2;
		}
	}

	void Divide(int size){
		//Debug.Log("Size: " + size);
		int half = size / 2;
		if (half < 1) {
			return;
		}

		for (int y = half; y < max; y += size) {
			for (int x = half; x < max; x += size) {
				Square(x, y, half, ((float)rng.NextDouble() - 0.5f)/(max/size));
			}
		}

		//Diamond(half, 0, half, ((float)rng.NextDouble() - 0.5f)/(max/size));

		for (int y = 0; y <= max; y += half) {
			for (int x = (y + half) % size; x <= max; x += size) {
				Debug.Log ("My x: " + x);
				Diamond(x, y, half, ((float)rng.NextDouble() - 0.5f)/(max/size));
			}
		}
		Divide(half);
	}

	void Diamond(int x, int y, int size, float offset){
		//Debug.Log ("(" + x + "," + y + ")");
		float ave = Average(new float[4]{
			Get(x, y - size),
			Get(x + size, y),
			Get(x, y + size),
			Get(x - size, y)});
		
		//float ave = 2.0f;

		heightmap[x, y] = ave + offset * roughness;
	}

	void Square(int x, int y, int size, float offset){
		float ave = Average(new float[4]{
			Get (x - size, y - size),
			Get (x + size, y - size),
			Get (x + size, y + size),
			Get (x - size, y + size)});

		heightmap[x, y] = ave + offset * roughness;
	}

	float Get(int x, int y){
		if (x < 0 || x > max || y < 0 || y > max) {
			return 0;
		}
		return heightmap[x, y];
	}

	float Average(float[] values){
		float total = 0;
		foreach (float f in values) {
			total += f;
		}
		return total / values.Length;
	}
}
