using UnityEngine;
using System.Collections;
using System.Collections.Generic;




[System.Serializable]
public class Node
{
	public ScriptModule module;
	//public ModuleType moduleType;

	private bool isChecked;
	public bool isEmpty;
	//private List<Node> isConnectedTo = new List<Node>();
	//private Vector2 nodeCoordinates;

	public Node()
	{
		module = null;
		isEmpty = true;
	}

	public Node(ScriptModule scriptModuleArg)
	{
		module = scriptModuleArg;
		isEmpty = false;
	}

}
/*
public class Schematic
{
	 //x list containing y list
	//public int xLowerLimit;
	//public int xUpper;
	//public int yLowerLimit;
	//public int yUpper;

	public Schematic ()
	{
	//	List<Node> yList = new List<Node>{
	//	xLowerLimit = 0;
		//xUpper = 0;
	//	yLowerLimit = 0;
		//yUpper = 0;
	}
}
*/

public class ScriptShipSheet : MonoBehaviour {




	static private int maxY = 50;
	static private int maxX = 50;

	//[System.Serializable]
	public Node[,] schematic = new Node[maxX*2,maxY*2];
	
	// Use this for initialization
	void Start () {

		//foreach (GameObject module in transform) {

		//		}

		InitializeGrid ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown("1"))
		   {
			GridStatus();
		}

	}

	void InitializeGrid()
	{
	//Temporary variables
		int bound0 = schematic.GetUpperBound(0);
		int bound1 = schematic.GetUpperBound(1);


		for (int i = 0; i < bound0; i++) {
			for(int j = 0; j < bound1; j++)
			{
				schematic[i, j] = new Node();
			}
				}

		/*

		*/
	}

	//Warning: very slow function
	void GridStatus()
	{
		int bound0 = schematic.GetUpperBound(0);
		int bound1 = schematic.GetUpperBound(1);

		for (int i = 0; i < bound0; i++) {
			for(int j = 0; j < bound1; j++)
			{
				Node hotNode = schematic[i, j];
				string moduleString = "None";
				if(hotNode.module)
				{
					moduleString = hotNode.module.gameObject.name;
				}
				Debug.Log ("Node(" + i + ", " + j + "): Empty? " + hotNode.isEmpty + ", Module: " + moduleString);
				//.module.gameObject.name);
			}
		}
	}
	
	public Vector2 GetGridNodeCoordinates(Vector2 nodeCoordinates)
	{
		Vector2 gridNodeCoordinates = Vector2.zero;
		gridNodeCoordinates.x = nodeCoordinates.x + maxX;
		gridNodeCoordinates.y = nodeCoordinates.y + maxY;
		//Debug.Log (gridNodeCoordinates);
		return gridNodeCoordinates;
	}
}
