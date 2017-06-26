using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum InputType
{
	None,
	Left,
	Thrust,
	Right,
	Reset
}

public class ScriptButton : MonoBehaviour
{

	//Configurable
	public InputType inputType;
	public Color defaultButtonColor;
	public Color activeButtonColor;
	//public Rect inputRect;

	//Inspector-assigned
	public HumanPilotInput humanInput;

	//Retrieved
	//SpriteRenderer spriteRenderer;
	//	UISprite uiSprite;

	//Status
	//public bool isTouched = false;

	// Use this for initialization
	void Start() {

//		uiSprite = GetComponent<UISprite>();
//		uiSprite.color = defaultButtonColor;
		//spriteRenderer = GetComponent<SpriteRenderer>();
		//spriteRenderer.color = defaultButtonColor;
	
	}

	void OnTouch() {

		switch (inputType) {
		//case InputType.P00Left:
			case InputType.Left:
				humanInput.mainInput[0] = true;
				break;
			case InputType.Thrust:
				humanInput.SetThrustInput(1);
				break;
			case InputType.Right:
				humanInput.mainInput[1] = true;
			//Debug.Log (scriptHumanInput.mainInput[0]);
				break;
			case InputType.Reset:
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				break;
		}
	}

	void OnRelease() {
		//spriteRenderer.color = defaultButtonColor;
//		uiSprite.color = defaultButtonColor;

		switch (inputType) {
			case InputType.Left:
				humanInput.mainInput[0] = false;
				break;
			case InputType.Thrust:
				humanInput.SetThrustInput(0);
				break;
			case InputType.Right:
				humanInput.mainInput[1] = false;
				break;
		}
	}

}
