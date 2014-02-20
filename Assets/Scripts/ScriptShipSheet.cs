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
	public int snakeIndex;
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

public class ScriptShipSheet : MonoBehaviour {

	public ScriptShipController scriptShipController;

	private List<ScriptModule> pilotContiguousModules;

	static private int maxY = 50;
	static private int maxX = 50;

	//[System.Serializable]
	public Node[,] schematic = new Node[maxX*2,maxY*2];
	
	// Use this for initialization
	void Start () {

		//Assign variables
		scriptShipController = gameObject.GetComponent<ScriptShipController>();

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

	//Get contiguous modules
	List<ScriptModule> GetModulesContiguousToPilot()
	{
		List<ScriptModule> pilotContiguousModules = new List<ScriptModule> ();

		ScriptModule pilotModule = scriptShipController.pilotModule.GetComponent<ScriptModule> ();

		AddContiguousModules (pilotModule.moduleNodeCoordinates);
		//Set is checked to true - Module is checked only once it is added and all its adjacent modules are added to the queue

		}

	void AddContiguousModules(Vector2 nodeWorldCoordinates)
	{
		Vector2 nodeGridCoordinates = GetGridNodeCoordinates (nodeWorldCoordinates);
		int nodeGridX = (int)nodeGridCoordinates.x;
		int nodeGridY = (int)nodeGridCoordinates.y;
		Node[] adjacentNodes = 
		{
			schematic [nodeGridX, nodeGridY + 1], //Up
			schematic [nodeGridX, nodeGridY - 1], //Down
			schematic [nodeGridX + 1, nodeGridY], //Right
			schematic [nodeGridX - 1, nodeGridY] //Left
		};

		foreach (Node node in adjacentNodes) {
			pilotContiguousModules.Add (node);

				}

		//Get nodes adjacent to node that are not empty and have not been checked

		//Up
		//if(schematic[nodeGridX, nodeGridY+1]


		//Down

		//Left

		//Right


		//1. Get nodes adjacent to pilot node that are not empty
		//2. Get nodes adjacent to those nodes that are not empty
		//3. Continue until there are no nodes remaining
		//scriptShipController.pilotModule



	//	foreach (ScriptModule module in transform.GetComponentsInChildren<ScriptModule>()) {

	//			}
	}
}
