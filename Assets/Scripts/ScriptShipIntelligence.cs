using UnityEngine;
using System.Collections;


public enum IntelligenceDirective
{
	None,
	Initialize,
	AcquireTarget,
	AlignToTarget,
}

public class ScriptShipIntelligence : MonoBehaviour {

	public float thrustInput = 1F;
	public float turnInput = -9999F;
	//public bool queueFire = false;
	//public bool[] mainInput;
	//public string playerInput;

	public IntelligenceDirective intelligenceDirective = IntelligenceDirective.Initialize;
	public GameObject target = null;



	private ScriptShipSheet scriptShipSheet;
	private Transform spaceController;

	// Use this for initialization
	void Start () {
		spaceController = GameObject.Find ("ControllerSpace").transform;
	}
	
	// Update is called once per frame
	void Update () {

		thrustInput = 1;

		if(intelligenceDirective == IntelligenceDirective.Initialize)
		{
			intelligenceDirective = IntelligenceDirective.AcquireTarget;
		}

		if(intelligenceDirective == IntelligenceDirective.AcquireTarget)
		{
			target = AcquireTarget();
			Debug.Log (target.GetComponent<ScriptModule>().moduleID);
			if(target)
			{
				intelligenceDirective = IntelligenceDirective.AlignToTarget;
			}
		}

		if(intelligenceDirective == IntelligenceDirective.AlignToTarget)
		{
			if(!target || target.tag == "Ship")
			{
				intelligenceDirective = IntelligenceDirective.AcquireTarget;
			}
			AlignToTarget();
		}

	}

	GameObject AcquireTarget()
	{
		float minimumDistance = 9999;
		GameObject nearestModule = gameObject;
		foreach(Transform module in spaceController)
		{
			Vector3 differenceVector = module.transform.position - transform.position;
			float distance = differenceVector.magnitude;
			if(distance <= minimumDistance)
			{
				minimumDistance = distance;
				nearestModule = module.gameObject;
			}
		}

		return nearestModule;


		//Debug.Log ("No targets available");
		//return null;
	}

	void AlignToTarget()
	{
		Vector2 forwardDirection = transform.TransformDirection(Vector2.up);
		Vector3 targetDirection = Vector3.Normalize(target.transform.position - transform.position);
		Vector2 targetDirection2D = new Vector2(targetDirection.x, targetDirection.y);
		float discrepancyAngle = Vector2.Angle (forwardDirection, targetDirection2D);

		if(discrepancyAngle > 10)
		{
			turnInput = 1;
		} else if(discrepancyAngle <= 10)
		{
			Debug.Log (discrepancyAngle);
			turnInput = 0;
		}


	}
}
