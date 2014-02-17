using UnityEngine;
using System.Collections;

public class ScriptProjectile : MonoBehaviour {

	public int projectileDamage;
	public GameObject owner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		//Debug.Log (collider.gameObject.name);
		if(collider.gameObject.tag == "Ship" && collider.gameObject.transform.root.gameObject != owner)
		{
			//Debug.Log (collider.tag + collider.gameObject.transform.parent.gameObject.name + owner);
		//	Debug.Log ("Ship Location");
			collider.gameObject.GetComponent<ScriptModule>().hitPoints -= projectileDamage;
			Destroy (gameObject);
		}
	}
}
