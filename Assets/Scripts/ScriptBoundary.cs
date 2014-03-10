using UnityEngine;
using System.Collections;

[System.Serializable]
public enum Direction
{
	None,
	Up,
	Right,
	Down,
	Left
}

public class ScriptBoundary : MonoBehaviour {

	//Confirgurable
	public ScriptSpace scriptSpace;
	public Direction direction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

//	void OnTriggerEnter2D(Collider2D collider)
//	{
//		scriptSpace.WrapAround(collider.gameObject, direction);
//	}
}
