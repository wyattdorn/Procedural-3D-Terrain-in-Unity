using UnityEngine;
using System.Collections;

public class TerrainTextures : TerrainScript {

	void textureTest(ref Terrain bit, ref TerrainClasses.Terrel[,] myTerrels, int size_x){
		float[, ,] splatmapData = new float[bit.terrainData.alphamapResolution, bit.terrainData.alphamapResolution, bit.terrainData.alphamapLayers];

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

		for (int y = 0; y < size_x; y++) {
			for (int x = 0; x < size_x; x++) {
				if (myTerrels [x, y].height >= 0.5f) {
					splatmapData [x, y, 0] = 1.0f;
				}
			}
		}

		bit.terrainData.SetAlphamaps(0, 0, splatmapData);
	}

}
