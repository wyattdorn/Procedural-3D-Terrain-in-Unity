using UnityEngine;
using System.Collections;
using System.Linq; // used for Sum of array

public class regions : MonoBehaviour {

	int size_x = 100;
	int size_z = 100;

	int numMountains = -1;


	char[,] region;

	string seed = Time.time.ToString ();
	System.Random pseudoRandom;

	void Start () {
		/*region types:
		 * -Mountain
		 * -Plain
		 * -Hills
		 * */
		//create a 2D array of chars, the sam size as our terrain, to represent the various terrain regions in the terrain
		region = new char[size_x, size_z];
		for (int i = 0; i < size_x; i++) {
			for (int j = 0; j < size_z; j++) {
				region [i, j] = '-';
			}
		}
		pseudoRandom = new System.Random (seed.GetHashCode ());
		numMountains = generateMountians (numMountains);
	}

	int generateMountians(int mounts)
	{
		int temp;
		int sign;
		int[] sizes = new int[2];
		sizes [0] = size_x;
		sizes [1] = size_z;

		//If numMountains == -1, randomize number of mountains
		if (mounts < 0) {
			mounts = pseudoRandom.Next (3, 7);
		}
		int[,] mountains = new int[mounts, 2];

		//generate a mountain within the map
		mountains[0,0] = pseudoRandom.Next (size_x / 10, size_x - (size_x / 10));
		mountains[0,1] = pseudoRandom.Next (size_z / 10, size_z - (size_z / 10));

		//create next mountain 15-30 units from initial mountain

		for (int i = 1; i < mounts; i++) {
			for (int j = 0; j < 2; j++) {
				//Set initial location for current mountain to location of previous mountain
				mountains [i, j] = mountains [i - 1, j];
				//prodice positive or negative result
				sign = pseudoRandom.Next (0, 10);
				if (sign % 2 == 0) {
					sign = 1;
				} else {
					sign = -1;
				}

				do {
					temp = (size_x + sign * pseudoRandom.Next (15, 30));
				} while(mountains [i, j]  + temp <= sizes[j]);

				mountains [i, j] += temp;
			}

		}

		for (int x = 0; x < mounts; x++) {
			region [mountains [0, 0], mountains [0, 1]] = 'm';
		}

		return mounts;
	}
		

}
