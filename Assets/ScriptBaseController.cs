using UnityEngine;
using System.Collections;

public class ScriptBaseController : MonoBehaviour {

	//Prefabs
	public GameObject bulletPrefab;

	//Configurable
	public int maxHP;

	//State
	public bool baseIsActive = true;
	float bulletTimer = 0;
	bool canShoot = false;

	//Configurable
	public float bulletRate = .2F;
	public float bulletDuration = 3f;
	public float bulletForce = 2000;	 
	public int currentHP;
	
	// Use this for initialization
	void Start () {
		currentHP = maxHP;
	}
	
	// Update is called once per frame
	void Update () {

			if(currentHP <= 0)
			{
				
					baseIsActive = false;
					
					//}
					tag = "Debris";
				

		}

		if(baseIsActive)
		{
	if(canShoot)
		{
			GameObject hotBullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
			hotBullet.GetComponent<Rigidbody2D>().AddForce(Vector2.right * bulletForce);
			ScriptProjectile hotScript = hotBullet.GetComponent<ScriptProjectile>();
				hotScript.owner = gameObject;
			hotScript.bulletDuration = bulletDuration;

			canShoot = false;
		} else {
			bulletTimer += Time.deltaTime;
			if (bulletTimer >= bulletRate)
			{
				canShoot = true;
				bulletTimer = 0;
			}
			}
		}
	}
}
