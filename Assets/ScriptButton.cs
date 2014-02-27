using UnityEngine;
using System.Collections;

public enum InputType
{
	None,
Left,
	Thrust,
	Right
}

public class ScriptButton : MonoBehaviour {

	//Configurable
	public InputType inputType;
	public Color defaultButtonColor;
	public Color activeButtonColor;
	public Rect inputRect;

	//Objects
	public ScriptHumanInput scriptHumanInput;

	//Retrieved
	SpriteRenderer spriteRenderer;


	// Use this for initialization
	void Start () {

		spriteRenderer = GetComponent<SpriteRenderer>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		//Debug.Log ("buttondown");
		spriteRenderer.color = activeButtonColor;

		switch(inputType)
		{
		//case InputType.P00Left:
		case InputType.Left:
			scriptHumanInput.mainInput[0] = true;
			//Debug.Log (scriptHumanInput.mainInput[0]);
			break;

		case InputType.Thrust:
			scriptHumanInput.thrustInput = 1;
			//Debug.Log (scriptHumanInput.mainInput[0]);
			break;

		case InputType.Right:
			scriptHumanInput.mainInput[1] = true;
			//Debug.Log (scriptHumanInput.mainInput[0]);
			break;

		}
	}

	void OnMouseUp()
	{
		spriteRenderer.color = defaultButtonColor;

		switch(inputType)
		{
		case InputType.Left:
		scriptHumanInput.mainInput[0] = false;
			break;
		case InputType.Thrust:
			scriptHumanInput.thrustInput = 0;
			break;
		case InputType.Right:
			scriptHumanInput.mainInput[1] = false;
			break;
		}
	}

}
