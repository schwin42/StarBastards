using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AgentType
{
	None,
	Human,
	ScriptedAi,
	ModelAi,
}

public enum ControlType
{
	None,
	Keyboard,
	Touch,
	Gamepad
}

public class GameController : MonoBehaviour
{
	
	public static GameController Instance;

	//Configurable static Mode
	public ControlType humanInputModality;

	//Configuration

	//Session settings
	public List<AgentType> players;
	public int startingModules;
	public int moduleSpawnSpacing;
	public Vector2 moduleSpawnOffset;

	//Gameplay constants
	public float startingForceConstant = 10;
	public float EJECTION_LINEAR_FORCE = 1000F;
	public float EJECTION_ANGULAR_FORCE = 1000F;
	public bool moduleRigidbodyMode = false;

	//Appearance settings
	public Color inactiveGunColor;
	public Color activeGunColor;
	public Color inactiveArmorColor;
	public Color activeArmorColor;
	public Color inactiveLaserColor;
	public Color activeLaserColor;

	//Prefabs
	public GameObject shipPrefab;
	public GameObject modulePrefab;

	//Game state
	private List<ShipController> activeShips = new List<ShipController>();
	//TODO Modules?

	//Bookkeeping
	private int nextModuleID = 0;
	private Transform shipContainer;

	private Transform _moduleContainer;

	public Transform ModuleContainer { get { return _moduleContainer; } }

	//Debug
	//public List<ScriptModule> pilotContiguousModules = new List<ScriptModule>();

	void Awake() {
		Instance = this;
	}

	void Start() {

		//Acquire gameObjects from scene
		shipContainer = GameObject.Find("Ships").transform;
		_moduleContainer = GameObject.Find("Modules").transform;

		//Set ships
		for (int i = 0; i < players.Count; i++) {
			//Instantiate prefab
			GameObject shipGo = Instantiate(shipPrefab);

			//Register ship
			ShipController shipController = shipGo.GetComponent<ShipController>();
			activeShips.Add(shipController);

			//Set pilot type
			shipController.SetPilotType(players[i]);

			//Position ships
		}

//		foreach(Transform child in shipContainer)
//		{
//			activeShips.Add (child.gameObject);
//		}

		//Generate neutral modules
		SpawnModules(startingModules);

		SetModuleRigidbodies(moduleRigidbodyMode);
	}

	void Update() {
//		if(Input.GetKeyDown("1"))
//		{
//			foreach(ScriptShipSheet ship in shipContainer.GetComponentsInChildren<ScriptShipSheet>())
//			ship.GridStatus();
//		}
//		
//		if(Input.GetKeyDown("2"))
//		{
//			foreach(ScriptShipSheet ship in shipContainer.GetComponentsInChildren<ScriptShipSheet>())
//				pilotContiguousModules = ship.GetModulesContiguousToPilot();
//		}

	}

	public static int GetNextID() {
		int returnID = Instance.nextModuleID;
		Instance.nextModuleID++;
		return returnID;
	}

	void SetModuleRigidbodies(bool value) {
		//Remove ship rigidbodies
		foreach (ShipController ship in activeShips) {
			if (value) {
				if (ship.GetComponent<Rigidbody2D>()) {
					Debug.Log("Destroyed" + ship.name + "'s rigidbody2D");
					Destroy(ship.GetComponent<Rigidbody2D>());
				}
			} else {
				ShipController scriptShipController = ship.GetComponent<ShipController>();
				Destroy( scriptShipController.PilotRigidbody);
			}
		}
		//Change code to not remove module rigidbodies on collect 
		//Add joints connecting the modules on collect

	}

