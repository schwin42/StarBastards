using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptLaser : MonoBehaviour {

	//Inspector
	public LineRenderer laserRenderer;
	public GameObject explosionEffect;

	//Script constants
	public float damageDelay = 1;
	public float damageTimer = 0;
	public int damageAmount = 10;

	//State
	public List<GameObject> trackedTargets = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		bool willDealDamage = false;
		if(trackedTargets.Count > 0)
		{
			damageTimer += Time.deltaTime;
			if(damageTimer >= damageDelay)
			{
				willDealDamage = true;
				damageTimer = 0;
			}
		//	List<GameObject> hotList = new List<GameObject>(trackedTargets); //Assign temporary list for iteration
		//	foreach(GameObject target in hotList)
		//{
			//Debug.Log (transform.InverseTransformPoint(target.transform.position));
			GameObject target = trackedTargets[0];
			laserRenderer.SetPosition(1, transform.InverseTransformPoint( target.transform.position));
				if(willDealDamage)
				{
					ScriptModule scriptModule = target.GetComponent<ScriptModule>();
					scriptModule.currentHP -= damageAmount;
					Instantiate (explosionEffect, target.transform.position, Quaternion.identity);
					if(scriptModule.currentHP <= 0)
					{
						trackedTargets.Remove(target);
						laserRenderer.SetPosition(1, Vector3.zero);
					damageTimer = 0;
					}

				}
		//}
		} else {
			damageTimer = 0;
		}
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		//Debug.Log (collider.gameObject.name + " @" + Time.time);
		//Add target if none 
		if(collider.gameObject.tag == "Ship")
		{
			if(!trackedTargets.Contains (collider.gameObject))
			   {
				trackedTargets.Add (collider.gameObject);
			}

		//	if(activeTargets.Count == 0)
		//	{
		//	Debug.Log (collider.gameObject.name + " @ " + Time.time);
		//	activeTargets.Add (collider.gameObject);
		//	}
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		//Debug.Log (collider.gameObject.name + " @ " + Time.time);
		if(trackedTargets.Contains(collider.gameObject))
		   {
			int targetIndex = trackedTargets.IndexOf(collider.gameObject);
	//			collider.gameObject);
			if(targetIndex == 0)
			{
				damageTimer = 0;
				laserRenderer.SetPosition(1, Vector3.zero);
			}
		trackedTargets.Remove(collider.gameObject);
		//laserRenderer.SetPosition(1, Vector3.zero);
		}
	}
}
