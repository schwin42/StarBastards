﻿using UnityEngine;
using System.Collections;

public enum ModuleType
{
	None,
	Weapon,
	Augment,
	Defense,
	Pilot
}

public enum ProjectileType
{
	None,
	Beam, 
	Pulse, 
	Spread, 
	Bomb
}

public class ScriptModule : MonoBehaviour {

	public string moduleName;
	public ScriptShipController moduleOwner = null; //Null indicates module is neutral
	public Vector2 shipSpaceCoordinates; //Location of owned module relative to pilot module

	public ModuleType moduleType = ModuleType.None;

	public int hitPoints;

	//Weapon stats
	public ProjectileType projectileType = ProjectileType.None;
	public float weaponRange;
	public int weaponDamage;
	public float shotsPerSecond;
	public float trackingSpeed;
	public float projectileSpeed;
	public float shotTimer;

	//Status
	public bool canShoot = false;
	
	// Use this for initialization
	void Start () {
		if(transform.parent)
		{
			moduleOwner = transform.parent.gameObject.GetComponent<ScriptShipController>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	if(moduleOwner != null)
		{

			if(hitPoints <= 0)
			{
				if(moduleType == ModuleType.Pilot)
				{
					Destroy (gameObject);
				} else {
					Destroy (gameObject);
				}
			}

			if(moduleType == ModuleType.Weapon)
			{
				Vector2 attackVector = moduleOwner.target.transform.position - transform.position;
				if(attackVector.magnitude <= weaponRange)
				{
					if(canShoot)
					{
						canShoot = false;
					GameObject hotBullet = Instantiate (moduleOwner.basicBullet, transform.position, transform.rotation) as GameObject;
						ScriptProjectile scriptProjectile = hotBullet.GetComponent<ScriptProjectile>();
						scriptProjectile.projectileDamage = weaponDamage;
						scriptProjectile.owner = moduleOwner.gameObject;
						hotBullet.rigidbody2D.AddForce(attackVector * 100); //Magic number
						shotTimer = 0;
					} else {
						shotTimer += Time.deltaTime;
						if(shotTimer >= 1/shotsPerSecond)
						{
							canShoot = true;
						}
					}
				} else {
					if(!canShoot)
					{
						canShoot = true;
					}
				}
			}
		}

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log(gameObject.name);

		//Debug.Log ("Collided");
		if(moduleOwner != null)
		{
			if(collision.gameObject.tag == "NeutralModule")
			{
				ContactPoint2D contact = collision.contacts[0];
				Vector2 normal = contact.normal;
				Vector2 inverseNormal = transform.InverseTransformDirection(normal);
				Vector2 roundNormal = RoundVector2(inverseNormal);
				Vector2 coordinatesOffset = RoundNormalToCoordinates(IgnoreCorners(roundNormal));
				Vector2 assimilationCoordinates = shipSpaceCoordinates + coordinatesOffset;

			//Debug.Log ("Hit neutral");
				collision.gameObject.GetComponent<ScriptModule>().SetOwner (gameObject, assimilationCoordinates);

			}
		} 
		else {
			//If unowned, physics collision as normal or wait for ship module to collect
		}


	}

	void RemoveOwner()
	{
		gameObject.AddComponent<Rigidbody2D>();
		moduleOwner = null;
		transform.parent = null;
		//transform.localPosition = Vector2.zero;
		shipSpaceCoordinates = Vector3.zero;
		tag = "NeutralModule";
		if(moduleType == ModuleType.Weapon)
		{
			canShoot = false;
		}

	}

	void SetOwner(GameObject assimilatingModule, Vector2 coordinates)
	{
		Destroy (rigidbody2D);
		moduleOwner = assimilatingModule.transform.parent.gameObject.GetComponent<ScriptShipController>();
		Vector2 lastVelocity = moduleOwner.rigidbody2D.velocity;
		Destroy(moduleOwner.rigidbody2D);
		transform.parent = moduleOwner.transform;
		//Vector2 assimilatingModulePosition = assimilatingModule.transform.position;
		transform.localPosition = coordinates;
		transform.localRotation = Quaternion.identity;
		shipSpaceCoordinates = coordinates;
		tag = "Ship";
		StartCoroutine(ResetShipRigidbody(lastVelocity));
		if(moduleType == ModuleType.Weapon)
		{
		canShoot = true;
		}

		//Debug.Log ("assmodpos" + assimilatingModulePosition + "coordinates" + coordinates);
	}

	IEnumerator ResetShipRigidbody(Vector2 lastVelocity)
	{
		yield return 0;
		GameObject ship = transform.parent.gameObject;
		transform.parent.gameObject.AddComponent<Rigidbody2D>();



		//Recalculate mass and update velocity appropriately

		Vector2 newVelocity = lastVelocity;

			moduleOwner.rigidbodyMass ++; //magic number
		ship.rigidbody2D.mass = moduleOwner.rigidbodyMass;
		ship.rigidbody2D.drag = moduleOwner.rigidbodyLinearDrag;
		ship.rigidbody2D.angularDrag = moduleOwner.rigidbodyAngularDrag;
		transform.parent.rigidbody2D.velocity = newVelocity;

	}

	Vector2 RoundVector2(Vector2 unroundedVector)
	{
		int x = (int)Mathf.Round (unroundedVector.x);
		int y = (int)Mathf.Round (unroundedVector.y);
		//int z = Mathf.Round (hotVector.z);
		Vector2 roundVector = new Vector2(x, y);
		return roundVector;
	}

	Vector2 RoundNormalToCoordinates(Vector2 roundNormal)
	{
		return -roundNormal * 2;
	}

	Vector2 IgnoreCorners(Vector2 coordinates)
	{

		Vector2 hotVector = coordinates;
		if(Mathf.Abs (coordinates.x) + Mathf.Abs(coordinates.y) == 2)
		{
			if(Random.value > 0.5F)
			{
				hotVector.x = 0;
			} else {
				hotVector.y = 0;
			}
		}
		Debug.Log (coordinates + " " + hotVector);
		return hotVector;
	}

}