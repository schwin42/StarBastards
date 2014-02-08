using UnityEngine;
using System.Collections;

public class ScriptModuleController : MonoBehaviour {

	public GameObject augmentModule;
	public GameObject defenseModule;
	public GameObject weaponModule;

	// Use this for initialization
	void Start () {
		for(int i = 0; i<25; i++)
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

			hotMod.transform.position = new Vector2(Random.value * 60 - 30, Random.value * 40 - 20);
			hotMod.transform.parent = this.gameObject.transform;
			//Debug.Log (i);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
