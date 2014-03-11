using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerControl
{
	None,
	Human,
	Computer
}

public class ScriptModuleController : MonoBehaviour {

	//Configurable
	public bool moduleRigidbodyMode = true;
	public float startingForceConstant = 10;
	public float ejectionLinearForceConstant = 1000F;
	public float ejectionAngularForceConstant = 1000F;

	//Prefabs
	public GameObject augmentModule;
	public GameObject defenseModule;
	public GameObject weaponModule;

	//Transforms
	public Transform shipContainer;
	public Transform spaceContainer;

	//Objects
	private List<GameObject> ships = new List<GameObject>();


	private int nextModuleID = 0;
	// Use this for initialization

	//Debug
	public List<ScriptModule> pilotContiguousModules = new List<ScriptModule>();

	void Start () {

		//Get objects
		//shipContainer = GameObject.Find ("ContainerShip").transform;

		//Set ships
		foreach(Transform child in shipContainer)
		{
			ships.Add (child.gameObject);
		}
		//Debug.Log (ships.Count);



		//Generate neutral modules
		for(int i = 0; i<50; i++)
		{
			//GameObject hotMod = Instantiate (modulePrefab) as GameObject;


			GameObject hotMod = gameObject;
			float hotRand = Random.value * 3;
			if(hotRand <= 1)
			{
				 hotMod = Instantiate (augmentModule) as GameObject;
				//Debug.Log ("hull module");
			} else if(hotRand <= 2){
				 hotMod = Instantiate (weaponModule) as GameObject;
				//Debug.Log ("weapon module");
			} else if(hotRand <= 3){
				hotMod = Instantiate (defenseModule) as GameObject;
			} else {
				Debug.LogError ("Bug in random number generator.");
			}

			hotMod.transform.position = new Vector2(Random.value * 200 - 100, Random.value * 200 - 100);
			hotMod.transform.parent = this.gameObject.transform;
			ScriptModule scriptModule = hotMod.GetComponent<ScriptModule>();
			scriptModule.moduleID = GetNextID();
			hotMod.name = "Module" + scriptModule.moduleID;

			Vector2 normalizedRandomForce = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1); 
			hotMod.rigidbody2D.AddForce(normalizedRandomForce * (Random.value * startingForceConstant));
			//GetNextID(hotModS.GetComponent<ScriptModule>());
			//Debug.Log (i);
		}

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

		if(Input.GetKeyDown("3")) //GetSnakes unit test
		{
			foreach(ScriptShipSheet ship in shipContainer.GetComponentsInChildren<ScriptShipSheet>())
			{
				ship.lazySnakes = ship.GetModuleSnakes();
			}
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

	public void RemoveModule(ScriptModule hotMod)
	{
		//Cache ejection vector
		Vector2 pilotToEjectionVector = hotMod.gameObject.transform.position - hotMod.moduleOwner.pilotModule.transform.position;

		hotMod.moduleOwner = null;
		hotMod.transform.parent = spaceContainer;
		hotMod.tag = "NeutralModule";
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


	}


}
