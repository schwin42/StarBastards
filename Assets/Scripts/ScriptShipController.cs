using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
//public class Activation
//{
//	public int activationID; //ID is unique to ship, not universe
//	public ModuleType moduleType; //Gadget type
//	public List<Node> constituentNodes; //Nodes that make up this activation
//
//	//Proper stats
//		//Gun
//	public int gunDurationLevel = 0;
//	public int gunHomingLevel = 0;
//	public int gunNumberLevel = 0;
//	public int gunPowerLevel = 0;
//	public int gunRadiusLevel = 0;
//	public int gunSpeedLevel = 0;
//
//		//Armor
//	public int armorPowerLevel = 0;
//	public int armorRadiusLevel = 0;
//
//		//Laser
//	public int laserRadiusLevel = 0;
//	public int laserPowerLevel = 0;
//	public int laserNumberLevel = 0;
//
//	//Mechanical stats
//		//Gun
//	public int damage = 10;
//	public float bulletsPerSecond; //In seconds
//	public int shotForce = 2000;
//	//public int size = 0;
//	public int scatterAngle = 0;
//	public int durationInSeconds = 0;
//	public int homingConstant = 0;
//	public int bulletsPerShot = 0;
//	public int bulletScale = 0;
//		//Armor
//	public int bonusHP;
//		//Laser
//	public float laserTriggerScale;
//
//	//State
//	public bool canShoot = false;
//	public float shotTimer;
//
//	public Activation(int activationIDArg, ModuleType moduleTypeArg, List<Node> constituentNodesArg)
//	{
//		activationID = activationIDArg;
//		moduleType = moduleTypeArg;
//		constituentNodes = constituentNodesArg;
//		}
//
//	public void DeriveRealStats(ModuleType moduleType)
//	{
//		switch(moduleType)
//		{
//		case ModuleType.Weapon:
//		durationInSeconds = gunDurationLevel + 1; //Magic numbers, please change
//		homingConstant = gunHomingLevel;
//		bulletsPerSecond = (gunNumberLevel * 2) + 1;
//		damage = (gunPowerLevel + 1) * 10;
//		bulletScale = (gunRadiusLevel * 2) + 1;
//		shotForce = (gunSpeedLevel + 1) * 1000;
//			break;
//		case ModuleType.Armor:
//			bonusHP = armorPowerLevel * 10;
//			break;
//		case ModuleType.Laser:
//			laserTriggerScale = laserRadiusLevel * 10;
//			break;
//		default:
//			Debug.LogError("Invalid module type: " + moduleType);
//			break;
//		}
//	}
//
//}

public class ScriptShipController : MonoBehaviour {

//Configurable
	public PlayerControl playerControl = PlayerControl.None;
	public Color playerColor;
	//public Activation activation = new Activation();

//State
	//public List<Activation> currentActivations = new List<Activation> ();

	//Motion
	public float thrustForceConstant = 10.0F;
	public float turnSpeedConstant = 1.0F;
//	public float bulletForceConstant = 10.0F;
	//public float topSpeed = 10.0F; 
	public float teleportDelay = 1.0F;
	
//Inspector Assigned
	public GameObject pilotModule;
	public GameObject basicBullet;
	public ParticleSystem thrustEffect;
	public GameObject laserTrigger;
	public GameObject triggerContainer;
	//public GameObject target;


//Acquired
	[System.NonSerialized]
	public Transform shipModuleContainer;

//Cached from inspector
	[System.NonSerialized]
	public float rigidbodyMass;
	[System.NonSerialized]
	public float rigidbodyLinearDrag;
	[System.NonSerialized]
	public float rigidbodyAngularDrag;

//Private variables
	private float ratioOfNodeToSpace = 2F;
	private ScriptHumanInput scriptHumanInput;
	private ScriptShipIntelligence scriptShipIntelligence;
	private ScriptShipSheet scriptShipSheet;
	private ScriptGameController scriptModuleController;
	private Transform dynamicObjectsContainer;
	//private Vector2 forwardDirection;
	private bool rigidbodyResetPending = false;
	private Vector2 lastVelocity;
	private AudioSource audioSource;
	//public Rigidbody2D rigidCharacter;
	//private GameObject newBullet = null;
	//private float teleportTimer = 0f;

//Status
	[System.NonSerialized]
	public bool isThrusting = false;
	//public bool justTeleported = false;
	//public BoundaryDirection teleportDirection = BoundaryDirection.None;
	public bool shipIsActive = true;

