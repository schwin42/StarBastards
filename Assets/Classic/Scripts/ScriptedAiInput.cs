using UnityEngine;
using System.Collections;


public enum IntelligenceDirective
{
	None,
	Initialize,
	AcquireTarget,
	AlignToTarget,
}

public class ScriptedPilotInput : IPilotInput {

	private float _thrustInput;
	public float ThrustInput { get { return _thrustInput; } }

	private float _turnInput;
	public float TurnInput { get { return _turnInput; } }
	//public bool queueFire = false;
	//public bool[] mainInput;
	//public string playerInput;

	public IntelligenceDirective intelligenceDirective = IntelligenceDirective.Initialize;
	private GameObject target = null;

	private ShipController shipController;

	public ScriptedPilotInput (ShipController shipController) {
		this.shipController = shipController;
	}

	// Update is called once per frame
	void Update () {

		_thrustInput = 1;

		if(intelligenceDirective == IntelligenceDirective.Initialize)
		{
			intelligenceDirective = IntelligenceDirective.AcquireTarget;
		}

		if(intelligenceDirective == IntelligenceDirective.AcquireTarget)
		{
			target = AcquireTarget();
			//Debug.Log (target.GetComponent<ScriptModule>().moduleID);
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
		float minimumDistance = Mathf.Infinity;
		GameObject nearestModule = null;
		foreach(Transform module in GameController.Instance.ModuleContainer)
		{
			Vector3 differenceVector = module.transform.position - shipController.transform.position;
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
		Vector2 forwardDirection = shipController.transform.TransformDirection(Vector2.up);
		Vector3 targetDirection = Vector3.Normalize(target.transform.position - shipController.transform.position);
		Vector2 targetDirection2D = new Vector2(targetDirection.x, targetDirection.y);
		float discrepancyAngle = Vector2.Angle (forwardDirection, targetDirection2D);

		if(discrepancyAngle > 10)
		{
			_turnInput = 1;
		} else if(discrepancyAngle <= 10)
		{
			//Debug.Log (discrepancyAngle);
			_turnInput = 0;
		}


	}
}
