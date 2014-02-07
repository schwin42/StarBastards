using UnityEngine;
using System.Collections;

public class ScriptMainInput : MonoBehaviour {
	
	public float thrustInput = 0.0F;
	public float turnInput = -9999F;
	public bool queueFire = false;
	public bool[] p1Input;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		thrustInput = Input.GetAxis("P01Thrust");
		p1Input = new bool[]{Input.GetButton ("P01Left"), Input.GetButton ("P01Right")};
		//float p1LeftDown = Input.GetButton ("P01Left");
		//float p1RightDown = Input.GetButton ("P01Right");

		if (p1Input[0] == false && p1Input[1] == false) {
			turnInput = 0F;
		} else if (p1Input[0] == true && p1Input[1] == false) {
			turnInput = -1F;
		} else if (p1Input[0] == false && p1Input[1] == true){
			turnInput = 1F;
		} else if (p1Input[0] == true && p1Input[1] == true) {
			turnInput = 0F;
				} else {
			Debug.LogError("Invalid input for player 1: " + Input.GetButton ("P01Left").ToString() + Input.GetButton ("P01Right").ToString ());
		}

		if (Input.GetButtonDown ("P01FireA")) {
			queueFire = true;
				};

	}
}
