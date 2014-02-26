using UnityEngine;
using System.Collections;

public class ScriptSpace : MonoBehaviour {

	//Configurable
	public GameObject upperBoundary;
	public GameObject rightBoundary;
	public GameObject lowerBoundary;
	public GameObject leftBoundary;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void WrapAround(GameObject thing, Direction direction)
	{
		if(thing.tag == "Ship")
		{
			Transform hotTrans = thing.transform.parent.parent;
			ScriptShipController scriptShipController = hotTrans.GetComponent<ScriptShipController>();
		if(!scriptShipController.justTeleported)
		{
	
			Vector3 newPosition = Vector3.zero;
			switch(direction)
		{
			case Direction.Down:
			newPosition = new Vector3(hotTrans.position.x, upperBoundary.transform.position.y, hotTrans.position.z);
			hotTrans.position = newPosition;
				break;
			case Direction.Up:
				newPosition = new Vector3(hotTrans.position.x, lowerBoundary.transform.position.y, hotTrans.position.z);
				hotTrans.position = newPosition;
			break;
			case Direction.Right:
				newPosition = new Vector3(leftBoundary.transform.position.x, hotTrans.position.y, hotTrans.position.z);
				hotTrans.position = newPosition;
				break;
				case Direction.Left:
					newPosition = new Vector3(rightBoundary.transform.position.x, hotTrans.position.y, hotTrans.position.z);
					hotTrans.position = newPosition;
					break;


		}
			//hotTrans.position = newPosition;
			scriptShipController.justTeleported = true;
				Debug.Log ("Teleported status on " + Time.time);

		//	scriptShipController.gameObject.layer = 9;
			} else {
				Debug.Log ("Ignore boundary " + Time.time);
			}
	}
	}
}
