/*
KNOWN ISSUES:
//On occasion, player 01 thrust effect does not trigger due to isThrusting not being set to true
//On occasion, module assimilation does not work properly
*/

using UnityEngine;
using System.Collections;

public class ScriptShipController : MonoBehaviour {

	//Inspector Assigned
	public GameObject pilotModule;
	public GameObject basicBullet;
	public ParticleSystem thrustEffect;
	public GameObject target;


	//Configurable variables
	public float thrustForceConstant = 10.0F;
	public float turnSpeedConstant = 1.0F;
	public float bulletForceConstant = 10.0F;
	public float topSpeed = 10.0F; 

	//Cached from inspector
	[System.NonSerialized]
	public float rigidbodyMass;
	[System.NonSerialized]
	public float rigidbodyLinearDrag;
	[System.NonSerialized]
	public float rigidbodyAngularDrag;

	//Private variables
	private ScriptMainInput scriptMainInput;
	private Vector2 forwardDirection;
	//private GameObject newBullet = null;

	//Status
	public bool isThrusting = false;

	// Use this for initialization
	void Start () {
	
		scriptMainInput = GetComponent<ScriptMainInput>();

		rigidbodyMass = rigidbody2D.mass;
		rigidbodyLinearDrag = rigidbody2D.drag;
		rigidbodyAngularDrag = rigidbody2D.angularDrag;

	}
	
	// Update is called once per frame

	void FixedUpdate () {
		//Update pilot module velocity
		if(rigidbody2D)
		{
		forwardDirection = transform.TransformDirection(Vector2.up);
		rigidbody2D.AddForce(forwardDirection * scriptMainInput.thrustInput * thrustForceConstant * Time.fixedDeltaTime);
		rigidbody2D.angularVelocity = scriptMainInput.turnInput * -turnSpeedConstant;

		if(!isThrusting && scriptMainInput.thrustInput == 1)
			{
				isThrusting = true;
				thrustEffect.enableEmission = true;
			} 
		if(isThrusting && scriptMainInput.thrustInput == 0)
			{
				isThrusting = false;
				thrustEffect.enableEmission = false;
			}

		}
		//Update parent object position
		//transform.position = pilotModule.transform.position;
		//transform.rotation = pilotModule.transform.rotation;

		//if(scriptMainInput.queueFire){
		//	scriptMainInput.queueFire = false;
		//	GameObject newBullet = Instantiate(basicBullet, transform.position, transform.rotation) as GameObject;
		//	newBullet.GetComponent<Rigidbody>().AddForce(forwardDirection * bulletForceConstant);
		//}
		//inputTest = scriptMainInput.turnInput;
		//transform.rotation = Quaternion.Euler(0, 
		//transform.Rotate(0, scriptMainInput.turnInput * turnSpeedConstant * Time.fixedDeltaTime, 0);
	}

}
