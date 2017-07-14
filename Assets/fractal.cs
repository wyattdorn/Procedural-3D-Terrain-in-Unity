using UnityEngine;
using System.Collections;

public class fractal : MonoBehaviour {

	System.Random pseudoRandom;

	float[,] heightmap;
	float[] fractalArray;
	TerrainData td;

	int size_x = 10;

	Terrain bit;

	// Use this for initialization
	void Start () {

		heightmap = new float[size_x,size_x];
		fractalArray = new float[size_x*size_x];

		for (int x = 0; x < size_x * size_x; x++) {
			fractalArray [x] = 0.5f;
		}

		bit = GetComponent<Terrain>();
		bit.terrainData.heightmapResolution = size_x;

		fill2DFractArray(ref fractalArray, 4, 27, 1.0f, 0.8f);

		for (int x = 0; x < size_x; x++) {
			for (int y = 0; y < size_x; y++) {
				heightmap [x, y] = fractalArray [y + x * size_x];
			}
		}

		bit.terrainData.SetHeights (0, 0, heightmap);

	}
	

	void fill2DFractArray (ref float[] fa, int size, int seedValue, float heightScale, float h){
	    int	i, j;
	    int	stride;
		bool oddline;
	    int subSize;
		float ratio, scale;


		/* We can't tesselate the array if it is not a power of 2. */


	    /* subSize is the dimension of the array in terms of connected line
	       segments, while size is the dimension in terms of number of
	       vertices. */
	    subSize = size;
	    size++;
	    
	    /* initialize random number generator */
		pseudoRandom = new System.Random ();//seedValue);

		/* Set up our roughness constants.
		   Random numbers are always generated in the range 0.0 to 1.0.
		   'scale' is multiplied by the randum number.
		   'ratio' is multiplied by 'scale' after each iteration
		   to effectively reduce the randum number range.
		   */
		ratio = (float)Mathf.Pow(2.0f,-h);
		scale = heightScale * ratio;

	    /* Seed the first four values. For example, in a 4x4 array, we
	       would initialize the data points indicated by '*':

	           *   .   .   .   *

	           .   .   .   .   .

	           .   .   .   .   .

	           .   .   .   .   .

	           *   .   .   .   *

	       In terms of the "diamond-square" algorithm, this gives us
	       "squares".

	       We want the four corners of the array to have the same
	       point. This will allow us to tile the arrays next to each other
	       such that they join seemlessly. */

	    stride = subSize / 2;
	    fa[(0*size)+0] = fa[(subSize*size)+0] = fa[(subSize*size)+subSize] = fa[(0*size)+subSize] = 0.0f;
	    

	    /* Now we add ever-increasing detail based on the "diamond" seeded
	       values. We loop over stride, which gets cut in half at the
	       bottom of the loop. Since it's an int, eventually division by 2
	       will produce a zero result, terminating the loop. */
	    while (stride!=0) {
			/* Take the existing "square" data and produce "diamond"
			   data. On the first pass through with a 4x4 matrix, the
			   existing data is shown as "X"s, and we need to generate the
		       "*" now:

	               X   .   .   .   X

	               .   .   .   .   .

	               .   .   *   .   .

	               .   .   .   .   .

	               X   .   .   .   X

		      It doesn't look like diamonds. What it actually is, for the
		      first pass, is the corners of four diamonds meeting at the
		      center of the array. */
			for (i=stride; i<subSize; i+=stride) {
				for (j=stride; j<subSize; j+=stride) {
					fa[(i * size) + j] =
						scale * fractRand (.5f) +
						avgSquareVals (i, j, stride, size, ref fa);
					j += stride;
				}
				i += stride;
			}

			/* Take the existing "diamond" data and make it into
		       "squares". Back to our 4X4 example: The first time we
		       encounter this code, the existing values are represented by
		       "X"s, and the values we want to generate here are "*"s:

	               X   .   *   .   X

	               .   .   .   .   .

	               *   .   X   .   *

	               .   .   .   .   .

	               X   .   *   .   X

		       i and j represent our (x,y) position in the array. The
		       first value we want to generate is at (i=2,j=0), and we use
		       "oddline" and "stride" to increment j to the desired value.
		       */
			oddline = false;
			for (i=0; i<subSize; i+=stride) {
			    oddline = (oddline == false);
				for (j=0; j<subSize; j+=stride) {
					if ((oddline) && !(j==0)) j+=stride;

					/* i and j are setup. Call avgDiamondVals with the
					   current position. It will return the average of the
					   surrounding diamond data points. */
					fa[(i * size) + j] =
						scale * fractRand (.5f) +
						avgDiamondVals (i, j, stride, size, subSize, ref fa);

					/* To wrap edges seamlessly, copy edge values around
					   to other side of array */
					if (i==0)
						fa[(subSize*size) + j] =
							fa[(i * size) + j];
					if (j==0)
						fa[(i*size) + subSize] =
							fa[(i * size) + j];

					j+=stride;
				}
			}


			/* reduce random number range. */
			scale *= ratio;
			stride >>= 1;
	    }
	}

	float fractRand(float v){
		pseudoRandom = new System.Random ();
		return ((float)(pseudoRandom.NextDouble ()) - 0.5f) * 2*v;
	}


	float avgDiamondVals (int i, int j, int stride, int size, int subSize, ref float[] fa)
	{
		/* In this diagram, our input stride is 1, the i,j location is
       indicated by "X", and the four value we want to average are
       "*"s:
           .   *   .

           *   X   *

           .   *   .
       */

		/* In order to support tiled surfaces which meet seamless at the
		edges (that is, they "wrap"), We need to be careful how we
		calculate averages when the i,j diamond center lies on an edge
		of the array. The first four 'if' clauses handle these
		cases. The final 'else' clause handles the general case (in
			which i,j is not on an edge).
		*/
		/*if (i == 0)
			return ((float) (fa[(i*size) + j-stride] +
				fa[(i*size) + j+stride] +
				fa[((subSize-stride)*size) + j] +
				fa[((i+stride)*size) + j]) * .25f);
		else if (i == size-1)
			return ((float) (fa[(i*size) + j-stride] +
				fa[(i*size) + j+stride] +
				fa[((i-stride)*size) + j] +
				fa[((0+stride)*size) + j]) * .25f);
		else if (j == 0)
			return ((float) (fa[((i-stride)*size) + j] +
				fa[((i+stride)*size) + j] +
				fa[(i*size) + j+stride] +
				fa[(i*size) + subSize-stride]) * .25f);
		else if (j == size-1)
			return ((float) (fa[((i-stride)*size) + j] +
				fa[((i+stride)*size) + j] +
				fa[(i*size) + j-stride] +
				fa[(i*size) + 0+stride]) * .25f);
		else*/
			Debug.Log ((i*size) + j-stride);
		/*return ((float)(fa [((i - stride) * size) + j] +
				fa[((i+stride)*size) + j] +
				fa[(i*size) + j-stride] +
				fa[(i*size) + j+stride]) * .25f);*/
		return (float)(pseudoRandom.NextDouble ());
	}

	static float avgSquareVals (int i, int j, int stride, int size, ref float[] fa)
	{
		/* In this diagram, our input stride is 1, the i,j location is
       indicated by "*", and the four value we want to average are
       "X"s:
           X   .   X

           .   *   .

           X   .   X
       */
		return ((float) (fa[((i-stride)*size) + j-stride] +
			fa[((i-stride)*size) + j+stride] +
			fa[((i+stride)*size) + j-stride] +
			fa[((i+stride)*size) + j+stride]) * .25f);
	}


}
