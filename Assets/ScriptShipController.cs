using UnityEngine;
using System.Collections;

public class ScriptShipController : MonoBehaviour {
	
	public ScriptMainInput scriptMainInput;
	
	public float thrustForceConstant = 10.0F;
	public float turnSpeedConstant = 1.0F;
	public Vector3 forwardDirection;
	public GameObject basicBullet;
	public float bulletForceConstant = 10.0F;
	public GameObject newBullet = null;
	//public float inputTest = 0.0F;
	
	
	// Use this for initialization
	void Start () {
	
		scriptMainInput = GetComponent<ScriptMainInput>();
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		forwardDirection = transform.TransformDirection(Vector3.forward);
		rigidbody.AddForce(forwardDirection * scriptMainInput.thrustInput * thrustForceConstant * Time.fixedDeltaTime);
		
		rigidbody.angularVelocity = new Vector3(0, scriptMainInput.turnInput * turnSpeedConstant, 0);
		
		if(scriptMainInput.fireInputA){
			GameObject newBullet = Instantiate(basicBullet, transform.position, transform.rotation) as GameObject;
			newBullet.GetComponent<Rigidbody>().AddForce(forwardDirection * bulletForceConstant);
		}
		//inputTest = scriptMainInput.turnInput;
		//transform.rotation = Quaternion.Euler(0, 
		//transform.Rotate(0, scriptMainInput.turnInput * turnSpeedConstant * Time.fixedDeltaTime, 0);
	}
}
