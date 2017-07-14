using UnityEngine;
using System.Collections;
using System.Linq; // used for Sum of array

public class terrains2 : MonoBehaviour {

	public class Terrel{//Terrel (Terrain element) class
		char terrainType;
		// For now 'm' will represent mountains, '-' will represent blanks
		float height;
		bool isWater;

		public void init(){
			terrainType = '-';
			height = 0.0f;
			isWater = false;
		}

		public void setTerrain(char c)
		{
			if (c == 'c' || c == '-')
				terrainType = c;
		}

		public char getTerrain()
		{
			return terrainType;
		}

	}


	//////////////////////////////////// Universal Variables	/////////////////////////////////////
	Terrel[,] myTerrels;
	int size_x;
	int numMounts;

	Terrain terrain;
	TerrainData terrainData;

	System.Random pseudoRandom;
	string seed;


	///////////////////////////////////////////////////////Start Functions//////////////////////////////////
	void Start () {
		seed = Time.time.ToString ();
		//terrain = GetComponent<Terrain>();
		//terrainData = terrain.terrainData;
		//terrainData.heightmapResolution = size_x;

		size_x = 100;
		myTerrels = new Terrel[size_x, size_x];

		numMounts = 4;

		generateTerrain ();
	}

	void generateTerrain(){
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				//myTerrels[x,y] = new Terrel ();
				//myTerrels [x, y].init ();
			}
		}
		generateMountains ();
	}

	void generateMountains()
	{
		pseudoRandom = new System.Random (seed.GetHashCode ());
		int temp;
		int sign;

		//If the number of mountains is set to -1, we randomize the number of mountains
		if (numMounts < 0) {
			numMounts = pseudoRandom.Next (3, 7);
		} else if (numMounts == 0) {
			return;
		}
		int[,] mountains = new int[numMounts, 2];

		//Set the first mountain to a random point on the interior of the map
		mountains[0,0] = pseudoRandom.Next (size_x / 10, size_x - (size_x / 10));
		mountains[0,1] = pseudoRandom.Next (size_x / 10, size_x - (size_x / 10));

		for (int i = 1; i < numMounts; i++) {
			for (int j = 0; j < 2; j++) {
				//Set initial location for current mountain to location of previous mountain
				mountains [i, j] = mountains [i - 1, j];
				//prodice positive or negative result

				//do {
					sign = pseudoRandom.Next (0, 10);
					if (sign % 2 == 0) {
						sign = 1;
					} else {
						sign = -1;
					}
					temp = (sign * pseudoRandom.Next (15, 30));
				//} while(mountains [i, j]  + temp <= size_x);
				if (mountains [i, j] + temp <= size_x) {
					mountains [i, j] += temp;// sign * pseudoRandom.Next (15, 30);// temp;
				} else {
					mountains [i, j] = size_x;
				}
					
			}
		}

		Debug.Log ("Number of mountains:" + numMounts);

		for (int x = 0; x < numMounts; x++) {
			Debug.Log (mountains [x, 0] + " , " + mountains [x, 1]);
		}
	}

}