	public void BreakModule(Module module) {
		//Cache ship values
		Vector2 pilotToEjectionVector = Vector2.zero;
		Debug.Log(module.moduleID + "'s type is " + module.moduleType);
		//if(hotMod.moduleType != ModuleType.Pilot)
		//{
		pilotToEjectionVector = module.gameObject.transform.position - module.moduleOwner.PilotModule.transform.position;

		ShipController previousOwner = module.moduleOwner;

		module.moduleOwner = null;
		module.transform.parent = _moduleContainer;
		module.tag = "Debris";

		//Jettison module
		JettisonModule(module, pilotToEjectionVector);

		ScriptShipSheet previousShipSheet = previousOwner.GetComponent<ScriptShipSheet>();

		previousShipSheet.RemoveModuleFromSnake(module);
		previousShipSheet.RemoveModuleFromGrid(module);

		//Jettison unconnected modules
		Module[] iterationScripts = previousOwner.shipModuleContainer.GetComponentsInChildren<Module>();
		if (module.moduleType != ModuleType.Pilot) {
			Debug.Log("Before removing disconnected.");
			List<Module> connectedModules = previousShipSheet.GetModulesContiguousToPilot();
			Debug.Log("Connected modules: " + connectedModules.Count);
		
			Debug.Log("ship modules: " + iterationScripts.Length);
			foreach (Module shipModule in iterationScripts) { //TODO Same code is repeated below!!
				if (!connectedModules.Contains(shipModule)) {
					Vector2 pilotToDisconnectedVector = shipModule.gameObject.transform.position - previousOwner.PilotModule.transform.position;
					shipModule.moduleOwner = null;
					shipModule.tag = "NeutralModule";
					JettisonModule(shipModule, pilotToDisconnectedVector);
					previousShipSheet.RemoveModuleFromSnake(shipModule);
					previousShipSheet.RemoveModuleFromGrid(shipModule);
				}
			}
		} else {
			foreach (Module shipModule in iterationScripts) {
				Vector2 pilotToDisconnectedVector = shipModule.gameObject.transform.position - previousOwner.PilotModule.transform.position;
				shipModule.moduleOwner = null;
				shipModule.tag = "NeutralModule";
				JettisonModule(shipModule, pilotToDisconnectedVector);
				previousShipSheet.RemoveModuleFromSnake(shipModule);
				previousShipSheet.RemoveModuleFromGrid(shipModule);
			}
		}
		previousShipSheet.DamageSnakeAtModule(module);
		//Remove module from activation
		//previousOwner.SendMessage("UpdateActivationStatus");
	}

	public static T GetRandomEnum<T>() {
		System.Array hotArray = System.Enum.GetValues(typeof(T));
		T value = (T)hotArray.GetValue(UnityEngine.Random.Range(0, hotArray.Length));
		return value;
	}

	void SpawnModules(int amount) {
		for (int i = 1; i <= amount; i++) {
			
			GameObject module = Instantiate(modulePrefab) as GameObject;

			float xPosition = moduleSpawnOffset.x + (i % Mathf.Sqrt(i)) * moduleSpawnSpacing;
			float yPosition = moduleSpawnOffset.y + Mathf.Floor(i / Mathf.Sqrt(i)) * moduleSpawnSpacing;
			print("X, y," + xPosition + ", " + yPosition);
			module.transform.position = new Vector2(xPosition, yPosition);
//			module.transform.position = new Vector2(Random.value * 200 - 100, Random.value * 200 - 100);
			module.transform.parent = this.gameObject.transform;
			Module scriptModule = module.GetComponent<Module>();
			scriptModule.moduleID = GetNextID();
			module.name = "Module" + scriptModule.moduleID;
			
			Vector2 normalizedRandomForce = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1); 
//			hotMod.GetComponent<Rigidbody2D>().AddForce(normalizedRandomForce * (Random.value * startingForceConstant));
			//GetNextID(hotModS.GetComponent<ScriptModule>());
			//Debug.Log (i);
			
			//Type
			float hotRand = Random.value * 3;
			//Color defaultColor = Color.black;
			//Color activeColor = Color.black;
			if (hotRand <= 1) {
				scriptModule.moduleType = ModuleType.Weapon;
				scriptModule.defaultColor = inactiveGunColor;
				scriptModule.activatedColor = activeGunColor;
			} else if (hotRand <= 2) {
				scriptModule.moduleType = ModuleType.Armor;
				scriptModule.defaultColor = inactiveArmorColor;
				scriptModule.activatedColor = activeArmorColor;
			} else if (hotRand <= 3) {
				scriptModule.moduleType = ModuleType.Laser;
				scriptModule.defaultColor = inactiveLaserColor;
				scriptModule.activatedColor = activeLaserColor;
			} else {
				Debug.LogError("Bug in random number generator.");
			}
			
			//Subtype
			scriptModule.moduleSubtype = GetRandomEnum<ModuleSubtype>();
			string enumString = scriptModule.moduleSubtype.ToString();
			scriptModule.textMesh.text = enumString[0].ToString();
		}
	}

	void JettisonModule(Module hotMod, Vector2 ejectionVector) {
		hotMod.gameObject.AddComponent<Rigidbody2D>();
		//Set rigidbody properties...
		
		//Add rigidbody velocity
		//Vector2 ejectionLinearForce = new Vector2((Random.value -0.5F) * ejectionLinearForceConstant, (Random.value -0.5F) * ejectionLinearForceConstant);
		Vector2 ejectionLinearForce = ejectionVector * EJECTION_LINEAR_FORCE;
		float ejectionAngularForce = (Random.value - 0.5F) * EJECTION_ANGULAR_FORCE;
		hotMod.GetComponent<Rigidbody2D>().AddForce(ejectionLinearForce);
		hotMod.GetComponent<Rigidbody2D>().AddTorque(ejectionAngularForce);
	}
}
