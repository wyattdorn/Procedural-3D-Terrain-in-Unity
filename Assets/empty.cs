using UnityEngine;
using System.Collections;
using System.Linq; // used for Sum of array


public class empty : MonoBehaviour {

	int size_x = 1025; //Map will always be a square, size_z represents both lenght and width

	int numMountains = 15;

	int allocatedTerrels; //Counter used to determine when the map is completely full;

	float[,] heights;
	float[,] tempHeights;

	Terrain bit;
	TerrainData terrainData;

	System.Random pseudoRandom;

	string seed;

	char[,] region; //a 2D array of chars, the sam size as our terrain, to represent the various terrain regions in the terrain

	Terrel[,] myTerrels;

	//Map the size of our terrain that records the time a plant at each location is ignited
	float[,] burnMap;

	public class Terrel{//Terrel (Terrain element) class
		//public char terrainType;
		public string terrainType;
		//Height from 0.0f-1.0f
		public float height;
		//bool isWater;
		float water_ratio, mountain_ratio, forest_ratio, grassland_ratio, desert_ratio;
		float[] terrainRatios;
		string[] terrainTypes = new string[5];

		public void init(){
			this.terrainType = "water";
			this.height = 0.0f;
			//isWater = false;
			this.terrainTypes[0] = "water";
			this.terrainTypes[1] = "mountain";
			this.terrainTypes[2] = "forest";
			this.terrainTypes[3] = "grassland";
			this.terrainTypes[4] = "desert";
		}

		public bool isWater(){
			if (this.terrainType == "water")
				return true;
			return false;
		}

		public void setTerrain(string t){
			if (t == "mountain" || t == "mountains" || t == "Mountain" || t == "Mountains") {
				this.terrainType = "mountain";
				this.mountain_ratio = 1f;
				this.forest_ratio = 0f;
				this.grassland_ratio = 0f;
			}
			else if (t == "forest" || t == "forests" || t == "Forest" || t == "Forests") {
				this.terrainType = "forest";
				this.mountain_ratio = 0f;
				this.forest_ratio = 1f;
				this.grassland_ratio = 0f;
			}
			else if (t == "grassland" || t == "grasslands" || t == "Grassland" || t == "Grasslands") {
				this.terrainType = "grassland";
				this.mountain_ratio = 0f;
				this.forest_ratio = 0f;
				this.grassland_ratio = 1f;
			}
			else if (t == "desert" || t == "deserts" || t == "Desert" || t == "Deserts") {
				this.terrainType = "desert";
				this.mountain_ratio = 0f;
				this.forest_ratio = 0f;
				this.desert_ratio = 1f;
			}
			normalizeTerrainTypes ();
		}

		void normalizeTerrainTypes(){
			//do stuff
			float max = 0, largest = 0;
			this.terrainRatios = new float[5];
			this.terrainRatios [0] = this.water_ratio;
			this.terrainRatios [1] = this.mountain_ratio;
			this.terrainRatios [2] = this.forest_ratio;
			this.terrainRatios [3] = this.grassland_ratio;
			this.terrainRatios [4] = this.desert_ratio;

			for (int x = 0; x < terrainRatios.Length; x++) {
				max += this.terrainRatios [x];
			}
			/*
			for (int x = 0; x < terrainRatios.Length; x++) {
				this.terrainRatios [x] = (max - this.terrainRatios [x]) / (max);
				if (this.terrainRatios [x] > largest) {
					largest = this.terrainRatios [x];
					this.terrainType = this.terrainTypes[x];
				}
			}
			*/
			this.mountain_ratio = this.terrainRatios [0];
			this.forest_ratio = this.terrainRatios [1];
			this.desert_ratio = this.terrainRatios [2];

		}
	}

	public class Plant{
		//Plant types: Tree, grass, bush
		string plant_type;
		//Model type, model height, model width
		//cast model type to int when using
		float[] model_info;
		//Burn time: How long the plant Burns
		//Ignite time: How long it takes to ingite plant.
		float burn_time, ignite_time;
		//The radius at which it is a threat to other plants;
		float threat_radius;
		//Neighbors determined by those this plant threatens.
		//Potentilly set length of array to pi*r^2
		int[,] neighbors;
		//Is it on fire, has it already burned?
		bool isBurning, hasBurned;
	}

