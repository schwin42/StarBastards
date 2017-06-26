using UnityEngine;
using System.Collections;

//public enum ControlType
//{
//	Keyboard,
//	Touch
//}

public class HumanPilotInput : IPilotInput
{

	//Configurable
	//public ControlType controlType;
	
	//Internal
	private float _thrustInput;
	public float ThrustInput { get { return _thrustInput; } }

	private float _turnInput;
	public float TurnInput { get { return _turnInput; } } 

	public bool queueFire = false;
	[System.NonSerialized]
	public bool[] mainInput = new bool[]{ false, false };
	public string playerInput;

	public void SetThrustInput(float value) {
		_thrustInput = value;
	}
		
	// Update is called once per frame
	void Update() {
				

		if (GameController.Instance.humanInputModality == ControlType.Keyboard) {
			_thrustInput = Input.GetAxis(playerInput + "Thrust");
			mainInput = new bool[]{ Input.GetButton(playerInput + "Left"), Input.GetButton(playerInput + "Right") };
			//float p1LeftDown = Input.GetButton ("P01Left");
			//float p1RightDown = Input.GetButton ("P01Right");
		} else if (GameController.Instance.humanInputModality == ControlType.Touch) {
			//Wait for input from button script
		} else {
			Debug.LogWarning("No control type selected for " + playerInput);
		}

		if (mainInput[0] == false && mainInput[1] == false) {
			_turnInput = 0F;
		} else if (mainInput[0] == true && mainInput[1] == false) {
			_turnInput = -1F;
		} else if (mainInput[0] == false && mainInput[1] == true) {
			_turnInput = 1F;
		} else if (mainInput[0] == true && mainInput[1] == true) {
			_turnInput = 0F;
		} else {
			//Debug.LogError("Invalid input for player 1: " + Input.GetButton ("P01Left").ToString() + Input.GetButton ("P01Right").ToString ());
		}

		//if (Input.GetButtonDown ("P01FireA")) {
		//	queueFire = true;
		//		};

	}
}
