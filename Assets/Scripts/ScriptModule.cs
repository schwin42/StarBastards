using UnityEngine;
using System.Collections;

public enum ModuleType
{
	None,
	Weapon,
	Upgrade,
	Shield,
	Plate
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
	public GameObject moduleOwner = null; //Null indicates module is neutral
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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