	// Use this for initialization
	void Start () {

		allocatedTerrels = 0;//Map starts off empty
		bit = GetComponent<Terrain>();
		terrainData = bit.terrainData;
		bit.terrainData.alphamapResolution = size_x;
		bit.terrainData.heightmapResolution = size_x;
		//Debug.Log (bit.terrainData.alphamapWidth + " shoud be: " + bit.terrainData.heightmapWidth);

		//Arrays to store map height data temporarily during calculation
		heights = new float[size_x,size_x];
		tempHeights = new float[size_x,size_x];

		seed =  Time.time.ToString ();

		generateRegions ();
		//bit.terrainData.SetHeights (0, 0, heights);
		textureTest ();
		//generateHeights ();
		clearFoliage ();
		//generateFoliage ();


		bit.terrainData.SetHeights (0, 0, heights);
		//Debug.Log (bit.terrainData.alphamapWidth + " should be: " + bit.terrainData.heightmapWidth);
		//Debug.Log ((float)((float)allocatedTerrels/(float)(size_x*size_x)));
	}

	void setMapHeights(){
		setMapHeights ();
		prototypeMountain ();
		for (int y = 0; y < size_x; y++) {
			for (int x = 0; x < size_x; x++) {
				if (isValidPoint(x,y)){// ((int)(x * 1.025f), (int)(y * 1.025f))) {
					heights[x,y] = myTerrels [x, y].height;;// [(int)((float)x * 1.025), (int)((float)y * 1.025)] = myTerrels [x, y].height;
				}
				//heights [(int)((float)x), (int)((float)y)] = myTerrels [x, y].height;
			}
		}
	}

	void generateRegions(){
		myTerrels = new Terrel[size_x,size_x];
		region = new char[size_x, size_x];
		for (int i = 0; i < size_x; i++) {
			for (int j = 0; j < size_x; j++) {
				myTerrels[i,j] = new Terrel();
				myTerrels [i, j].init ();
			}
		}
		//myTerrels [500, 500].setTerrain ('m');
		generateMountains (numMountains);
		generateTest ();

		//clearFoliage ();
	}

	void generateTest(){
		int[] centerForest = new int[]{600, 400};//Vector2's are nice, but I don't want to have to cast their float elements to int's
		int[] centerGrassland = new int[]{400, 400};
		int[] centerDesert = new int[]{500, 600};
		int i = 0;
		//myTerrels [600, 500].setTerrain ("forest");
		//myTerrels [400, 500].setTerrain ("grassland");
		do {
			for (int x = 0; x < 100; x++) {
				expandTerrain (centerForest [0], centerForest [1], x, "forest");
				expandTerrain (centerGrassland [0], centerGrassland [1], x, "grassland");
				expandTerrain (centerDesert [0], centerDesert [1], x, "desert");
			}
			getPerimiterPoint(ref centerForest [0], ref centerForest [1], 100);
			getPerimiterPoint(ref centerGrassland [0], ref centerGrassland [1], 100);
			getPerimiterPoint(ref centerDesert [0], ref centerDesert [1], 100);
			i++;
		} while((float)((float)allocatedTerrels/(float)(size_x*size_x))<=0.3f);
	}

	int generateMountains(int mounts)
	{
		System.Random myRand = new System.Random();
		//pseudoRandom = new System.Random (seed.GetHashCode ());
		int temp;
		int sign;

		int mountainRadius = 30; //Initial radius around mountain peaks that will be set to "mountain" type

		//If numMountains == -1, randomize number of mountains
		if (mounts < 0) {
			mounts = pseudoRandom.Next (3, 7);
		}
		int[,] mountains = new int[mounts, 2];

		//generate a mountain within the map
		mountains[0,0] = myRand.Next (size_x / 10, size_x - (size_x / 10));
		mountains[0,1] = myRand.Next (size_x / 10, size_x - (size_x / 10));

		//create next mountain 15-30 units from initial mountain

		for (int i = 1; i < mounts; i++) {
			for (int j = 0; j < 2; j++) {
				//Set initial location for current mountain to location of previous mountain
				mountains [i, j] = mountains [i - 1, j];
				//prodice positive or negative result
				sign = myRand.Next (0, 10);
				if (sign % 2 == 0) {
					sign = 1;
				} else {
					sign = -1;
				}
					
				temp = (sign * myRand.Next (15, 30));

				if (mountains [i, j] + temp <= size_x) {
					mountains [i, j] += temp;// sign * pseudoRandom.Next (15, 30);// temp;
				} else {
					mountains [i, j] = size_x;
				}
			}
		}

		for (int x = 0; x < mounts; x++) {
			//Debug.Log (mountains [x, 0] +" , " + mountains [x, 1]);
			myTerrels [mountains [x, 0], mountains [x, 1]].terrainType="peak";
			for (int m = 0; m < mountainRadius; m++) {
				expandTerrain (mountains [x, 0], mountains [x, 1], m, "mountain");
			}
		}

		return mounts;
	}

