using UnityEngine;
using System.Collections;

public class MyFractal : MonoBehaviour {

	int size_x;
	float[,] heights;
	System.Random myRand;

	Terrain myTerrain;

	// Use this for initialization
	void Start () {

		size_x = 16;

		myTerrain = GetComponent<Terrain>();
		myTerrain.terrainData.alphamapResolution = size_x;
		myTerrain.terrainData.heightmapResolution = size_x;


		heights = new float[size_x,size_x];

		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				heights [x, y] = -1f;
			}
		}

		heights [0, 0] = 1f;
		heights [0, size_x-1] = 1f;
		heights [size_x-1, 0] = 1f;
		heights [size_x-1, size_x-1] = 1f;

		fractal ();

		myTerrain.terrainData.SetHeights (0, 0, heights);
	
	}

	void fractal(){
		//31, 15, 7, 3, 1, 0

		int squares = 1;
		int scope = size_x-1;

		while (scope > 0) {
			Debug.Log ("Scope is: " + scope);
			Debug.Log ("Squares: " + squares);
			for (int i = 0; i < (int)Mathf.Sqrt ((float)(squares)); i++) {
				for (int j = 0; j < (int)Mathf.Sqrt ((float)(squares)); j++) {
					Vector2 basePoint = new Vector2(i*scope, j*scope);
					squareValue (basePoint, scope);
					squareValue (basePoint + new Vector2(0, scope), scope);
					squareValue (basePoint + new Vector2(scope, 0), scope);
					squareValue (basePoint + new Vector2(scope, scope), scope);
					diamondValue (basePoint, scope);
				}
			}

			scope = Mathf.FloorToInt(scope / 2);
			squares *= 4;
		}
	}

	void squareValue(Vector2 point, int scope){
		int count = 0;
		float result = 0f;
		float newScope = (float)scope / 2f;

		if (heights [(int)point [0], (int)point [1]] == -1f) {
			heights [(int)point [0], (int)point [1]] = 0;
			//Debug.Log ("(" + point [0] + " , " + point [1] + ")");
			//return;
		} else {
			return;
		}

		if (isValidPoint (new Vector2 (point [0] + newScope, point [1] + 0))) {
			count++;
			result += heights [(int)(point [0] + newScope), (int)(point [1] + 0)];
		}
		if (isValidPoint (new Vector2 (point [0] - scope / 2f, point [1] + 0))) {
			count++;
			result += heights [(int)(point [0] - newScope), (int)(point [1] + 0)];
		}
		if (isValidPoint (new Vector2 (point [0] + 0, point [1] + scope / 2))) {
			count++;
			result += heights [(int)(point [0] + 0), (int)(point [1] + newScope)];
		}
		if (isValidPoint (new Vector2 (point [0] + 0, point [1] - scope / 2))) {
			count++;
			result += heights [(int)(point [0] + 0), (int)(point [1] - newScope)];
		}

		if (count == 0) {
			//Debug.Log ("CAnnot dvide by 0");
			return;
		}
		heights [(int)point [0], (int)point [1]] = (float)(result / count);
		/*
		Vector2[] neighbors =  new Vector2[4];
		for (int x = 0; x < 4; x++) {
			if(isValidPoint(
			neighbors [x] = new Vector2 (0f, 0f);
		}
		*/
	}

	void diamondValue(Vector2 point, int scope){
		int count = 0;
		float result = 0f;
		float newScope = (float)scope / 2;

		if (heights [(int)point [0], (int)point [1]] == -1f) {
			heights [(int)point [0], (int)point [1]] = 0;
			Debug.Log ("Diamond: (" + point [0] + " , " + point [1] + ")");
		} else {
			return;
		}

		if (isValidPoint (new Vector2 (point [0] + newScope, point [1] + newScope))) {
			count++;
			result += heights [(int)(point [0] + newScope), (int)(point [1] + newScope)];
		}
		if (isValidPoint (new Vector2 (point [0] - newScope, point [1] + newScope))) {
			count++;
			result += heights [(int)(point [0] - newScope), (int)(point [1] + newScope)];
		}
		if (isValidPoint (new Vector2 (point [0] + newScope, point [1] - newScope))) {
			count++;
			result += heights [(int)(point [0] + newScope), (int)(point [1] - newScope)];
		}
		if (isValidPoint (new Vector2 (point [0] - newScope, point [1] - newScope))) {
			count++;
			result += heights [(int)(point [0] - newScope), (int)(point [1] - newScope)];
		}

		if (count == 0) {
			//Debug.Log ("Cannot dvide by 0");
			return;
		}
		heights[(int)point[0], (int)point[1]] = (float)(result / count);
	}

	bool isValidPoint(Vector2 vec){
		if (vec [0] >= 0 && vec [0] < size_x && vec [1] >= 0 && vec [1] < size_x && heights[(int)vec [0], (int)vec [1]]>=0) {
			return true;
		}
		return false;
	}
}
