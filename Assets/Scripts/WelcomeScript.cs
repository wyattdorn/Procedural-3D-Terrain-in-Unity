using UnityEngine;
using System.Collections;

public class WelcomeScript : MonoBehaviour {


	// Update is called once per frame
	void Update () {

		if (Input.GetKeyUp(KeyCode.Q)) {

			Debug.Log ("exit1");
			this.enabled = false;

		}

		if (Input.anyKeyDown) {
			Debug.Log ("exit");
			this.enabled = false;
		}
	}
}
