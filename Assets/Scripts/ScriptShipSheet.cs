﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
//Node within x-y grid
public class Node
{
	public ScriptModule module; //Gameobject at this node in the grid, if any
	public bool isAdded = false;  //True indicates this node has been checked off
	public bool isEmpty; //Whether a module exists in this node
	public int snakeIndex = -1; //Negative one for index indicates null value
	public int activationIndex = -1; //Ditto

	//Empty node constructor
	public Node()
	{
		module = null;
		isEmpty = true;
	}

	//Occupied node constructor
	public Node(ScriptModule scriptModuleArg)
	{
		module = scriptModuleArg;
		isEmpty = false;
	}
}

[System.Serializable]
public class Snake
{
	public int snakeID; //id and index
	public List<Node> constituentNodes;
	public ModuleType moduleType;
	public bool isPruned = false; //Whether this snake has been subsumed by another

	public Snake(List<Snake> hotSnakes, ModuleType moduleTypeArg)
	{
		moduleType = moduleTypeArg;
		snakeID = hotSnakes.Count;
		hotSnakes.Add (this);
		constituentNodes = new List<Node>();

	}

	public void AddNodeToSnake(Node hotNode)
	{

		constituentNodes.Add (hotNode);
		hotNode.snakeIndex = snakeID;
		//Debug.Log (hotNode.module.name + " added to " + snakeID);
	
	}

}

public class ScriptShipSheet : MonoBehaviour {

	public ScriptShipController scriptShipController;

	public List<ScriptModule> pilotContiguousModules; //Modules connected to first module


	//Scriptable
	static private int maxX = 50; //Max positive and negative x-value
	static private int maxY = 50; //Max positive and negative y-value
	private int minNodesForActivation = 3; //Number of contiguous modules required to form activation

	public List<Snake> lastSnakes; //List of last known snakes

	//Grid as 2D array
	public Node[,] schematic = new Node[maxX*2,maxY*2];
	
	// Use this for initialization
	void Start () {

		//Acquire scripts
		scriptShipController = gameObject.GetComponent<ScriptShipController>();
		
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

		//Create a node for each pair of coordinates
		for (int i = 0; i < bound0; i++) {
			for(int j = 0; j < bound1; j++)
			{
				schematic[i, j] = new Node();
			}
				}
	}

	public void GridStatus()
	{
		int bound0 = schematic.GetUpperBound(0);
		int bound1 = schematic.GetUpperBound(1);

		for (int i = 0; i < bound0; i++) {
			for(int j = 0; j < bound1; j++)
			{
				Node hotNode = schematic[i, j];
				string moduleString;
				if(hotNode.module)
				{
					moduleString = hotNode.module.gameObject.name;
				}
				//Debug.Log ("Node(" + i + ", " + j + "): Empty? " + hotNode.isEmpty + ", Module: " + moduleString);
			}
		}
	}

	//Convert real world coordinates to absolute grid coordinates
	public static Vector2 GetGridNodeCoordinates(Vector2 nodeCoordinates)
	{
		Vector2 gridNodeCoordinates = Vector2.zero;
		gridNodeCoordinates.x = nodeCoordinates.x + maxX;
		gridNodeCoordinates.y = nodeCoordinates.y + maxY;
		//Debug.Log (gridNodeCoordinates);
		return gridNodeCoordinates;
	}

