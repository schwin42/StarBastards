using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptSpace : MonoBehaviour {

	public Transform shipContainer;
	public List<ScriptShipController> shipsAtStart;

	//Configurable
	public float cameraScaleConstant = 1;
//	public float cameraScaleOffset = 0;
	public float cameraSizeMin = 10;
	public float cameraSizeMax = Mathf.Infinity;

	//Configurable
	//public GameObject upperBoundary;
	//public GameObject rightBoundary;
	//public GameObject lowerBoundary;
	//public GameObject leftBoundary;

	// Use this for initialization
	void Start () {
	
		foreach(ScriptShipController ship in shipContainer.GetComponentsInChildren<ScriptShipController>())
		{
			shipsAtStart.Add (ship);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		UpdateCameraWindow();

	}
	/*
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
*/
	void UpdateCameraWindow()
	{
		if(shipsAtStart.Count > 1)
		{
			Vector2 firstShipToSecond = shipsAtStart[1].transform.position - shipsAtStart[0].transform.position;
			Vector2 cameraCenterPosition = (Vector2)shipsAtStart[0].transform.position + firstShipToSecond / 2;
			float absLargerDistance = Mathf.Max(Mathf.Abs (firstShipToSecond.x), Mathf.Abs (firstShipToSecond.y) );
			float newCameraSize = Mathf.Clamp((absLargerDistance * cameraScaleConstant), cameraSizeMin, cameraSizeMax);
			//Debug.Log (absLargerDistance);
			camera.orthographicSize = newCameraSize;
			Debug.Log ("First ship to second: " +firstShipToSecond + ", Camera center position: "
			           + cameraCenterPosition + ", Abs larger distance: " + absLargerDistance + ", New camera size: " + newCameraSize);
			transform.position = new Vector3(cameraCenterPosition.x, cameraCenterPosition.y, transform.position.z);


		//Debug.Log (firstShipToSecond);
		}
	}
}