	void generateHeights() {
		System.Random newRandom = new System.Random (seed.GetHashCode ());
		for (int y = 0; y < size_x; y++) {
			for (int x = 0; x < size_x; x++) {
				if(newRandom.NextDouble() <=0.1 && myTerrels[x,y].isWater()==false){
					myTerrels [x, y].height = (float)(newRandom.NextDouble()/10);//(float)((x + 1.0f) / (y + 1.0f));//(pseudoRandom.NextDouble());// (0.0f, 1.0f));// < randomFillPercent)? 1: 0;
				}
			}
		}	

		for (int y = 0; y < size_x; y++) {
			for (int x = 0; x < size_x; x++) {
				//if (isValidPoint(x,y)){// ((int)((float)(x) / 1.025f), (int)((float)(y) / 1.025f))) {
				if (myTerrels [x, y].terrainType == "peak") {
					myTerrels [x, y].height = 5.0f;
				}
				heights [x, y] = myTerrels [x , y].height;
				//}
			}
		}
		/*
		for (int y = 1; y < size_x; y++) {
			for (int x = 1; x < size_x; x++) {
				if (myTerrels [x, y].terrainType == "mountain") {
					heights [x , y] = 1.0f;
				}
				else if (myTerrels [x, y].terrainType == "forest") {
					heights [x , y] = 0.5f;
				}
				else if (myTerrels [x, y].terrainType == "desert") {
					heights [x , y] = 0.0f;
				}
				else if (myTerrels [x, y].terrainType == "grassland") {
					heights [x , y] = -0.5f;
				}
			}
		}
		*/
		//smoothTest ();
		for (int c = 0; c < 3; c++) {
			//SmoothMap ();
		}

		smoothForest (2);

		float tallest = 0;
		//let's make some weird mountains
		/*
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				if (tallest < tempHeights [x, y]) {
					tallest = tempHeights [x, y];
				}
				if (heights [x, y] == 1.0f) {
					//Debug.Log ("Mountain");
					for (int z = 1; z < 10; z++) {
						for(int p=1; p<=z; p++){
							heights [x - z, y - p] =Mathf.Pow(0.9f, (float)z);
						}
					}
				}
			}
		}
		*/
	}

	void textureTest(){
		float[, ,] splatmapData = new float[bit.terrainData.alphamapResolution, bit.terrainData.alphamapResolution, terrainData.alphamapLayers];
		for (int y = 0; y < size_x; y++) {
			for (int x = 0; x < size_x; x++) {
				
				float[] splatWeights = new float[terrainData.alphamapLayers];

				if (myTerrels[x, y].terrainType == "peak") {
					
					for(int i = 0; i<terrainData.alphamapLayers; i++){
						splatmapData [x, y, 0] = 1; //splatWeights[1];
						for (int z = 1; z < 10; z++) {
							for (int p = 1; p <= z; p++) {
								splatmapData [x - z, y - p, 1] = 1;
								splatmapData [x - z, y + p, 1] = 1;
								splatmapData [x - z, y, 1] = 1;
								splatmapData [x - 1, y + 1, 3] = 1;
								splatmapData [x - 1, y - 1, 3] = 1;
								splatmapData [x + 1, y + 1, 3] = 1;
								splatmapData [x + 1, y - 1, 3] = 1;
							}
						}
					}
				}
			}
		}
		for (int y = 1; y < size_x-1; y++) {
			for (int x = 1; x < size_x-1; x++) {
				if (myTerrels [x, y].terrainType == "mountain") {
					splatmapData [x, y, 2] = 1;
				} else if (myTerrels [x, y].terrainType == "forest") {
					splatmapData [x, y, 5] = 1;
				} else if (myTerrels [x, y].terrainType == "grassland") {
					splatmapData [x, y, 6] = 1;
				} else if (myTerrels [x, y].terrainType == "desert") {
					splatmapData [x, y, 4] = 1.0f;
				} else {
					splatmapData [x, y, 7] = 1.0f;
				}
			}
		}
			
		bit.terrainData.SetAlphamaps(0, 0, splatmapData);
	}