	// Use this for initialization
	void Start () {
	
		//Get other objects
		scriptHumanInput = GetComponent<ScriptHumanInput>();
		scriptShipIntelligence = GetComponent<ScriptShipIntelligence>();
		scriptShipSheet = GetComponent<ScriptShipSheet>();
		shipModuleContainer = transform.FindChild ("ContainerModule");
		scriptModuleController = GameObject.Find ("ControllerGame").GetComponent<ScriptGameController> ();
		audioSource = GetComponent<AudioSource>();
		dynamicObjectsContainer = GameObject.Find ("ContainerDynamicObjects").transform;


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

		//Register starting modules, update to add joints if applicable
		ScriptModule[] iterationScripts = shipModuleContainer.GetComponentsInChildren<ScriptModule> ();
		foreach (ScriptModule module in iterationScripts) {
			module.moduleID = scriptModuleController.GetNextID();
			Vector3 vector3Position = module.transform.localPosition;
			Vector2 hotCoordinates = LocalPositionToNodeCoordinates(new Vector2(vector3Position.x, vector3Position.y));
			AddModule(module, gameObject, hotCoordinates);
			}
		//for (int i = 0; i < shipModuleContainer.childCount; i++) {
		//	GameObject hotMod = shipModuleContainer.GetChild(i).gameObject;
		//	Vector3 vector3Position = hotMod.transform.localPosition;
		//	Vector2 hotCoordinates = LocalPositionToNodeCoordinates(new Vector2(vector3Position.x, vector3Position.y));
		//	AddModule(hotMod.gameObject, hotCoordinates);
		//}

		//Set rigidbody velocity to zero
		//rigidbody2D.velocity = Vector2.zero;
		//rigidbody2D.angularVelocity = 0F;


	}
	
	// Update is called once per frame
	void Update()
	{
		ScriptModule[] iterationModules = shipModuleContainer.GetComponentsInChildren<ScriptModule>();
		foreach(ScriptModule module in iterationModules)
		{
			Node node = scriptShipSheet.GetNodeFromModule(module);
			module.transform.FindChild("LabelModule").GetComponent<TextMesh>().text = node.snakeIndex.ToString();
		}
		//if(Input.GetKeyDown("3")) 
	//	{
			//foreach(ScriptShipSheet ship in shipContainer.GetComponentsInChildren<ScriptShipSheet>())
			//{


		}

	void FixedUpdate () 
	{		

		if(shipIsActive)
		{
		//Temporary variables
		float thrustInput = 0;
		float turnInput = 0;

		//Assign input
		if (playerControl == PlayerControl.Human) {

			thrustInput = scriptHumanInput.thrustInput;
			turnInput = scriptHumanInput.turnInput;
			//Debug.Log ("Human");
		} else if (playerControl == PlayerControl.Computer) {
			thrustInput = scriptShipIntelligence.thrustInput;
			turnInput = scriptShipIntelligence.turnInput;
		} else {
			Debug.Log ("No control selected for " + this);
		}

			//Update position

		if(scriptModuleController.moduleRigidbodyMode)
		{
				//rigidCharacter = pilotModule.rigidbody2D;
			Vector2 pilotModuleForward = pilotModule.transform.TransformDirection(Vector2.up);
			//Debug.Log ("Pilot forward: " + pilotModuleForward);
			UpdateVelocity(thrustInput, turnInput, pilotModule.rigidbody2D, pilotModuleForward);
				//Debug.Log ("pilot rigidbody mode");
		} else {
			Vector2 upVector = Vector2.up;
			Vector2 shipForward = transform.TransformDirection (upVector);
			//Debug.Log ("Ship forward: " + shipForward + "Up: " + upVector + Time.frameCount);
			if(rigidbody2D)
			{
				//Debug.Log (thrustInput + turnInput + rigidbody2D.gameObject.name + forwardDirection + Time.frameCount);
				UpdateVelocity(thrustInput, turnInput, rigidbody2D, shipForward);
			//	Debug.Log ("Velocity Updated: " + thrustInput + ", " + turnInput);
			} else if(rigidbodyResetPending){
			//	Debug.Log ("Reset");
				gameObject.AddComponent<Rigidbody2D> ();
				rigidbodyResetPending = false;
				//Cache rigidbody
				Vector2 newVelocity = lastVelocity;
				rigidbody2D.mass = rigidbodyMass;
				rigidbody2D.drag = rigidbodyLinearDrag;
				rigidbody2D.angularDrag = rigidbodyAngularDrag;
				rigidbody2D.velocity = newVelocity;
			}

		}
			
			//Debug.Log (rigidCharacter.gameObject.name + hotForce);
			//Debug.Log ("Turn input: " + turnInput);
		//	hotRigid.angularVelocity = 90;
		//if(canThrust)
						if (!isThrusting && thrustInput == 1) {
								isThrusting = true;
								thrustEffect.enableEmission = true;
						} 
						if (isThrusting && thrustInput == 0) {
								isThrusting = false;
								thrustEffect.enableEmission = false;
						}
				//} else if(rigidbodyResetPending){
		


				
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

			//UpdateActivationStatus();

			foreach(Snake snake in scriptShipSheet.currentSnakes)
			{
		UpdateActivationAbility (snake);
			}


			/*
		if(teleportDirection != BoundaryDirection.None)
		{
			if(teleportTimer >= teleportDelay)
			{
				teleportDirection = BoundaryDirection.None;
			//	Debug.Log ("Teleported status none " + Time.time);
				teleportTimer = 0;
				//gameObject.layer = 0;
			} else {
			teleportTimer += Time.deltaTime;
			}
			}

*/

		}
	}


