using UnityEngine;
using System.Collections;

public enum InputType
{
	None,
Left,
	Thrust,
	Right,
	Reset
}

public class ScriptButton : MonoBehaviour {

	//Configurable
	public InputType inputType;
	public Color defaultButtonColor;
	public Color activeButtonColor;
	//public Rect inputRect;

	//Inspector-assigned
	public ScriptHumanInput scriptHumanInput;

	//Retrieved
	//SpriteRenderer spriteRenderer;
	UISprite uiSprite;

	//Status
	//public bool isTouched = false;

	// Use this for initialization
	void Start () {

		uiSprite = GetComponent<UISprite>();
		uiSprite.color = defaultButtonColor;
		//spriteRenderer = GetComponent<SpriteRenderer>();
		//spriteRenderer.color = defaultButtonColor;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/*
	void OnMouseDown()
	{
		//Debug.Log ("buttondown");

	}

	void OnMouseUp()
	{

	}
*/
	void OnTouch()
	{
		//spriteRenderer.color = activeButtonColor;
		uiSprite.color = activeButtonColor;

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

		case InputType.Reset:
			Application.LoadLevel("SceneMain");
			break;
			
		}
	}

	void OnRelease()
	{
		//spriteRenderer.color = defaultButtonColor;
		uiSprite.color = defaultButtonColor;

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
