using UnityEngine;
using System.Collections;

public class CamEvents : MonoBehaviour {

	public Camera camera1;
	public Camera camera2;
	public Rigidbody FPSCam;
	public Canvas welcome;
	public Canvas FPSControls;

	void Start () {
		camera1.enabled = true;
		camera2.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Tab)) {
			welcome.enabled = false;
			camera1.enabled = !camera1.enabled;
			camera2.enabled = !camera2.enabled;
			FPSControls.enabled = camera2.enabled;
		}
		if (Input.GetKeyUp(KeyCode.Escape)) {
			Application.Quit ();
		}
	}
}
