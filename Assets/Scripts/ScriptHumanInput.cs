using UnityEngine;
using System.Collections;

//public enum ControlType
//{
//	Keyboard,
//	Touch
//}

public class ScriptHumanInput : MonoBehaviour {

	//Configurable
	//public ControlType controlType;
	
	//Internal
	public float thrustInput = 0.0F;
	public float turnInput = -9999F;
	public bool queueFire = false;
	[System.NonSerialized]
	public bool[] mainInput = new bool[]{false, false};
	public string playerInput;
	//private ScriptShipSheet scriptShipSheet;

	//GameObjects
	ScriptGameController scriptGameController;

	// Use this for initialization
	void Start () {
	//	scriptShipSheet = GetComponent<ScriptShipSheet>();
		scriptGameController = GameObject.Find ("ControllerGame").GetComponent<ScriptGameController>();
	}
	
	// Update is called once per frame
	void Update () {

		if(scriptGameController.controlType == ControlType.Keyboard)
		{

		thrustInput = Input.GetAxis(playerInput + "Thrust");
		mainInput = new bool[]{Input.GetButton (playerInput + "Left"), Input.GetButton (playerInput + "Right")};
		//float p1LeftDown = Input.GetButton ("P01Left");
		//float p1RightDown = Input.GetButton ("P01Right");
		} else if (scriptGameController.controlType == ControlType.Touch){
	
			//Wait for input from button script

		} else {
			Debug.Log ("No control type selected for " + playerInput);
		}

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

		//if (Input.GetButtonDown ("P01FireA")) {
		//	queueFire = true;
		//		};

	}
}
