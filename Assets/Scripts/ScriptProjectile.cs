using UnityEngine;
using System.Collections;

public class ScriptProjectile : MonoBehaviour {

	public int projectileDamage;
	public GameObject owner;
	public GameObject explosionEffect;
	public float bulletDuration;
	public float bulletTimer = 0;

	// Use this for initialization
	void Start () {
	


	}
	
	// Update is called once per frame
	void Update () {
	
		bulletTimer += Time.deltaTime;

		if(bulletTimer >= bulletDuration)
		{
			Destroy (gameObject);
		}


		//if (!renderer.isVisible) {
		//	Destroy(gameObject);
		//		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{

		//Debug.Log (collider.gameObject.name + " " + Time.time);
		if(collider.gameObject.tag == "Ship")
		{
			if(collider.gameObject.transform.parent.parent.gameObject != owner)
		{
		//Debug.Log (collider.tag + collider.gameObject.transform.parent.gameObject.name + owner);
		//	Debug.Log ("Ship Location"
			collider.gameObject.GetComponent<ScriptModule>().currentHP -= projectileDamage;
			//Explosion effect
			Instantiate(explosionEffect, transform.position, Quaternion.identity);
			//Destroy self
			Destroy (gameObject);
		}
		} else if(collider.gameObject.tag == "Base")
		{
			if(collider.gameObject != owner){
			ScriptBaseController scriptBaseController = collider.gameObject.GetComponent<ScriptBaseController>();
			scriptBaseController.currentHP -= projectileDamage;
			Instantiate(explosionEffect, new Vector3(transform.position.x, transform.position.y, transform.position.z -3), Quaternion.identity);
			Destroy (gameObject);
			}
		} else {
			//Do nothing
		}
	}
}
