using UnityEngine;
using System.Collections;

public enum PlayerControl
{
	None,
	Human,
	Computer
}

public class ScriptModuleController : MonoBehaviour {

	//Prefabs
	public GameObject augmentModule;
	public GameObject defenseModule;
	public GameObject weaponModule;

	private int nextModuleID = 0;
	// Use this for initialization
	void Start () {
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
			//GetNextID(hotMod.GetComponent<ScriptModule>());
			//Debug.Log (i);
		}
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
}
