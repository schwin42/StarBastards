using UnityEngine;
using System.Collections;

public enum ModuleType
{
	None,
	Weapon,
	Assist,
	Defense,
	Pilot
}

public enum ModuleSubtype
{
	None,
	Power,
	Range,
	Number,

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
	public int moduleID;
	//[System.NonSerialized]
	public ScriptShipController moduleOwner = null; //Null indicates module is neutral
	public Vector2 moduleNodeCoordinates; //Location of owned module relative to pilot module

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

	//Error-checking
	public int ownTime = -9999;
	public GameObject captureModule;
	
	// Use this for initialization
	void Start () {
		if(tag == "Ship")
		{
			moduleOwner = transform.parent.parent.gameObject.GetComponent<ScriptShipController>();
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
				Vector2 nodeCoordinatesOffset = CalibrateCoordinates(roundNormal); //Further adjustments
				Vector2 assimilationNodeCoordinates = moduleNodeCoordinates + nodeCoordinatesOffset;
				//Debug.Log ("Normal: " + normal + "; Inverse Normal: " + inverseNormal + "; Round Normal: " + roundNormal
				  //         + "Node Coordinates Offset: " + nodeCoordinatesOffset + "; Assimilation Node Coordinates: " + assimilationNodeCoordinates
				    //       + ".");

			//Debug.Log ("Hit neutral");
				moduleOwner.AddModule(collision.gameObject.GetComponent<ScriptModule>(), gameObject, assimilationNodeCoordinates);
				//collision.gameObject.GetComponent<ScriptModule>().SetOwner (gameObject, assimilationCoordinates);

			}
		} 
		else {
			//If unowned, physics collision as normal or wait for ship module to collect
		}


	}


				/*
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
*/

	Vector2 RoundVector2(Vector2 unroundedVector)
	{
		int x = (int)Mathf.Round (unroundedVector.x);
		int y = (int)Mathf.Round (unroundedVector.y);
		//int z = Mathf.Round (hotVector.z);
		Vector2 roundVector = new Vector2(x, y);
		return roundVector;
	}



	Vector2 CalibrateCoordinates(Vector2 coordinates)
	{
		//Once traversable grid is ready, require check for possible directions (of the two) that is empty
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
		hotVector *= -1;
		//Vector2 hotterVector = new Vector2(-hotVector.y, hotVector.x); //Rotate 90 degrees

		//Debug.Log (coordinates + " " + hotVector);
		return hotVector;
	}

}
