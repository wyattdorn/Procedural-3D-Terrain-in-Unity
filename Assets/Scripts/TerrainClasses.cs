using UnityEngine;
using System.Collections;

public class TerrainClasses : MonoBehaviour {


	//////////////////////////////////////////////////////
	/// This is the plant class
	/// 
	///////////////////////////////////////////////////////
	public class Terrel{//Terrel (Terrain element) class
		public string terrainType;
		//Height from 0.0f-1.0f
		public float height;
		float water_ratio, mountain_ratio, forest_ratio, grassland_ratio, desert_ratio;
		float[] terrainRatios;
		string[] terrainTypes = new string[5];

		public void init(){
			this.terrainType = "water";
			this.height = 0.0f;
			this.terrainTypes[0] = "water";
			this.terrainTypes[1] = "mountain";
			this.terrainTypes[2] = "forest";
			this.terrainTypes[3] = "grassland";
			this.terrainTypes[4] = "desert";
		}

	}
}