	//Should be migrated to module controller
	public void AddModule(ScriptModule addedModule, GameObject assimilatingObject, Vector2 nodeCoordinates)
	{
		//Debug.Log (addedModule.moduleID);
		//Temporary variables
		ScriptModule scriptModule = addedModule.GetComponent<ScriptModule> ();

		//Register non-starting modules
		if (addedModule.moduleType != ModuleType.Pilot) {
			//Set ship as parent
			addedModule.transform.parent = shipModuleContainer; //Make new module child to ship
			//Vector2 assimilatingModulePosition = assimilatingModule.transform.position;
		}

			//Log event
			scriptModule.ownTime = Time.frameCount;
			scriptModule.captureModule = assimilatingObject;

			//Update module
			scriptModule.moduleNodeCoordinates = nodeCoordinates; //Log coordinates to module
			addedModule.tag = "Ship"; //Tag as part of ship
			scriptModule.moduleOwner = this; //Mark this ship as new owner
			scriptModule.currentHP = scriptModule.maxHP; //Set starting HP

			//Verify module
			//Debug.Log ("Before call");
			if(!CoordinatesAreValid (addedModule))
			{
				//Eject module
				scriptModuleController.BreakModule(addedModule);
				//Debug.Log ("Coordinates are invalid.");
			} else {
			//Play sound effect
				//Debug.Log ("Coordinates are valid");
			audioSource.Play();
		


		//Mode handler
		if (scriptModuleController.moduleRigidbodyMode) 
		{
			if(addedModule.moduleType != ModuleType.Pilot)
			{

				//Positioning
				Vector2 localCoordinates = NodeCoordinatesToLocalPosition (nodeCoordinates);
				Vector2 worldCoordinates = pilotModule.transform.TransformDirection(localCoordinates); //Set position
				Debug.Log ("Local coordinates" + localCoordinates + "; World position: " + worldCoordinates);
				addedModule.transform.position = worldCoordinates + new Vector2(pilotModule.transform.position.x, pilotModule.transform.position.y);
				addedModule.transform.localRotation = Quaternion.identity; //Set rotation

				//Rigidbody
				DistanceJoint2D hotJoint = addedModule.gameObject.AddComponent<DistanceJoint2D>();
				hotJoint.connectedBody = assimilatingObject.rigidbody2D;
				hotJoint.collideConnected = true;
				hotJoint.distance = 2;
			//	hotJoint.anchor;
			//	hotJoint.connectedAnchor;
			}
		} else {

			//Positioning
			Vector2 localCoordinates = NodeCoordinatesToLocalPosition (nodeCoordinates);
			addedModule.transform.localPosition = localCoordinates; //Set position
			addedModule.transform.localRotation = Quaternion.identity; //Set rotation

			//Rigidbody
			if (addedModule.rigidbody2D) 
			{
				Destroy (addedModule.gameObject.rigidbody2D); 
			}
			//Ship rigidbody
			lastVelocity = rigidbody2D.velocity; //Cache rigidbody velocity
			Destroy(rigidbody2D); //Destroy rigidbody for replacement
			//Debug.Log ("Destroyed ship rigidbody");
			rigidbodyResetPending = true;
		}

		//Add to grid
		Vector2 gridNodeCoordinates = ScriptShipSheet.GetGridNodeCoordinates(nodeCoordinates);
			//int[] schematicCoordinates = new int[]{(int)gridNodeCoordinates.x, (int)gridNodeCoordinates.y};
			//(int)gridNodeCoordinates.x, (int)gridNodeCoordinates.y] = hotNode;
		
			scriptShipSheet.AddModuleToGrid(addedModule, gridNodeCoordinates);
				

		//Add to activation
		//UpdateActivationStatus();
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

	bool CoordinatesAreValid(ScriptModule hotMod)
	{
		//Throw error iff hotMod's coordinates match another child object of this ship's  module container
		ScriptModule[] iterationScripts = shipModuleContainer.GetComponentsInChildren<ScriptModule> ();
		foreach (ScriptModule otherMod in iterationScripts) {
			if(hotMod.moduleNodeCoordinates == otherMod.moduleNodeCoordinates && hotMod != otherMod)
			{
				Debug.LogError(hotMod.moduleID + "'s coordinates conflict with " + otherMod.moduleID + "'s.");
				return false;
			}
		}
		return true;
	}

	void UpdateVelocity(float thrustInput, float turnInput, Rigidbody2D hotRigid, Vector2 hotForward)
	{
		Vector2 hotForce = hotForward * thrustInput * thrustForceConstant * Time.fixedDeltaTime;
		hotRigid.AddForce(hotForce);
		hotRigid.angularVelocity = turnInput * -turnSpeedConstant;
		//Debug.Log (hotForce + " " + hotRigid.velocity);
	}

	void UpdateActivationAbility(Snake snake)
	{
	//	if(moduleType == ModuleType.Weapon && moduleOwner.target)
	//	{
		//	Vector2 attackVector = moduleOwner.target.transform.position - transform.position;
		if(pilotModule)
		{
			if(snake.isArmed){
			if(snake.moduleType == ModuleType.Weapon)
			{
				Vector2 attackVector = pilotModule.transform.TransformDirection (Vector2.up);
		//	if(attackVector.magnitude <= weaponRange)
			//{
				if(snake.canShoot)
				{
					//Debug.Log ("Can shoot");
					//Change state
					snake.shotTimer = 0;
					snake.canShoot = false;
					//Bullet creation
					Vector3 bulletPosition = pilotModule.transform.position;
					GameObject hotBullet = Instantiate (basicBullet, bulletPosition, transform.rotation) as GameObject;
					//Bullet registration
					hotBullet.transform.parent = dynamicObjectsContainer;
					ScriptProjectile scriptProjectile = hotBullet.GetComponent<ScriptProjectile>();
					scriptProjectile.owner = gameObject;

					//Assign properties
					scriptProjectile.projectileDamage = snake.damage;
					hotBullet.transform.localScale *= snake.bulletScale;
					scriptProjectile.bulletDuration = snake.durationInSeconds;

					//Add force
					hotBullet.rigidbody2D.velocity = rigidbody2D.velocity;	
					hotBullet.rigidbody2D.AddForce(attackVector * snake.shotForce); //Magic number

				} else {

					snake.shotTimer += Time.deltaTime;
					//Debug.Log (snake.shotTimer + " @"+Time.time);
					float shotDelay = 1 / snake.bulletsPerSecond;
					if(snake.shotTimer >= shotDelay)
					{
						snake.canShoot = true;
					}
				}
		} else if (snake.moduleType == ModuleType.Laser)
			{
				if(!snake.laserTriggerEnabled)
				{
					snake.laserTriggerEnabled = true;

						GameObject hotTrigger = Instantiate(laserTrigger) as GameObject;
						Vector3 triggerScale = new Vector3(snake.laserTriggerScale, snake.laserTriggerScale, 1);
						hotTrigger.transform.localScale = triggerScale;
						hotTrigger.transform.position = transform.position;
						//hotTrigger.transform.localScale.y = activation.laserTriggerScale;
						hotTrigger.transform.parent = triggerContainer.transform;
					//} else 
				//	{
				//		Debug.LogError("Invalid activation type for laser: " + snake.moduleType);
					}
				}
				}
			}
			}


//	void UpdateActivationStatus()
//	{
//		currentActivations = scriptShipSheet.GetActivations();
//		scriptShipSheet.ArmActivations(currentActivations);
//
//		foreach (ScriptModule module in shipModuleContainer.GetComponentsInChildren<ScriptModule>()) {
//			Node node = scriptShipSheet.GetNodeFromModule (module);
//			if (node.activationIndex == -1) {
//				module.SetActivation (false);
//			} else if (node.activationIndex >= 0) {
//				module.SetActivation (true);
//			} else {
//				Debug.Log ("Invalid activation id on " + node.module.name);
//			}
//		}
//	}
	


//	public void ClearOldLasers()
//	{
//		foreach(Transform child in triggerContainer.transform)
//		{
//			Destroy(child.gameObject);
//		}
//	}

}