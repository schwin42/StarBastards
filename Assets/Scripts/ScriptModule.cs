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
/*
public enum ProjectileType
{
	None,
	Beam, 
	Pulse, 
	Spread, 
	Bomb
}
*/
public class ScriptModule : MonoBehaviour {

	public string moduleName;
	public int moduleID;
	//[System.NonSerialized]
	public ScriptShipController moduleOwner = null; //Null indicates module is neutral
	public Vector2 moduleNodeCoordinates; //Location of owned module relative to pilot module
	//public Transform spaceController;
	//public GameObject explosionEffect;

	//Scriptable

	//Inspector objects
	public ScriptModuleController scriptModuleController;

	//Acquired objects
	//public Renderer renderer;

	public ModuleType moduleType = ModuleType.None;

	public int maxHP = 10;
	public int currentHP;

	//Weapon stats
	//public ProjectileType projectileType = ProjectileType.None;
	//public float weaponRange;
	//public int weaponDamage;
	//public float shotsPerSecond;
	//public float trackingSpeed;
	//public float projectileSpeed;
	//public float shotTimer;

	//Status
	//public bool canShoot = false;

	//Error-checking
	public int ownTime = -9999;
	public GameObject captureModule;
	//public Vector2 captureDirection;
	
	// Use this for initialization
	void Start () {

		//Acquire objects
		scriptModuleController = GameObject.Find ("ControllerSpace").GetComponent<ScriptModuleController>();
		//renderer = GetComponent<Renderer>();
		//material = GetComponent<Material>();

		if(tag == "Ship")
		{
			moduleOwner = transform.parent.parent.gameObject.GetComponent<ScriptShipController>();
		}

	//	if(moduleType == ModuleType.Pilot)
	//	{
			//renderer.material.color = moduleOwner.playerColor;
	//	}

	}
	
	// Update is called once per frame
	void Update () {
	if(moduleOwner != null)
		{

			if(currentHP <= 0)
			{
				if(moduleType == ModuleType.Pilot)
				{
					Destroy (moduleOwner.gameObject);
					ScriptModule[] iterationScripts = moduleOwner.shipModuleContainer.GetComponentsInChildren<ScriptModule>();
					foreach(ScriptModule hotMod in iterationScripts){
						scriptModuleController.RemoveModule(hotMod);
					}
				} else {
					scriptModuleController.RemoveModule(this);
				}
			}

			/*
			if(moduleType == ModuleType.Weapon && moduleOwner.target)
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

			*/
		}

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		//Debug.Log(gameObject.name);

		//Debug.Log ("Collided");
		if(moduleOwner != null)
		{
			if(collision.gameObject.tag == "NeutralModule")
			{

				//Calculate module location
				ContactPoint2D contact = collision.contacts[0];
				Vector2 normal = contact.normal;
				Vector2 inverseNormal = transform.InverseTransformDirection(normal);
				Vector2 roundNormal = RoundVector2(inverseNormal); 
				Vector2 nodeCoordinatesOffset = CalibrateCoordinates(roundNormal); //Further adjustments
				//Debug.Log ("Inverse normal: " + inverseNormal + " , Node coordinates offset: " + nodeCoordinatesOffset + ", Time: " + Time.time);
				Vector2 assimilationNodeCoordinates = moduleNodeCoordinates + nodeCoordinatesOffset;

				//Collect module
				moduleOwner.AddModule(collision.gameObject.GetComponent<ScriptModule>(), gameObject, assimilationNodeCoordinates);

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


	//Unadjusted offset -> adjusted offset
	Vector2 CalibrateCoordinates(Vector2 coordinates)
	{
		//Reverse direction for unknown yet necessary reason
		Vector2 hotVector = coordinates *= -1;
		if(Mathf.Abs (coordinates.x) + Mathf.Abs(coordinates.y) == 2)
		{
			ScriptShipSheet scriptShipSheet = moduleOwner.GetComponent<ScriptShipSheet>();
			Debug.Log (coordinates + " checked for adjacent");
			Vector2 prospectiveOffsetX = new Vector2(hotVector.x, 0F);
			Vector2 prospectiveCoordinatesX = moduleNodeCoordinates + prospectiveOffsetX;
			Vector2 prospectiveGridCoordinatesX = scriptShipSheet.GetGridNodeCoordinates(prospectiveCoordinatesX);
			if(scriptShipSheet.schematic[(int)prospectiveGridCoordinatesX.x, (int)prospectiveGridCoordinatesX.y].isEmpty)
			{
				hotVector = prospectiveOffsetX;
			} else {
				Vector2 prospectiveOffsetY = new Vector2(0F, hotVector.y);
				Vector2 prospectiveCoordinatesY = moduleNodeCoordinates + prospectiveOffsetY;
				Vector2 prospectiveGridCoordinatesY = scriptShipSheet.GetGridNodeCoordinates(prospectiveCoordinatesY);
				if(scriptShipSheet.schematic[(int)prospectiveGridCoordinatesY.x, (int)prospectiveGridCoordinatesY.y].isEmpty)
				{
					hotVector = prospectiveOffsetY;
				} else {
					Debug.LogError("No possible attach sites on " + coordinates);
				}
			}

		//	Vector2 prospectiveDirectionY = new Vector2(0F, hotVector.y);


			//if(Random.value > 0.5F)
			//{
			//	hotVector.x = 0;
			//} else {
			//	hotVector.y = 0;
			//}
		}
		
		//Vector2 hotterVector = new Vector2(-hotVector.y, hotVector.x); //Rotate 90 degrees

		//Debug.Log (coordinates + " " + hotVector);
		return hotVector;
	}





	//	//transform.localPosition = Vector2.zero;
	//	shipSpaceCoordinates = Vector3.zero;

	//	if(moduleType == ModuleType.Weapon)
	//	{
	//		canShoot = false;
	//	}
	



}
