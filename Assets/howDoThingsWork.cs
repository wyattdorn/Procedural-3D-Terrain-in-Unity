using UnityEngine;
using System.Collections;

public class howDoThingsWork : MonoBehaviour {

	public int size_x = 1000;

	Terrain bit;

	Terrel[,] myTerrels;
	float[,] heights;

	public class Terrel{//Terrel (Terrain element) class
		//public char terrainType;
		public string terrainType;
		// For now 'm' will represent mountains, '-' will represent blanks
		public float height;
		//bool isWater;
		float mountain_ratio, forrest_ratio, grassland_ratio, desert_ratio;

		public void init(){
			terrainType = "blank";
			height = 0.0f;
			//isWater = false;
		}

		public bool isBlank(){
			if (this.terrainType == "blank")
				return true;
			return false;
		}

		public void setTerrain(string t){
			if (t == "mountain" || t == "mountains" || t == "Mountain" || t == "Mountains") {
				this.terrainType = "mountain";
				this.mountain_ratio = 1f;
				this.forrest_ratio = 0f;
				this.grassland_ratio = 0f;
			}
			else if (t == "forest" || t == "forests" || t == "Forest" || t == "Forests") {
				this.terrainType = "forest";
				this.mountain_ratio = 0f;
				this.forrest_ratio = 1f;
				this.grassland_ratio = 0f;
			}
			else if (t == "grassland" || t == "grasslands" || t == "Grassland" || t == "Grasslands") {
				this.terrainType = "grassland";
				this.mountain_ratio = 0f;
				this.forrest_ratio = 0f;
				this.grassland_ratio = 1f;
			}
			else if (t == "desert" || t == "deserts" || t == "Desert" || t == "Deserts") {
				this.terrainType = "desert";
				this.mountain_ratio = 0f;
				this.forrest_ratio = 0f;
				this.desert_ratio = 1f;
			}
		}
	}

	// Use this for initialization
	void Start () {
		
		bit = GetComponent<Terrain>();
		myTerrels = new Terrel[size_x, size_x];
		heights = new float[size_x, size_x];
		bit.terrainData.alphamapResolution = size_x;
		bit.terrainData.heightmapResolution = size_x;
		float[, ,] splatmapData = new float[bit.terrainData.alphamapResolution, bit.terrainData.alphamapResolution, bit.terrainData.alphamapLayers];

		for (int z = 0; z < size_x; z++) {
			for (int x = 0; x < size_x; x++) {
				heights [x, z] = 0.0f;
				splatmapData [x, z, 0] = 1;
			}
		}

		heights [(int)(500f*1.025f), (int)(500f*1.025f)] = 0.5f;

		bit.terrainData.SetHeights (0, 0, heights);


		splatmapData [499, 499, 2] = 1;
		splatmapData [499, 499, 0] = 0;
		splatmapData [500, 499, 2] = 1;
		splatmapData [499, 500, 0] = 0;
		splatmapData [499, 500, 2] = 1;
		splatmapData [500, 499, 0] = 0;
		splatmapData [500, 500, 2] = 1;
		splatmapData [500, 500, 0] = 0;
		bit.terrainData.SetAlphamaps(0, 0, splatmapData);

		TreeInstance[] newTree;
		newTree = new TreeInstance[10];

		newTree [0].position = new Vector3 (0.5f, 0.5f,0.5f);
		newTree [0].heightScale = 1;
		newTree [0].widthScale = 1;
		newTree [0].color = Color.cyan;
		newTree [0].lightmapColor = Color.cyan;
		newTree [0].prototypeIndex = 0;
			
		bit.terrainData.treeInstances = newTree	;
	
	}

	void textureTest(){
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
