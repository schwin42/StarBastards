using UnityEngine;
using System.Collections;

public class ScriptMainInput : MonoBehaviour {
	
	public float thrustInput = 0.0F;
	public float turnInput = -9999F;
	public bool queueFire = false;
	public bool[] mainInput;
	public string playerInput = "P01"; 
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		thrustInput = Input.GetAxis(playerInput + "Thrust");
		mainInput = new bool[]{Input.GetButton (playerInput + "Left"), Input.GetButton (playerInput + "Right")};
		//float p1LeftDown = Input.GetButton ("P01Left");
		//float p1RightDown = Input.GetButton ("P01Right");

		if (mainInput[0] == false && mainInput[1] == false) {
			turnInput = 0F;
		} else if (mainInput[0] == true && mainInput[1] == false) {
			turnInput = -1F;
		} else if (mainInput[0] == false && mainInput[1] == true){
			turnInput = 1F;
		} else if (mainInput[0] == true && mainInput[1] == true) {
			turnInput = 0F;
				} else {
			//Debug.LogError("Invalid input for player 1: " + Input.GetButton ("P01Left").ToString() + Input.GetButton ("P01Right").ToString ());
		}

		if (Input.GetButtonDown ("P01FireA")) {
			queueFire = true;
				};

	}
}