	void splatMap () {
		// Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
		float[, ,] splatmapData = new float[size_x, size_x, terrainData.alphamapLayers];

		for (int y = 0; y < size_x; y++)
		{
			for (int x = 0; x < size_x; x++)
			{
				// Normalise x/y coordinates to range 0-1 
				float y_01 = (float)y/(float)size_x;
				float x_01 = (float)x/(float)size_x;

				// Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
				float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * size_x),Mathf.RoundToInt(x_01 * size_x) );

				// Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
				Vector3 normal = terrainData.GetInterpolatedNormal(y_01,x_01);

				// Calculate the steepness of the terrain
				float steepness = terrainData.GetSteepness(y_01,x_01);

				// Setup an array to record the mix of texture weights at this point
				float[] splatWeights = new float[terrainData.alphamapLayers];

				// CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

				// Texture[0] has constant influence
				splatWeights[0] = 0.5f;

				// Texture[1] is stronger at lower altitudes
				splatWeights[1] = Mathf.Clamp01((size_x - height));

				// Texture[2] stronger on flatter terrain
				// Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
				// Subtract result from 1.0 to give greater weighting to flat surfaces
				splatWeights[2] = 1.0f - Mathf.Clamp01(steepness*steepness/(size_x/5.0f));

				// Texture[3] increases with height but only on surfaces facing positive Z axis 
				splatWeights[3] = height * Mathf.Clamp01(normal.z);

				// Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
				float z = splatWeights.Sum();

				// Loop through each terrain texture
				for(int i = 0; i<terrainData.alphamapLayers; i++){

					// Normalize so that sum of all texture weights = 1
					splatWeights[i] /= z;

					// Assign this point to the splatmap array
					splatmapData[x, y, i] = splatWeights[i];
				}
			}
		}

		// Finally assign the new splatmap to the terrainData:
		terrainData.SetAlphamaps(0, 0, splatmapData);
	}

	void smoothForest(int itterations) {

		for (int x = 0; x < size_x; x ++) {
			for (int y = 0; y < size_x; y ++) {
				tempHeights [x, y] = heights [x, y];
				//heights [x, y] = tempHeights [x, y];
			}
		}

		for (int i = 0; i < itterations; i++) {

			for (int x = 5; x < size_x - 5; x++) {
				for (int y = 5; y < size_x - 5; y++) {
					//tempHeights [x, y] = 0;
					if (myTerrels [x, y].terrainType == "forest") {
						for (int n = -3; n < 4; n++) {
							for (int m = -3; m < 4; m++) {
								tempHeights [x, y] += (heights [x + n, y + m]) / 9f;
							}
						}
					}
				}
			}

			for (int x = 0; x < size_x; x++) {
				for (int y = 0; y < size_x; y++) {
					heights [x, y] = Mathf.Sin(tempHeights [x, y]);
				}
			}
		}
	}

	void smoothTest() {
		for (int x = 1; x < size_x-1; x ++) {
			for (int y = 1; y < size_x-1; y ++) {
				tempHeights [x, y] = 0;
				for(int n = -1; n<2; n++){
					for(int m = -1; m<2; m++){
						if (myTerrels [x + n, y + m].isWater()) {
							tempHeights [x, y] = -1f;
							m = 2;
							n = 2;
						} else {
							tempHeights [x, y] += heights [x + n, y + m];
						}
					}
				}
				tempHeights[x,y] /= (float)7;
			}
		}
		for (int x = 1; x < size_x - 1; x++) {
			for (int y = 1; y < size_x - 1; y++) {
				heights [x, y] = tempHeights [x, y];
			}
		}
	}

	void SmoothMap() {

		for(int y = 1; y < size_x-1; y++){
			for (int x = 1; x < size_x-1; x++) {

				if (myTerrels [x, y].isWater()==false) {
					for(int n = -1; n<2; n++){
						for(int m = -1; m<2; m++){
							heights [x, y] += myTerrels [x + m, y + n].height;								
						}
					}
				}
				heights[x,y] /= 9.0f;
			}
		}

		for (int y = 1; y < size_x - 1; y++) {
			for (int x = 1; x < size_x - 1; x++) {
				myTerrels [x, y].height = heights [x, y];
			}
		}
		/*
		float tallest = 0;
		float shortest = heights [0, 0];
		float range;
		float multiplyer = 0.5f;
		//Smooth out map and determine tallest snd shortest points
		for (int x = 0; x < size_x; x ++) {
			for (int y = 0; y < size_x; y ++) {
				//tempHeights[x,y] = GetSurroundingWallCount(x,y);
				tempHeights[x,y] = heights[x,y];
				if (tallest < tempHeights [x, y]) {
					tallest = tempHeights [x, y];
				}
				if (shortest > tempHeights [x, y]) {
					shortest = tempHeights [x, y];
				}
			}
		}

		//Subtract shortest from tallest to get the range of values
		range = tallest - shortest;

		//Normalize values of tempMap
		//Make tall points taller, and short points shorter.

		for (int x = 0; x < size_x; x ++) {
			for (int y = 0; y < size_x; y ++) {
				tempHeights [x, y] -= shortest;
				tempHeights [x, y] /= range;
				//Range of values is now 0 - 1
				tempHeights [x, y] += multiplyer;
				tempHeights [x, y] *= tempHeights [x, y];
				//Add 0.5 and square values to increase distance between peaks and valleys
				tempHeights [x, y] /= (1.0f + multiplyer);
				tempHeights [x, y] /= (1.0f + multiplyer);
				//Divide all values by 1.5^2 (the max possible value) to normalize again
			}
		}

		//Repopulate map with values from tempMap
		for (int x = 0; x < size_x; x ++) {
			for (int y = 0; y < size_x; y ++) {
				heights [x, y] = (tempHeights [x, y]);//*(tempHeights[x,y]);
			}
		}
		*/
	}

	float GetSurroundingWallCount(int gridX, int gridY) {
		float wallCount = 0.0f;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (neighbourX >= 0 && neighbourX < terrainData.heightmapWidth && neighbourY >= 0 && neighbourY < terrainData.heightmapHeight) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += heights[neighbourX,neighbourY];
					}
				}
				else {
					//if we want the edges of the map raised:
					//wallCount ++;
				}
			}
		}

		return wallCount;
	}

	//Returns number of points on circle. Returns by reference the coordinates of the circle, centered at (0,0)
	int calcCircle(int radius, ref int[,] ring){
		int count = 0;
		//initial pass gives the coordinates in quadrant 1 only, but circles are symmetric horizontally, and vertically
		for (int y = 0; y < radius+1; y++) {
			for (int x = 0; x < radius+1; x++) {
				if ((int)(Mathf.Sqrt(x * x + y * y)) == radius) {
					ring [count, 0] = x;
					ring [count, 1] = y;
					count++;
				}
			}
		}
			
		//count *= 4;

		for (int c = 0; c < count; c ++) {
			ring [count + c, 0] = -ring [c, 0];
			ring [count + c, 1] = ring [c, 1];
		}
		count *= 2;
		for (int c = 0; c < count; c ++) {
			ring [count + c , 0] = ring [c, 0];
			ring [count + c , 1] = -ring [c, 1];
		}
		count *= 2;
		return count;
	}

	//Set tiles in a circle of "radius" arround (x,z) to "terrainType
	void expandTerrain(int x, int z, int radius, string terrainType){
		int[,] locations = new int[100000, 2];
		int numLocs = calcCircle (radius, ref locations);

		if (numLocs <= 0 || numLocs >= 100000) {
			Debug.Log ("Error: no terrels found in array: " + numLocs);
			return;
		}

		for (int n = 0; n < numLocs; n++) {
			if (x + locations [n, 0] >= 0 && x + locations [n, 0] < size_x && z + locations [n, 1] >= 0 && z + locations [n, 1] <= size_x) {// &&
				if (myTerrels [x + locations [n, 0], z + locations [n, 1]].isWater ()) {
					myTerrels [x + locations [n, 0], z + locations [n, 1]].setTerrain (terrainType);
					allocatedTerrels++;
				}
			}
		}
	}

	void getPerimiterPoint(ref int x, ref int z, int radius){
		int[,] locations = new int[100000, 2];
		int numLocs = calcCircle (radius, ref locations);
		int random;
		int tempX = x, tempZ = z;
		System.Random myRand = new System.Random();
		//myRand = new System.Random ( ((x*z*radius)*(int)Time.time).GetHashCode());

		do {
			random = myRand.Next (0, numLocs);
			x = tempX + locations[random, 0];
			z = tempZ + locations[random, 1];
		} while(!isValidPoint (x, z));
	}

	bool isValidPoint(int x, int z){
		if (x >= 0 && x < size_x) {
			if (z >= 0 && z < size_x) {
				return true;
			}
		}
		return false;
	}

	void clearFoliage(){
		TreeInstance[] newTree;
		newTree = new TreeInstance[0];
		bit.terrainData.treeInstances = newTree	;
	}

	void generateFoliage(){
		clearFoliage ();
		TreeInstance[] newTree;
		newTree = new TreeInstance[100000];
		int numTrees = 0;
		plantTrees (ref numTrees, ref newTree);
		//plantGrass (ref numTrees, ref newTree);
		plantBushes (ref numTrees, ref newTree);
		bit.terrainData.treeInstances = newTree	;
	}

	void plantTrees(ref int numTrees, ref TreeInstance[] newTree){
		int randInt;
		System.Random myRand = new System.Random();
		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				if (myTerrels [x, y].terrainType == "forest") {
					randInt = myRand.Next (0, 100);
					if (randInt <= 5) {
						//Debug.Log ((float)y / 100f + " , " + (float)x / 100f);
						newTree [numTrees].position = new Vector3 ((float)y / (float)size_x, 0, (float)x / (float)size_x);//(y/1000f, 0,x/1000f);
						newTree [numTrees].heightScale = 1f;
						newTree [numTrees].widthScale = 1f;
						if (randInt % 3 == 0) {
							newTree [numTrees].color = Color.cyan;
							newTree [numTrees].lightmapColor = Color.cyan;
						} else {
							newTree [numTrees].color = Color.gray;
							newTree [numTrees].lightmapColor = Color.gray;
						}
						if (randInt % 2 == 0) {
							newTree [numTrees].prototypeIndex = 2;
						} else {
							newTree [numTrees].prototypeIndex = 1;
						}
						numTrees++;
					}
				}
			}
		}
	}

	void plantGrass(ref int numTrees, ref TreeInstance[] newTree){
		System.Random myRand = new System.Random();
		for (int x = 0; x < size_x*10; x++) {
			for (int y = 0; y < size_x*10; y++) {
				if (myTerrels [x/10, y/10].terrainType == "grassland") {
					if (myRand.Next (0, 100) < 2 && numTrees<10000) {
						//Debug.Log ((float)y / 100f + " , " + (float)x / 100f);
						newTree [numTrees].position = new Vector3 ((float)y / ((float)size_x*10f), 0, (float)x / ((float)size_x*10f));//(y/1000f, 0,x/1000f);
						newTree [numTrees].heightScale = 0.05f;
						newTree [numTrees].widthScale = 0.05f;
						newTree [numTrees].color = Color.cyan;
						newTree [numTrees].lightmapColor = Color.cyan;
						newTree [numTrees].prototypeIndex = 3;
						numTrees++;
					}
				}
			}
		}
	}

	void plantBushes(ref int numTrees, ref TreeInstance[] newTree){
		System.Random myRand = new System.Random();
		for (int x = 0; x < (size_x); x++) {
			for (int y = 0; y < (size_x); y++) {
				if (myTerrels [x, y].terrainType == "desert" && myTerrels [x, y].height==0.0f){
					if (myRand.Next (0, 10) < 5) {
						//Debug.Log ((float)y / 100f + " , " + (float)x / 100f);
						newTree [numTrees].position = new Vector3 ((float)y / (float)size_x, 0, (float)x / (float)size_x);//(y/1000f, 0,x/1000f);
						newTree [numTrees].heightScale = 0.3f;
						newTree [numTrees].widthScale = 0.5f;
						newTree [numTrees].color = Color.cyan;
						newTree [numTrees].lightmapColor = Color.cyan;
						newTree [numTrees].prototypeIndex = 2;
						numTrees++;
					}
				}
			}
		}
	}

	void raiseDesert(){
		for (int z = 0; z < size_x; z++) {
			for (int x = 0; x < size_x; x++) {
				
			}
		}
	}

	void prototypeMountain(){
		int[,] locations = new int[100000, 2];
		int numLocs;  
		for (int z = 0; z < size_x; z++) {
			for (int x = 0; x < size_x; x++) {
				if (myTerrels [x, z].terrainType == "peak") {
					//Debug.Log ("Boop.");
					//myTerrels [x, z].height = 1.0f;
					for (int r = 0; r < 30; r++) {
						numLocs = calcCircle (r, ref locations);
						for (int p = 0; p < numLocs; p++) {
							if (myTerrels [x + locations [p, 0], z + locations [p, 1]].height < 1.0f - 0.1f * r) {
								myTerrels [x + locations [p, 0], z + locations [p, 1]].height = 1.0f - 0.1f * r;
							}	
						}
					}
				}
			}
		}
	}
}
