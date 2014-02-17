/*
KNOWN ISSUES:
//On occasion, player 01 thrust effect does not trigger due to isThrusting not being set to true. Fixed?
//On occasion, module is assimilated at coordinates of existing module
//For unknown reason, each ship's rotation increases by 90 or so on start
*/

using UnityEngine;
using System.Collections;

public class ScriptShipController : MonoBehaviour {

	//Configurable
	public PlayerControl playerControl = PlayerControl.None;
	
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
	private float ratioOfNodeToSpace = 2F;
	private Transform shipModuleContainer;
	private ScriptHumanInput scriptHumanInput;
	private ScriptShipIntelligence scriptShipIntelligence;
	private ScriptShipSheet scriptShipSheet;
	private ScriptModuleController scriptModuleController;
	private Vector2 forwardDirection;
	private bool rigidbodyResetPending = false;
	private Vector2 lastVelocity;
	//public Rigidbody2D rigidCharacter;
	//private GameObject newBullet = null;

	//Status
	[System.NonSerialized]
	public bool isThrusting = false;

	// Use this for initialization
	void Start () {
	
		//Get other objects
		scriptHumanInput = GetComponent<ScriptHumanInput>();
		scriptShipIntelligence = GetComponent<ScriptShipIntelligence>();
		scriptShipSheet = GetComponent<ScriptShipSheet>();
		shipModuleContainer = transform.FindChild ("ContainerModule");
		scriptModuleController = GameObject.Find ("ControllerSpace").GetComponent<ScriptModuleController> ();

		//Set controlling rigidbody
		//if (scriptModuleController.moduleRigidbodyMode) {
		//				rigidCharacter = pilotModule.rigidbody2D;
		//		} else {
			//rigidCharacter = this.rigidbody2D;
		//		}

		//Cache starting rigidbody values from inspector
		rigidbodyMass = rigidbody2D.mass;
		rigidbodyLinearDrag = rigidbody2D.drag;
		rigidbodyAngularDrag = rigidbody2D.angularDrag;

		//Register starting modules
		ScriptModule[] iterationScripts = shipModuleContainer.GetComponentsInChildren<ScriptModule> ();
		foreach (ScriptModule module in iterationScripts) {
			module.moduleID = scriptModuleController.GetNextID();
			Vector3 vector3Position = module.transform.localPosition;
			Vector2 hotCoordinates = LocalPositionToNodeCoordinates(new Vector2(vector3Position.x, vector3Position.y));
			AddModule(module, null, hotCoordinates);
			}

		//for (int i = 0; i < shipModuleContainer.childCount; i++) {
		//	GameObject hotMod = shipModuleContainer.GetChild(i).gameObject;
		//	Vector3 vector3Position = hotMod.transform.localPosition;
		//	Vector2 hotCoordinates = LocalPositionToNodeCoordinates(new Vector2(vector3Position.x, vector3Position.y));
		//	AddModule(hotMod.gameObject, hotCoordinates);
		//	}

		//Set rigidbody velocity to zero
		//rigidbody2D.velocity = Vector2.zero;
		//rigidbody2D.angularVelocity = 0F;


	}
	
	// Update is called once per frame

