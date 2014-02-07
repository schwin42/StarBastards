using UnityEngine;
using System.Collections;

public class ScriptShipController : MonoBehaviour {

	public GameObject basicBullet;

	//Configurable variables
	public float thrustForceConstant = 10.0F;
	public float turnSpeedConstant = 1.0F;
	public float bulletForceConstant = 10.0F;
	public float topSpeed = 10.0F; 

	private ScriptMainInput scriptMainInput;
	private Vector2 forwardDirection;
	private GameObject newBullet = null;
	
	// Use this for initialization
	void Start () {
	
		scriptMainInput = GetComponent<ScriptMainInput>();

		//Determine ship scaffold (place at which blocks will attach)

		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		forwardDirection = transform.TransformDirection(Vector2.up);
		rigidbody2D.AddForce(forwardDirection * scriptMainInput.thrustInput * thrustForceConstant * Time.fixedDeltaTime);
		
		rigidbody2D.angularVelocity = scriptMainInput.turnInput * -turnSpeedConstant;
		
		if(scriptMainInput.queueFire){
			scriptMainInput.queueFire = false;
			GameObject newBullet = Instantiate(basicBullet, transform.position, transform.rotation) as GameObject;
			newBullet.GetComponent<Rigidbody>().AddForce(forwardDirection * bulletForceConstant);
		}
		//inputTest = scriptMainInput.turnInput;
		//transform.rotation = Quaternion.Euler(0, 
		//transform.Rotate(0, scriptMainInput.turnInput * turnSpeedConstant * Time.fixedDeltaTime, 0);
	}
}
