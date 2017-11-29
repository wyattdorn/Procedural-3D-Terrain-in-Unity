using UnityEngine;
using System.Collections;

/// <summary>
/// The Following is an implementation of the Diamond-Square fractal algorithm.
/// 
/// </summary>

public class NeighborFractals : MonoBehaviour {

	System.Random randNum;
	float[,] heightmap;
	float[,] heights;
	TerrainData td;
	int hash;
	int fractalSize;
	int size_x;
	float roughness;

	public void generateFractal(ref float[,] myHeights, int size, float rough){
		roughness = rough;
		fractalSize = size * 2;
		heightmap = new float[fractalSize,fractalSize];
		for (int x = 0; x < fractalSize; x++) {
			for (int y = 0; y < fractalSize; y++) {
				heightmap [x, y] = 0.5f;
			}
		}
		returnTerrain (ref myHeights, ref heightmap, size);
	}

	void returnTerrain(ref float[,] myHeights, ref float[,] heightmap, int size){
		randNum = new System.Random();
		size_x = fractalSize - 1;
		halve (size_x);
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				myHeights [x, y] = heightmap [x + 512, y + 512];
			}
		}
	}

	void halve(int fractalSize){
		int half = fractalSize / 2;
		if (half < 1) {
			//We've reached the last itteration of the loop
			return;
		}
		for (int y = half; y < size_x; y += fractalSize) {
			for (int x = half; x < size_x; x += fractalSize) {
				square(x, y, half, ((float)randNum.NextDouble() - 0.5f)/(size_x/fractalSize));
			}
		}
		for (int y = 0; y <= size_x; y += half) {
			for (int x = (y + half) % fractalSize; x <= size_x; x += fractalSize) {
				diamond(x, y, half, ((float)randNum.NextDouble() - 0.5f)/(size_x/fractalSize));
			}
		}
		halve(half);
	}
		
	void diamond(int x, int y, int fractalSize, float offset){
		if (heightmap [x, y] != 0.5) {
			return;
		}
		float average = avg(new float[4]{	isVAlidPoint(x, y - fractalSize),
											isVAlidPoint(x + fractalSize, y),
											isVAlidPoint(x, y + fractalSize),
											isVAlidPoint(x - fractalSize, y)});
		heightmap[x, y] = average + offset * roughness;
	}

	void square(int x, int y, int fractalSize, float offset){
		if (heightmap [x, y] != 0.5) {
			return;
		}
		float average = avg(new float[4]{	isVAlidPoint (x - fractalSize, y - fractalSize),
											isVAlidPoint (x + fractalSize, y - fractalSize),
											isVAlidPoint (x + fractalSize, y + fractalSize),
											isVAlidPoint (x - fractalSize, y + fractalSize)});
		heightmap [x, y] = average + offset * roughness;
	}
		
	float isVAlidPoint(int x, int y){
		if (x < 0 || x > size_x || y < 0 || y > size_x) {
			return 0.0f;
		}
		return heightmap[x, y];
	}

	float avg(float[] values){
		float total = 0f;
		for (int x = 0; x < 4; x++) {
			total += values [x];
		}
		return total / 4.0f;
	}
}