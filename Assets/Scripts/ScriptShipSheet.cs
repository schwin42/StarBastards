using UnityEngine;
using System.Collections;
using System.Collections.Generic;




[System.Serializable]
public class Node
{
	public ScriptModule module;
	//public ModuleType moduleType;

	//public bool isChecked;
	public bool isAdded;
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

	public List<ScriptModule> pilotContiguousModules;

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
	public void GridStatus()
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
	public List<ScriptModule> GetModulesContiguousToPilot()
	{
		pilotContiguousModules = new List<ScriptModule> ();

		ScriptModule pilotModule = scriptShipController.pilotModule.GetComponent<ScriptModule> ();

		//AddContiguousModules (pilotModule.moduleNodeCoordinates);
		//Set is checked to true - Module is checked only once it is added and all its adjacent modules are added to the queue

		pilotContiguousModules.Add (pilotModule);
		Vector2 pilotGridCoordinates = GetGridNodeCoordinates (pilotModule.moduleNodeCoordinates);
		schematic[(int)pilotGridCoordinates.x, (int)pilotGridCoordinates.y].isAdded = true;

		//Main iteration
		for(int i = 0; i < pilotContiguousModules.Count; i++)
		{
			//Add adjacent modules to list and set as added and set current node as checked
			AddContiguousModules(pilotContiguousModules[i].moduleNodeCoordinates);

		}

		//Clear temp variables
		foreach(ScriptModule module in pilotContiguousModules)
		{
			Vector2 nodeGridCoordinates = GetGridNodeCoordinates (module.moduleNodeCoordinates);
			Node hotNode = schematic[(int)nodeGridCoordinates.x, (int)nodeGridCoordinates.y];

			//hotNode.isChecked = false;
			hotNode.isAdded = false;
		}

		return pilotContiguousModules;

		}

	 void AddContiguousModules(Vector2 nodeWorldCoordinates)
	{
		Vector2 nodeGridCoordinates = GetGridNodeCoordinates (nodeWorldCoordinates);
		//int nodeGridX = (int)nodeGridCoordinates.x;
		//int nodeGridY = (int)nodeGridCoordinates.y;
		Vector2[] adjacentCoordinates = 
		{
			new Vector2(nodeGridCoordinates.x, nodeGridCoordinates.y + 1), //Up
			new Vector2(nodeGridCoordinates.x, nodeGridCoordinates.y - 1), //Down
			new Vector2(nodeGridCoordinates.x + 1, nodeGridCoordinates.y), //Right
			new Vector2(nodeGridCoordinates.x - 1, nodeGridCoordinates.y) //Left
		};

		foreach (Vector2 adjacentVector2 in adjacentCoordinates) {
			Debug.Log ((int)adjacentVector2.x + " " + (int)adjacentVector2.y);
			Node adjacentNode = schematic[(int)adjacentVector2.x, (int)adjacentVector2.y];
			if(adjacentNode.isEmpty || adjacentNode.isAdded)
			{
				Debug.Log ("PK Stand There");
				//PK Stand There
			} else {
				Debug.Log ("Added");
			pilotContiguousModules.Add (adjacentNode.module);
				adjacentNode.isAdded = true;
			}
			//schematic [nodeGridX, nodeGridY].isChecked = true;
				}


	}
}
