using UnityEngine;
using System.Collections;

public enum ModuleType
{
	None,
	Weapon,
	Laser,
	Armor,
	Pilot
}

public enum ModuleSubtype
{
	//Homing,
	Radius,
	Power,
	Duration,
	Number,
	Speed
}



public class Module : MonoBehaviour {
	

	//Properties
	public ShipController moduleOwner { get; set; } //Null indicates module is neutral
	public Vector2 moduleNodeCoordinates; //Location of owned module relative to pilot module
	public ModuleType moduleType = ModuleType.None;
	public ModuleSubtype moduleSubtype;

	//State
	public int moduleID;
	public bool isActivated = false;
	//public bool isArmed = false;

	//[System.NonSerialized] 
	public Color defaultColor;
	//[System.NonSerialized] 
	public Color activatedColor;

	//Scriptable

	//Inspector objects

	public TextMesh textMesh;
	public SpriteRenderer spriteRenderer;

	//Prefabs
	public GameObject explosionEffect;
	
	public int maxHP = 10;
	public int currentHP;
	
	//Error-checking
	public int ownTime = -9999;
	public GameObject captureModule;
	//public Vector2 captureDirection;
	
	// Use this for initialization
	void Start () {

		//moduleBox = transform.FindChild("BoxModule").gameObject;

		currentHP = maxHP;

		//Acquire objects
		//spriteRenderer = moduleBox.GetComponent<SpriteRenderer> ();
		if(tag == "Ship")
		{
			moduleOwner = transform.parent.parent.gameObject.GetComponent<ShipController>();
		}

		//Colors
		spriteRenderer.color = defaultColor;


	}
	
	// Update is called once per frame
	void Update () {
	if(moduleOwner != null)
		{

			if(currentHP <= 0)
			{
				GameController.Instance.BreakModule(this); //TODO Migrate to ship controller?

			}
		}

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		//Debug.Log(gameObject.name);
		//Debug.Log (collision.gameObject.name);
		//Debug.Log ("Collided");
		if(moduleOwner != null) //If A. unowned
		{

			if(collision.gameObject.tag == "NeutralModule") //And B. other is unowned
			{
				//Stick together


				//Calculate module location
				ContactPoint2D contact = collision.contacts[0];
				Vector2 normal = contact.normal;
				Vector2 inverseNormal = transform.InverseTransformDirection(normal);
				Vector2 roundNormal = RoundVector2(inverseNormal); 
				Vector2 nodeCoordinatesOffset = CalibrateCoordinates(roundNormal); //Further adjustments
				//Debug.Log ("Inverse normal: " + inverseNormal + " , Node coordinates offset: " + nodeCoordinatesOffset + ", Time: " + Time.time);
				Vector2 assimilationNodeCoordinates = moduleNodeCoordinates + nodeCoordinatesOffset;

				//Collect module
				moduleOwner.AddModule(collision.gameObject.GetComponent<Module>(), gameObject, assimilationNodeCoordinates);

			} else if(collision.gameObject.tag == "Ship" 
			          //&& collision.rigidbody == null
			          )
			{

				//Debug.Log ("Self: " + gameObject.name + " , Collided object: " + collision.gameObject.name + ", Collider: " + collision.collider.name + " @" + Time.time);


				Module colMod = collision.collider.GetComponent<Module>();
				//Debug.Log (colMod.name);
				if(colMod != null)
				{
				if(colMod.moduleOwner == moduleOwner)
				{
			//		Debug.Log(moduleOwner.name + " collided with self.");
				} else {
					if(moduleType == ModuleType.Armor && isActivated) //If this module is activated armor
					{
						Debug.Log (gameObject.name + " deals damage to " + colMod.gameObject);
						colMod.currentHP -= maxHP;
						Instantiate(explosionEffect, colMod.transform.position, Quaternion.identity);
					} else { //If this module  is not activated armor
						if(colMod.moduleType == ModuleType.Armor && colMod.isActivated) //Inactive against active armor
						{
							//No effect
						} else { //Inactive against inactive
							colMod.currentHP -= maxHP;
							Instantiate(explosionEffect, colMod.transform.position, Quaternion.identity);
						}
					}
				}
			}

			}
		} 
		else {
			//If unowned, physics collision as normal or wait for ship module to collect
		}


	}

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
			Debug.Log (coordinates + " checked for adjacent at " + Time.time);
			Vector2 prospectiveOffsetX = new Vector2(hotVector.x, 0F);
			Vector2 prospectiveCoordinatesX = moduleNodeCoordinates + prospectiveOffsetX;
			Vector2 prospectiveGridCoordinatesX = ScriptShipSheet.GetGridNodeCoordinates(prospectiveCoordinatesX);
			if(scriptShipSheet.schematic[(int)prospectiveGridCoordinatesX.x, (int)prospectiveGridCoordinatesX.y].isEmpty)
			{
				hotVector = prospectiveOffsetX;
			} else {
				Vector2 prospectiveOffsetY = new Vector2(0F, hotVector.y);
				Vector2 prospectiveCoordinatesY = moduleNodeCoordinates + prospectiveOffsetY;
				Vector2 prospectiveGridCoordinatesY = ScriptShipSheet.GetGridNodeCoordinates(prospectiveCoordinatesY);
				if(scriptShipSheet.schematic[(int)prospectiveGridCoordinatesY.x, (int)prospectiveGridCoordinatesY.y].isEmpty)
				{
					hotVector = prospectiveOffsetY;
				} else {
					Debug.LogError("No possible attach sites on " + coordinates);
				}
			}
		}
		return hotVector;
	}


	public void SetActivation(bool willBeActivated)
	{
		if (willBeActivated) {
			spriteRenderer.color = activatedColor;
			isActivated = true;

				} else {
			spriteRenderer.color = defaultColor;
			isActivated = false;
				}
	}
}
