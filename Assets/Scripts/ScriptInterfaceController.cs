using UnityEngine;
using System.Collections;

public class ScriptInterfaceController : MonoBehaviour {

	public GameObject interfaceTouch;
	public GameObject interfaceGamepad;

	// Use this for initialization
	void Start () {
	
		//interfaceTouch = transform.FindChild("InterfaceTouch").gameObject;
		//interfaceGamepad = transform.FindChild("InterfaceGamepad").gameObject;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetInterface(ControlType controlType)
	{
		if(controlType == ControlType.Keyboard || controlType == ControlType.Gamepad)
		{
			interfaceGamepad.SetActive(true);
		} else if(controlType == ControlType.Touch)
		{
			interfaceTouch.SetActive(true);
		}
		else
		{
			Debug.Log ("Invalid control type: " + controlType);
		}
	}

	//void ResetScene()
	//{
	//	Application.LoadLevel("SceneKeyboardMain");
	//}
}
