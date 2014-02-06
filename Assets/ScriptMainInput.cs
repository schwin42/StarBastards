using UnityEngine;
using System.Collections;

public class ScriptMainInput : MonoBehaviour {
	
	public float thrustInput = 0.0F;
	public float turnInput = 0.0F;
	public bool fireInputA = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		thrustInput = Input.GetAxis("P01Thrust");
		turnInput = Input.GetAxis ("P01Turn");
		if(Input.GetButtonDown("P01FireA")){
			fireInputA = true;	
		} else {
			fireInputA = false;
		}
		
			
		
	}
}
