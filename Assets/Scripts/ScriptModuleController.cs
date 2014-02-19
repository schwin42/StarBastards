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

	//Prefabs
	public GameObject augmentModule;
	public GameObject defenseModule;
	public GameObject weaponModule;

	//Objects
	public Transform shipContainer;
	private List<GameObject> ships = new List<GameObject>();

	private int nextModuleID = 0;
	// Use this for initialization
	void Start () {

		//Get objects
		shipContainer = GameObject.Find ("ContainerShip").transform;

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
				Debug.LogError ("Random leak.");
			}

			hotMod.transform.position = new Vector2(Random.value * 100 - 50, Random.value * 100);
			hotMod.transform.parent = this.gameObject.transform;
			ScriptModule scriptModule = hotMod.GetComponent<ScriptModule>();
			scriptModule.moduleID = GetNextID();
			hotMod.name = "Module" + scriptModule.moduleID;
			//GetNextID(hotModS.GetComponent<ScriptModule>());
			//Debug.Log (i);
		}

		SetModuleRigidbodies (moduleRigidbodyMode);
	}
	
	// Update is called once per frame
	void Update () {
	
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


}
