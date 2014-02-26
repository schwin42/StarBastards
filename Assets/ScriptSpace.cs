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
		//if(!scriptShipController.justTeleported)
		//{
	
			Vector3 newPosition = Vector3.zero;
			switch(direction)
		{
			case Direction.Down:
				if(scriptShipController.teleportDirection != Direction.Down)
					{
						newPosition = new Vector3(hotTrans.position.x, upperBoundary.transform.position.y, hotTrans.position.z);
						hotTrans.position = newPosition;
						scriptShipController.teleportDirection = Direction.Up;
					//Debug.Log ("Up" + Time.time);
					}
				break;
			case Direction.Up:
				if(scriptShipController.teleportDirection != Direction.Up )
					{
					//Debug.Log ("Direction on up case " + scriptShipController.teleportDirection + " " + Time.time);
				newPosition = new Vector3(hotTrans.position.x, lowerBoundary.transform.position.y, hotTrans.position.z);
				hotTrans.position = newPosition;
							scriptShipController.teleportDirection = Direction.Down;
					//Debug.Log ("Down" + Time.time);
				}
			break;
			case Direction.Right:
					if(scriptShipController.teleportDirection != Direction.Right )
						{
				newPosition = new Vector3(leftBoundary.transform.position.x, hotTrans.position.y, hotTrans.position.z);
						hotTrans.position = newPosition;
								scriptShipController.teleportDirection = Direction.Left;
					}
				break;
				case Direction.Left:
						if(scriptShipController.teleportDirection != Direction.Left )
							{
					newPosition = new Vector3(rightBoundary.transform.position.x, hotTrans.position.y, hotTrans.position.z);
					hotTrans.position = newPosition;
									scriptShipController.teleportDirection = Direction.Right;
						}
					break;


		}
			//hotTrans.position = newPosition;
			//scriptShipController.justTeleported = true;

				//Debug.Log ("Teleported status on " + Time.time);

		//	scriptShipController.gameObject.layer = 9;
			//} else {
			//	Debug.Log ("Ignore boundary " + Time.time);
			//}
	}
	}
}
