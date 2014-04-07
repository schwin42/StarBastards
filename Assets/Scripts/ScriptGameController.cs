using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerControl
{
	None,
	Human,
	Computer
}

public enum ControlType
{
	None,
	Keyboard,
	Touch,
	Gamepad
}

public class ScriptGameController : MonoBehaviour {

	//Configurable static Mode
	public ControlType controlType;

	//Configurable
	public int startingModules = 200;
	public bool moduleRigidbodyMode = true;
	public float startingForceConstant = 10;
	public float ejectionLinearForceConstant = 1000F;
	public float ejectionAngularForceConstant = 1000F;
	public Color inactiveGunColor;
	public Color activeGunColor;
	public Color inactiveArmorColor;
	public Color activeArmorColor;
	public Color inactiveLaserColor;
	public Color activeLaserColor;

	//Prefabs
	//public GameObject augmentModule;
	//public GameObject defenseModule;
	public GameObject modulePrefab;

	//Transforms
	public Transform shipContainer;
	public Transform spaceContainer;

	//Objects
	private List<GameObject> ships = new List<GameObject>();
	protected ScriptInterfaceController scriptInterfaceController;


	private int nextModuleID = 0;
	// Use this for initialization

	//Debug
	public List<ScriptModule> pilotContiguousModules = new List<ScriptModule>();

	void Start () {

		scriptInterfaceController = GameObject.Find ("RootInterface").GetComponent<ScriptInterfaceController>();
		scriptInterfaceController.SetInterface(controlType);

		//Get objects
		//shipContainer = GameObject.Find ("ContainerShip").transform;

		//Set ships
		foreach(Transform child in shipContainer)
		{
			ships.Add (child.gameObject);
		}
		//Debug.Log (ships.Count);



		//Generate neutral modules
		SpawnModules(startingModules);
	

		SetModuleRigidbodies (moduleRigidbodyMode);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("1"))
		{
			foreach(ScriptShipSheet ship in shipContainer.GetComponentsInChildren<ScriptShipSheet>())
			ship.GridStatus();
		}
		
		if(Input.GetKeyDown("2"))
		{
			foreach(ScriptShipSheet ship in shipContainer.GetComponentsInChildren<ScriptShipSheet>())
				pilotContiguousModules = ship.GetModulesContiguousToPilot();
		}

	}

	public int GetNextID()
	{
		int returnID = nextModuleID;
		nextModuleID++;
		return returnID;
	}

	void SetModuleRigidbodies(bool value)
	{
		//Remove ship rigidbodies
		foreach (GameObject ship in ships) {
			if(value)
			{
			if(ship.rigidbody2D){
				Debug.Log ("Destroyed" + ship.name + "'s rigidbody2D");
				Destroy(ship.rigidbody2D);
				}
			} else {
				ScriptShipController scriptShipController = ship.GetComponent<ScriptShipController>();
				GameObject targetModule = scriptShipController.pilotModule;

				Destroy(targetModule.rigidbody2D);
			}
				}
		//Change code to not remove module rigidbodies on collect 
		//Add joints connecting the modules on collect

	}

	public void BreakModule(ScriptModule hotMod)
	{
		//Cache ejection vector
		Vector2 pilotToEjectionVector = hotMod.gameObject.transform.position - hotMod.moduleOwner.pilotModule.transform.position;
		GameObject previousOwner = hotMod.moduleOwner.gameObject;
		hotMod.moduleOwner = null;
		hotMod.transform.parent = spaceContainer;
		hotMod.tag = "Debris";
		//if(hotMod.moduleType == ModuleType.Weapon)
		//{
		//	hotMod.canShoot = false;
		//}
		
		hotMod.gameObject.AddComponent<Rigidbody2D>();
		//Set rigidbody properties...

		//Add rigidbody velocity
		//Vector2 ejectionLinearForce = new Vector2((Random.value -0.5F) * ejectionLinearForceConstant, (Random.value -0.5F) * ejectionLinearForceConstant);
		Vector2 ejectionLinearForce = pilotToEjectionVector * ejectionLinearForceConstant;
		float ejectionAngularForce = (Random.value - 0.5F) * ejectionAngularForceConstant;
		hotMod.rigidbody2D.AddForce(ejectionLinearForce);
		hotMod.rigidbody2D.AddTorque(ejectionAngularForce);

		//Remove module from grid
		Vector2 gridNodeCoordinates = ScriptShipSheet.GetGridNodeCoordinates(hotMod.moduleNodeCoordinates);
		previousOwner.GetComponent<ScriptShipSheet>().schematic[(int)gridNodeCoordinates.x, (int)gridNodeCoordinates.y] = new Node();

		//Remove module from activation
		//previousOwner.SendMessage("UpdateActivationStatus");
	}

	public static T GetRandomEnum<T>()
	{
		System.Array hotArray = System.Enum.GetValues(typeof(T));
		T value = (T)hotArray.GetValue(UnityEngine.Random.Range (0, hotArray.Length));
		return value;
	}

	void SpawnModules(int amount)
	{
		for(int i = 0; i<amount; i++)
		{
			
			GameObject hotMod = Instantiate (modulePrefab) as GameObject;
			
			hotMod.transform.position = new Vector2(Random.value * 200 - 100, Random.value * 200 - 100);
			hotMod.transform.parent = this.gameObject.transform;
			ScriptModule scriptModule = hotMod.GetComponent<ScriptModule>();
			scriptModule.moduleID = GetNextID();
			hotMod.name = "Module" + scriptModule.moduleID;
			
			Vector2 normalizedRandomForce = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1); 
			hotMod.rigidbody2D.AddForce(normalizedRandomForce * (Random.value * startingForceConstant));
			//GetNextID(hotModS.GetComponent<ScriptModule>());
			//Debug.Log (i);
			
			//Type
			float hotRand = Random.value * 3;
			//Color defaultColor = Color.black;
			//Color activeColor = Color.black;
			if(hotRand <= 1)
			{
				scriptModule.moduleType = ModuleType.Weapon;
				scriptModule.defaultColor = inactiveGunColor;
				scriptModule.activatedColor = activeGunColor;
			} else if(hotRand <= 2){
				scriptModule.moduleType = ModuleType.Armor;
				scriptModule.defaultColor = inactiveArmorColor;
				scriptModule.activatedColor = activeArmorColor;
			} else if(hotRand <= 3){
				scriptModule.moduleType = ModuleType.Laser;
				scriptModule.defaultColor = inactiveLaserColor;
				scriptModule.activatedColor = activeLaserColor;
			} else {
				Debug.LogError ("Bug in random number generator.");
			}
			
			//Subtype
			scriptModule.moduleSubtype = GetRandomEnum<ModuleSubtype>();
			string enumString = scriptModule.moduleSubtype.ToString();
			scriptModule.textMesh.text = enumString[0].ToString();
		}
	}

}