	void FixedUpdate () {
		//Update pilot module velocity
		if (rigidbody2D) {

			//Temporary variables
			forwardDirection = transform.TransformDirection (Vector2.up);
			Rigidbody2D hotRigid = null;
					

						float thrustInput = 0;
						float turnInput = 0;

						if (playerControl == PlayerControl.Human) {
								thrustInput = scriptHumanInput.thrustInput;
								turnInput = scriptHumanInput.turnInput;
						} else if (playerControl == PlayerControl.Computer) {
								thrustInput = scriptShipIntelligence.thrustInput;
								turnInput = scriptShipIntelligence.turnInput;
						} else {
								Debug.Log ("No control selected for " + this);
						}

			//Update rigidbody of ship or pilot module
	
			if(scriptModuleController.moduleRigidbodyMode)
			{
				hotRigid = pilotModule.rigidbody2D;
				Debug.Log ("pilot rigidbody mode");
			} else {
				hotRigid = rigidbody2D;
			}
				  	hotRigid.AddForce (forwardDirection * thrustInput * thrustForceConstant * Time.fixedDeltaTime);
					hotRigid.angularVelocity = turnInput * -turnSpeedConstant;

						if (!isThrusting && thrustInput == 1) {
								isThrusting = true;
								thrustEffect.enableEmission = true;
						} 
						if (isThrusting && thrustInput == 0) {
								isThrusting = false;
								thrustEffect.enableEmission = false;
						}
				} else if(rigidbodyResetPending){
			gameObject.AddComponent<Rigidbody2D> ();

			rigidbodyResetPending = false;

			//Recalculate mass and update velocity appropriately
			
			Vector2 newVelocity = lastVelocity;
			
			
			rigidbody2D.mass = rigidbodyMass;
			rigidbody2D.drag = rigidbodyLinearDrag;
			rigidbody2D.angularDrag = rigidbodyAngularDrag;
			rigidbody2D.velocity = newVelocity;
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

	//void ReadShipToSchematic(ScriptShipController ship)
	//{
	//
	//}


	void RemoveModule()
	{

		//UNDER CONSTRUCTION
	//	gameObject.AddComponent<Rigidbody2D>();
	//	moduleOwner = null;
	//	transform.parent = null;
	//	//transform.localPosition = Vector2.zero;
	//	shipSpaceCoordinates = Vector3.zero;
	//	tag = "NeutralModule";
	//	if(moduleType == ModuleType.Weapon)
	//	{
	//		canShoot = false;
	//	}
		
	}


	public void AddModule(ScriptModule addedModule, GameObject assimilatingObject, Vector2 nodeCoordinates)
	{
		//Temporary variables
		ScriptModule scriptModule = addedModule.GetComponent<ScriptModule> ();

		//Set ship as parent
		addedModule.transform.parent = shipModuleContainer; //Make new module child to ship
		//Vector2 assimilatingModulePosition = assimilatingModule.transform.position;

		//Log event
		scriptModule.ownTime = Time.frameCount;
		scriptModule.captureModule = assimilatingObject;

		//Update module
		scriptModule.moduleNodeCoordinates = nodeCoordinates; //Log coordinates to module
		Vector2 worldCoordinates = NodeCoordinatesToLocalPosition (nodeCoordinates);
		addedModule.transform.localPosition = worldCoordinates; //Set position
		Debug.Log (addedModule.transform.eulerAngles);
		addedModule.transform.localRotation = Quaternion.identity; //Set rotation
		Debug.Log (addedModule.transform.eulerAngles);
		addedModule.tag = "Ship"; //Tag as part of ship
		scriptModule.moduleOwner = this; //Mark this ship as new owner

		//Verify module
		VerifyCoordinates (addedModule);

		//Handle Rigidbody

		if (scriptModuleController.moduleRigidbodyMode) {
			if(addedModule.moduleType != ModuleType.Pilot)
			{
			DistanceJoint2D hotJoint = addedModule.gameObject.AddComponent<DistanceJoint2D>();
			hotJoint.connectedBody = assimilatingObject.rigidbody2D;
			//	hotJoint.distance;
			//	hotJoint.anchor;
			//	hotJoint.connectedAnchor;
			}
				} else {
					//Destroy module's rigidbody
						if (addedModule.rigidbody2D) {
								Destroy (addedModule.gameObject.rigidbody2D); 
						}
			//Ship rigidbody
			lastVelocity = rigidbody2D.velocity; //Cache rigidbody velocity
			Destroy(rigidbody2D); //Destroy rigidbody for replacement
			rigidbodyResetPending = true;
				}






	

		//Ready module
		if(scriptModule.moduleType == ModuleType.Weapon) //Ready weapon
		{
			scriptModule.canShoot = true;
		}
	}
	/*
	IEnumerator ResetShipRigidbody(Vector2 lastVelocity)
	{
		Debug.Log ("Rigidbody Reset Init" + gameObject.name);
		yield return 0;
		//if (!rigidbody2D) {
						gameObject.AddComponent<Rigidbody2D> ();

						//Recalculate mass and update velocity appropriately
		
						Vector2 newVelocity = lastVelocity;
		

						rigidbody2D.mass = rigidbodyMass;
						rigidbody2D.drag = rigidbodyLinearDrag;
						rigidbody2D.angularDrag = rigidbodyAngularDrag;
						rigidbody2D.velocity = newVelocity;
			//	} else {
			//			StartCoroutine (ResetShipRigidbody (lastVelocity));
			//	}
	}
*/
	Vector2 NodeCoordinatesToLocalPosition(Vector2 nodeCoordinates)
	{
		return nodeCoordinates * ratioOfNodeToSpace;
	}

	Vector2 LocalPositionToNodeCoordinates(Vector2 localPosition)
	{
		return localPosition / ratioOfNodeToSpace;
	}

	void VerifyCoordinates(ScriptModule hotMod)
	{
		//Throw error iff hotMod's coordinates match another child object of this ship's  module container
		ScriptModule[] iterationScripts = shipModuleContainer.GetComponentsInChildren<ScriptModule> ();
		foreach (ScriptModule otherMod in iterationScripts) {
			if(hotMod.moduleNodeCoordinates == otherMod.moduleNodeCoordinates && hotMod != otherMod)
			{
				Debug.LogError(hotMod.moduleID + "'s coordinates conflict with " + otherMod.moduleID + "'s.");
			}
		//	Vector3 vector3Position = module.transform.localPosition;
		//	Vector2 hotCoordinates = LocalPositionToNodeCoordinates(new Vector2(vector3Position.x, vector3Position.y));
		//	AddModule(module, hotCoordinates);
		}
	}


}