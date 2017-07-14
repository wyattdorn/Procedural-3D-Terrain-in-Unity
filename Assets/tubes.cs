using UnityEngine;
using System.Collections;

public class tubes : MonoBehaviour {

	public string seed;
	[Range(0.1f, 1f)]
	public float roughness = 0.5f;

	System.Random rng;
	float[,] heightmap;
	float[,] heights;
	TerrainData td;
	int hash;
	int fractalSize;
	int max;

	void Awake(){
		td = GetComponent<Terrain> ().terrainData;
		td.heightmapResolution = 2048;
		fractalSize = td.heightmapResolution;
		td.heightmapResolution = 1024;
		max = fractalSize - 1;
	}

	void Start(){
		heights = new float[1024,1024];
		heightmap = new float[fractalSize,fractalSize];
		for (int x = 0; x < fractalSize; x++) {
			for (int y = 0; y < fractalSize; y++) {
					heightmap [x, y] = 0.5f;
			}
		}
		GenerateTerrain ();
	}

	public void GenerateTerrain(){
		if (seed != "") {
			hash = seed.GetHashCode ();
			rng = new System.Random (hash);
		} else {
			rng = new System.Random();
		}
		heightmap [(int)(fractalSize/2), (int)(fractalSize/2)] = 0.6f;
		Divide (max);
		for (int x = 0; x < 1024; x++) {
			for (int y = 0; y < 1024; y++) {
				heights [x, y] = heightmap [x + 512, y + 512];
			}
		}
		td.SetHeights (0, 0, heights);
	}

	void Divide(int fractalSize){
		int half = fractalSize / 2;
		if (half < 1) {
			return;
		}
		for (int y = half; y < max; y += fractalSize) {
			for (int x = half; x < max; x += fractalSize) {
				Square(x, y, half, ((float)rng.NextDouble() - 0.5f)/(max/fractalSize));
			}
		}
		for (int y = 0; y <= max; y += half) {
			for (int x = (y + half) % fractalSize; x <= max; x += fractalSize) {
				Diamond(x, y, half, ((float)rng.NextDouble() - 0.5f)/(max/fractalSize));
			}
		}
		Divide(half);
	}

	void Diamond(int x, int y, int fractalSize, float offset){
		if (heightmap [x, y] != 0.5) {
			Debug.Log ("GOT ONE!");
			return;
		}
		float ave = Average(new float[4]{
			Get(x, y - fractalSize),
			Get(x + fractalSize, y),
			Get(x, y + fractalSize),
			Get(x - fractalSize, y)});

		heightmap[x, y] = ave + offset * roughness;
	}

	void Square(int x, int y, int fractalSize, float offset){
		if (heightmap [x, y] != 0.5) {
			Debug.Log ("GOT ONE!");
			return;
		}
		float ave = Average(new float[4]{
			Get (x - fractalSize, y - fractalSize),
			Get (x + fractalSize, y - fractalSize),
			Get (x + fractalSize, y + fractalSize),
			Get (x - fractalSize, y + fractalSize)});

		heightmap[x, y] = ave + offset * roughness;
	}

	/*void OnGUI(){
		if (GUI.Button (new Rect (0, 0, 120, 40), "Generate Terrain")) {
			GenerateTerrain();
		}
	}*/

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