	//Return list of modules that are connected to first module
	public List<ScriptModule> GetModulesContiguousToPilot()
	{
		//Temporary variables
		pilotContiguousModules = new List<ScriptModule> ();
		ScriptModule pilotModule = scriptShipController.pilotModule.GetComponent<ScriptModule> ();

		//Breadth-first search
		pilotContiguousModules.Add (pilotModule);
		Vector2 pilotGridCoordinates = GetGridNodeCoordinates (pilotModule.moduleNodeCoordinates);
		schematic[(int)pilotGridCoordinates.x, (int)pilotGridCoordinates.y].isAdded = true;

		//Main iteration
		for(int i = 0; i < pilotContiguousModules.Count; i++)
		{
			//Add adjacent modules to list and set as added
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
		Debug.Log (pilotContiguousModules.Count);
		return pilotContiguousModules;
	}

	 void AddContiguousModules(Vector2 nodeWorldCoordinates)
	{
		Vector2 nodeGridCoordinates = GetGridNodeCoordinates (nodeWorldCoordinates);

		Vector2[] adjacentCoordinates = GetAdjacentPoints (nodeGridCoordinates);

		foreach (Vector2 adjacentVector2 in adjacentCoordinates) {
			//Debug.Log ((int)adjacentVector2.x + " " + (int)adjacentVector2.y);
			Node adjacentNode = schematic[(int)adjacentVector2.x, (int)adjacentVector2.y];
			if(adjacentNode.isEmpty || adjacentNode.isAdded)
			{
				//Do nothing
			} else {
			pilotContiguousModules.Add (adjacentNode.module);
				adjacentNode.isAdded = true;
			}
		}
	}


	List<Snake> GetModuleSnakes()
	{
		ScriptModule[] childModules = GetComponent<ScriptShipController>().shipModuleContainer.
			GetComponentsInChildren<ScriptModule>();
		List<Snake> hotSnakes = new List<Snake>();
		//int nextSnakeID = 0;

		
		//After loop ends, clear temporary variables
		ClearOldNodeValues();

		foreach(ScriptModule hotMod in childModules)
		{
			ModuleType targetType = hotMod.moduleType;
			if(!(targetType == ModuleType.Pilot || targetType == ModuleType.None))
			{
				Node hotModNode = GetNodeFromModule(hotMod);
			Vector2 nodeGridCoordinates = GetGridNodeCoordinates(hotMod.moduleNodeCoordinates);
			//Debug.Log ("Root module: " + hotModNode.module.name);

			//Check adjacent modules for like modules
				Vector2[] adjacentDirections = GetAdjacentPoints(nodeGridCoordinates);
		
			foreach (Vector2 adjacentCoordinates in adjacentDirections) 
			{
				Node adjacentNode = schematic[(int)adjacentCoordinates.x, (int)adjacentCoordinates.y];
				if(adjacentNode.isEmpty)
				{
					//PK Stand There!
				} else {
						if(adjacentNode.module.moduleType == targetType)
						{ //If module type matches center node
						//	Debug.Log ("Root module, adjacent module: " + hotModNode.module.name + ", " + adjacentNode.module.name);

							if(hotModNode.snakeIndex == -1){ //A0: Center node does NOT belong to snake
								if(adjacentNode.snakeIndex == -1){//B0: Adjacent node does NOT belong to snake
									//Debug.Log ("00");
									//Create new snake and add both to it
									Snake hotSnake = new Snake(hotSnakes, targetType);
									hotSnake.AddNodeToSnake(hotModNode);
									hotSnake.AddNodeToSnake(adjacentNode);} 
								else if(adjacentNode.snakeIndex >= 0){//B1: Adjacent node belongs to snake
									//Debug.Log ("01");
									//Add center node to adjacent node's snake
									hotSnakes[adjacentNode.snakeIndex].AddNodeToSnake(hotModNode);} 
								else {Debug.LogError ("Invalid adjacent node index: " + adjacentNode.snakeIndex);}
							} else if(hotModNode.snakeIndex >= 0){//A1: Center node belongs to snake
								if(adjacentNode.snakeIndex == -1){//B0: Adjacent node does NOT belong to snake
									//Debug.Log ("10");
									//Add adjacent node to center node's snake
									hotSnakes[hotModNode.snakeIndex].AddNodeToSnake(adjacentNode);}
								else if(adjacentNode.snakeIndex == hotModNode.snakeIndex){//B1: Adjacent node belongs to same snake
									//Debug.Log ("11");
									//PK Stand There!
								}
								else if(adjacentNode.snakeIndex >= 0){//B2: Adjacent node belongs to different snake	
									//Debug.Log ("12");
									hotSnakes[adjacentNode.snakeIndex].isPruned = true; //Mark adjacent snake as dead
									//Assign node's snake id as root node snake
									List<Node> transferList = new List<Node>(hotSnakes[adjacentNode.snakeIndex].constituentNodes); //Cache adjacent snake's nodes
									hotSnakes[adjacentNode.snakeIndex].constituentNodes = new List<Node>(); //Clear adjacent snake's nodes

									foreach(Node transferNode in transferList)
									{
										Debug.Log("12 - root snake, adjacent snake, module:" + hotModNode.snakeIndex + ", " + adjacentNode.snakeIndex + ", " + transferNode.module.name);
										hotSnakes[hotModNode.snakeIndex].AddNodeToSnake(transferNode);
										//node.snakeIndex = hotModNode.snakeIndex;
									}
									//Debug.Break ();
									//hotSnakes[hotModNode.snakeIndex].constituentNodes.AddRange(
									//	hotSnakes[adjacentNode.snakeIndex].constituentNodes); //Add adjacent snake's modules to center snake
								}
								else {Debug.LogError ("Invalid adjacent node index: " + adjacentNode.snakeIndex);}
							}
							else
							{Debug.LogError ("Invalid root node index: " + hotModNode.snakeIndex);}

				}
				}
			}
		}
		}

		lastSnakes = hotSnakes;
		return hotSnakes;
	}
		
	void ClearOldNodeValues()
	{
		foreach(ScriptModule hotMod in GetComponent<ScriptShipController>().shipModuleContainer.GetComponentsInChildren<ScriptModule>())
		        {
			Node hotModNode = GetNodeFromModule(hotMod);
		
			hotModNode.snakeIndex = -1;
			hotModNode.activationIndex = -1;
			hotModNode.isAdded = false;
		}
	}

	List<Activation> ConvertSnakesToActivations(List<Snake> hotSnakes)
	{
		List<Activation> hotActivations = new List<Activation> ();
		foreach (Snake snake in hotSnakes) {
			//Debug.Log ("Node count >= min Nodes " + snake.constituentNodes.Count + ", " + minNodesForActivation);
			if(snake.constituentNodes.Count >= minNodesForActivation)
			{
				hotActivations.Add (new Activation(snake.snakeID, snake.moduleType, snake.constituentNodes)); 
				foreach(Node node in snake.constituentNodes)
				{
					node.activationIndex = hotActivations.Count - 1;
				}
			}
				}
				                    return hotActivations;
	}

	public void ArmActivations (List<Activation> hotActivations)
	{

		scriptShipController.ClearOldLasers();
		//Debug.Log ("Arming");
		foreach(Activation activation in hotActivations)
		{

				//Calculate activation stats
				foreach(Node node in activation.constituentNodes)
				{
					switch(node.module.moduleType)
					{
					case ModuleType.Weapon:

					switch(node.module.moduleSubtype)
					{
					case ModuleSubtype.Duration:
						activation.gunDurationLevel++;
						break;
					//case ModuleSubtype.Homing:
					//	activation.homingLevel++;
					//	break;
					case ModuleSubtype.Number:
						activation.gunNumberLevel++;
						break;
					case ModuleSubtype.Power:
						activation.gunPowerLevel++;
						break;
					case ModuleSubtype.Radius:
						activation.gunRadiusLevel++;
						break;
					case ModuleSubtype.Speed:
						activation.gunSpeedLevel++;
						break;
					default:
						Debug.LogError("Invalid module subtype: " + node.module.moduleSubtype);
						break;
					}
						break;
					case ModuleType.Armor:
						activation.armorPowerLevel++;
						break;
					case ModuleType.Laser:
						activation.laserRadiusLevel++;
						break;
					default:
						Debug.Log ("Invalid module type: " + node.module.moduleType);
					break;
					}

					activation.DeriveRealStats(activation.moduleType);

				if(activation.moduleType == ModuleType.Armor)
				{
					node.module.currentHP += activation.bonusHP;
				} 
				}
			//For each activation
			if(activation.moduleType == ModuleType.Laser)
			{
				scriptShipController.InitializeLaser(activation);
			}
			
			

		}

		//return hotActivations;
	}


	public List<Activation> GetActivations()
	{
		return ConvertSnakesToActivations(GetModuleSnakes ());
	}

	public Node GetNodeFromModule(ScriptModule module)
	{
		Vector2 nodeGridCoordinates = GetGridNodeCoordinates(module.moduleNodeCoordinates);
		return schematic[(int)nodeGridCoordinates.x, (int)nodeGridCoordinates.y];
	}

	Vector2[] GetAdjacentPoints(Vector2 startingPoint)
	{
		Vector2[] adjacentPoints = 
		{
			new Vector2(startingPoint.x, startingPoint.y + 1), //Up
			new Vector2(startingPoint.x, startingPoint.y - 1), //Down
			new Vector2(startingPoint.x + 1, startingPoint.y), //Right
			new Vector2(startingPoint.x - 1, startingPoint.y) //Left
		};
		return adjacentPoints;
	}

}
