using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerTouch : MonoBehaviour {

	//Inspector-assigned
	public Camera touchCamera; 

	//Records
	Dictionary<int, Collider> fingerIdToButton = new Dictionary<int, Collider>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (Input.touchCount + " " + Time.time);
		if(Input.touchCount > 0)
		{

			foreach(Touch touch in Input.touches)
			{
				//Debug.Log (touch.position);
				Ray ray = touchCamera.ScreenPointToRay(touch.position);
				//Debug.Log (ray.origin + " " + ray.direction);
				//Debug.DrawRay(ray.origin, ray.direction, Color.white);
				RaycastHit hit;
				if(touch.phase == TouchPhase.Began)
				{

					if(Physics.Raycast(ray.origin, ray.direction, out hit, 100F))
					   {
					//Debug.Log ("Began and hit " + Time.time);
						if(!fingerIdToButton.ContainsKey(touch.fingerId))
						{
						fingerIdToButton.Add (touch.fingerId, hit.collider);
					hit.collider.SendMessage("OnTouch");
						} else {
							Debug.LogError (touch.fingerId + " is already in fingerIdToButton and cannot be added.");
						}
					} else {
						//Touch did not hit UI element; ignore

					//	fingerIdToButton.Add (touch.fingerId, null);
					}
				} else if (touch.phase == TouchPhase.Ended) {
					if(fingerIdToButton.ContainsKey(touch.fingerId))
					   {
					Collider hotCollider = fingerIdToButton[touch.fingerId];
					fingerIdToButton.Remove(touch.fingerId);
					hotCollider.SendMessage("OnRelease");
					} else {
						//Touch not registered to button; ignore
					}
				} else if (touch.phase == TouchPhase.Canceled) {
					Debug.Log ("Touch " + touch.fingerId + " canceled.");
				} else {
					//Touch's phase is moved or stationary; do nothing
				}

			}
		}

	}
}
