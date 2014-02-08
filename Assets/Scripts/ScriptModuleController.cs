using UnityEngine;
using System.Collections;

public class ScriptModuleController : MonoBehaviour {

	public GameObject hullModule;
	public GameObject weaponModule;

	// Use this for initialization
	void Start () {
		for(int i = 0; i<25; i++)
		{
			//GameObject hotMod = Instantiate (modulePrefab) as GameObject;


			GameObject hotMod = gameObject;
			if(Random.value * 5 <= 4.5)
			{
				 hotMod = Instantiate (hullModule) as GameObject;
				Debug.Log ("hull module");
			} else {
				 hotMod = Instantiate (weaponModule) as GameObject;
				Debug.Log ("weapon module");
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
