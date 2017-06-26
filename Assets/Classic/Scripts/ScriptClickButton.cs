using UnityEngine;
using System.Collections;

public class ScriptClickButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton()
	{
		Debug.Log ("Clicked");
		SendMessageUpwards("ResetScene");
	}
}
