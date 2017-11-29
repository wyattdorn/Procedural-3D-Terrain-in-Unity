using UnityEngine;
using System.Collections;

public class NeighborTerrainScript : MonoBehaviour {

	[Range(0.1f, 1f)]
	public float rough = 0.5f;

	System.Random rng;
	float[,] heights, tempHeights;
	Terrain myTerrain;
	TerrainData td;
	int hash, size_x, fractalSize, max;
	[Range(0, 25)]

	float[, ,] splatmapData;

	public Rigidbody FPSCam;

	// What runs every time we press play
	void Start () {
		initializeAll ();
		myTerrain.terrainData.SetDetailResolution (1000, 1000);
		generateTerrain ();
		FPSCam.position = (new Vector3 (500, heights[500,500]+100f, 500));
	}

	void Update () {
		if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp (KeyCode.Mouse0)) {
			Start ();
		}
	}

	void generateNeighbor(ref float[,] myHeights, char direction){
		//first we check which neighbor spawned our current terrain
		switch(direction){
			case 'N'||'n': 
				
				break;
			case 'E'||'e':
			case 'W'||'w':
			case 'S'||'s':
		}

		
	}

	void generateTerrain(){
		splatmapData = new float[myTerrain.terrainData.alphamapResolution, myTerrain.terrainData.alphamapResolution, myTerrain.terrainData.alphamapLayers];

		NeighborFractals fractalCode = gameObject.AddComponent <Fractals>();

		fractalCode.generateFractal (ref heights, size_x, 3.3f);

		normalizeHeights();



		matchNeighbor ();

		Debug.Log (heights[0,0]+", "+heights[100,100]);

		myTerrain.terrainData.SetHeights (0, 0, heights);
		myTerrain.terrainData.SetAlphamaps(0, 0, splatmapData);

		//make sure to get rid of heights[,] and only use tempHeights[,]
		//also, might want to change the roughness sent to fractal method
	}

	void initializeAll(){
		size_x = 1025;
		myTerrain = GetComponent<Terrain>();
		td = myTerrain.terrainData;

		heights = new float[size_x,size_x];
		tempHeights = new float[size_x,size_x];
	}
		
	bool isValidPoint(int x, int z){
		if (x >= 0 && x < size_x) {
			if (z >= 0 && z < size_x) {
				return true;
			}
		}
		return false;
	}

	void matchNeighbor(){
		Debug.Log ("match!");
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				heights [x, y] = 1.0f;
			}
		}
	}

	void normalizeHeights(){
		float lowest, highest;
		highest = lowest = 0.5f;
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				if (heights [x, y] <= lowest) {
					lowest = heights [x, y];
				}
				else if(heights [x, y] >= highest) {
					highest = heights [x, y];
				}
			}
		}

		highest = highest - lowest;

		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				heights [x, y] -= lowest;
				heights [x, y] /= highest;
				heights [x, y] *= heights [x, y];
			}
		}

	}
		
}
