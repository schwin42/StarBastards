using UnityEngine;
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

	//State
	public bool isArmed = false;
	public float shotTimer;
	
		//Proper stats
			//Gun
		public int gunDurationLevel = 0;
		public int gunHomingLevel = 0;
		public int gunNumberLevel = 0;
		public int gunPowerLevel = 0;
		public int gunRadiusLevel = 0;
		public int gunSpeedLevel = 0;
	
			//Armor
		public int armorPowerLevel = 0;
		public int armorRadiusLevel = 0;
	
			//Laser
		public int laserRadiusLevel = 0;
		public int laserPowerLevel = 0;
		public int laserNumberLevel = 0;
	
		//Mechanical stats
			//Gun
		public int damage = 10;
		public float bulletsPerSecond; //In seconds
		public int shotForce = 2000;
		//public int size = 0;
		public int scatterAngle = 0;
		public int durationInSeconds = 0;
		public int homingConstant = 0;
		public int bulletsPerShot = 0;
		public int bulletScale = 0;
			//Armor
		public int bonusHP;
			//Laser
		public float laserTriggerScale;


		//Gun state
	public bool canShoot = false;
	//Laser state
	public bool laserTriggerEnabled = false;



		public void DeriveRealStats(ModuleType moduleType)
		{
			switch(moduleType)
			{
			case ModuleType.Weapon:
			durationInSeconds = gunDurationLevel + 1; //Magic numbers, please change
			homingConstant = gunHomingLevel;
			bulletsPerSecond = (gunNumberLevel * 2) + 1;
			damage = (gunPowerLevel + 1) * 10;
			bulletScale = (gunRadiusLevel * 2) + 1;
			shotForce = (gunSpeedLevel + 1) * 1000;
				break;
			case ModuleType.Armor:
				bonusHP = armorPowerLevel * 10;
				break;
			case ModuleType.Laser:
				laserTriggerScale = laserRadiusLevel * 10;
				break;
			default:
				Debug.LogError("Invalid module type: " + moduleType);
				break;
			}
		}

	public Snake(List<Snake> hotSnakes, ModuleType moduleTypeArg)
	{
		moduleType = moduleTypeArg;
		snakeID = hotSnakes.Count;
		hotSnakes.Add (this);
		constituentNodes = new List<Node>();

	}

	public void AddNodeToSnake(Node node)
	{
		constituentNodes.Add (node);
		node.snakeIndex = snakeID;

		//Increment snake levels
		switch(node.module.moduleType)
								{
								case ModuleType.Weapon:
									
									switch(node.module.moduleSubtype)
									{
									case ModuleSubtype.Duration:
										gunDurationLevel++;
										break;
										//case ModuleSubtype.Homing:
										//	activation.homingLevel++;
										//	break;
									case ModuleSubtype.Number:
										gunNumberLevel++;
										break;
									case ModuleSubtype.Power:
										gunPowerLevel++;
										break;
									case ModuleSubtype.Radius:
										gunRadiusLevel++;
										break;
									case ModuleSubtype.Speed:
										gunSpeedLevel++;
										break;
									default:
										Debug.LogError("Invalid module subtype: " + node.module.moduleSubtype);
										break;
									}
									break;
								case ModuleType.Armor:
									armorPowerLevel++;
									break;
								case ModuleType.Laser:
									laserRadiusLevel++;
									break;
								default:
									Debug.Log ("Invalid module type: " + node.module.moduleType);
									break;
								}
		DeriveRealStats(moduleType);

	}

	public void SetArmed(bool willBeArmed)
	{
		if(willBeArmed)
		{
		
		foreach(Node node in constituentNodes)
		{
				node.module.SetActivation (true);
				//node.module.isActivated = true;
	
					if(moduleType == ModuleType.Armor)
					{
						node.module.currentHP += bonusHP;
					} 
	
		}
			isArmed = true;

		}else { 
			foreach(Node node in constituentNodes)
			{
				node.module.SetActivation(false);
			}
			isArmed = false;

	}
	}

}

public class ScriptShipSheet : MonoBehaviour {

	protected ScriptShipController scriptShipController;

	public List<ScriptModule> pilotContiguousModules; //Modules connected to first module


	//Scriptable
	static private int maxX = 50; //Max positive and negative x-value
	static private int maxY = 50; //Max positive and negative y-value
	private int minNodesForActivation = 3; //Number of contiguous modules required to form activation

	//public List<Snake> lastSnakes; //List of last known snakes
	public List<Snake> currentSnakes; 

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


//	List<Snake> GetModuleSnakes()
//	{
//		ScriptModule[] childModules = GetComponent<ScriptShipController>().shipModuleContainer.
//			GetComponentsInChildren<ScriptModule>();
//		List<Snake> hotSnakes = new List<Snake>();
//		//int nextSnakeID = 0;
//
//		
//		//After loop ends, clear temporary variables
//		ClearOldNodeValues();
//
//		foreach(ScriptModule hotMod in childModules)
//		{
//			ModuleType targetType = hotMod.moduleType;
//			if(!(targetType == ModuleType.Pilot || targetType == ModuleType.None))
//			{
//				Node hotModNode = GetNodeFromModule(hotMod);
//			Vector2 nodeGridCoordinates = GetGridNodeCoordinates(hotMod.moduleNodeCoordinates);
//			//Debug.Log ("Root module: " + hotModNode.module.name);
//
//			//Check adjacent modules for like modules
//				Vector2[] adjacentDirections = GetAdjacentPoints(nodeGridCoordinates);
//		
//			foreach (Vector2 adjacentCoordinates in adjacentDirections) 
//			{
//				Node adjacentNode = schematic[(int)adjacentCoordinates.x, (int)adjacentCoordinates.y];
//				if(adjacentNode.isEmpty)
//				{
//					//PK Stand There!
//				} else {
//						if(adjacentNode.module.moduleType == targetType)
//						{ //If module type matches center node
//						//	Debug.Log ("Root module, adjacent module: " + hotModNode.module.name + ", " + adjacentNode.module.name);
//
//							if(hotModNode.snakeIndex == -1){ //A0: Center node does NOT belong to snake
//								if(adjacentNode.snakeIndex == -1){//B0: Adjacent node does NOT belong to snake
//									//Debug.Log ("00");
//									//Create new snake and add both to it
//									Snake hotSnake = new Snake(hotSnakes, targetType);
//									hotSnake.AddNodeToSnake(hotModNode);
//									hotSnake.AddNodeToSnake(adjacentNode);} 
//								else if(adjacentNode.snakeIndex >= 0){//B1: Adjacent node belongs to snake
//									//Debug.Log ("01");
//									//Add center node to adjacent node's snake
//									hotSnakes[adjacentNode.snakeIndex].AddNodeToSnake(hotModNode);} 
//								else {Debug.LogError ("Invalid adjacent node index: " + adjacentNode.snakeIndex);}
//							} else if(hotModNode.snakeIndex >= 0){//A1: Center node belongs to snake
//								if(adjacentNode.snakeIndex == -1){//B0: Adjacent node does NOT belong to snake
//									//Debug.Log ("10");
//									//Add adjacent node to center node's snake
//									hotSnakes[hotModNode.snakeIndex].AddNodeToSnake(adjacentNode);}
//								else if(adjacentNode.snakeIndex == hotModNode.snakeIndex){//B1: Adjacent node belongs to same snake
//									//Debug.Log ("11");
//									//PK Stand There!
//								}
//								else if(adjacentNode.snakeIndex >= 0){//B2: Adjacent node belongs to different snake	
//									//Debug.Log ("12");
//									hotSnakes[adjacentNode.snakeIndex].isPruned = true; //Mark adjacent snake as dead
//									//Assign node's snake id as root node snake
//									List<Node> transferList = new List<Node>(hotSnakes[adjacentNode.snakeIndex].constituentNodes); //Cache adjacent snake's nodes
//									hotSnakes[adjacentNode.snakeIndex].constituentNodes = new List<Node>(); //Clear adjacent snake's nodes
//
//									foreach(Node transferNode in transferList)
//									{
//										Debug.Log("12 - root snake, adjacent snake, module:" + hotModNode.snakeIndex + ", " + adjacentNode.snakeIndex + ", " + transferNode.module.name);
//										hotSnakes[hotModNode.snakeIndex].AddNodeToSnake(transferNode);
//										//node.snakeIndex = hotModNode.snakeIndex;
//									}
//									//Debug.Break ();
//									//hotSnakes[hotModNode.snakeIndex].constituentNodes.AddRange(
//									//	hotSnakes[adjacentNode.snakeIndex].constituentNodes); //Add adjacent snake's modules to center snake
//								}
//								else {Debug.LogError ("Invalid adjacent node index: " + adjacentNode.snakeIndex);}
//							}
//							else
//							{Debug.LogError ("Invalid root node index: " + hotModNode.snakeIndex);}
//
//				}
//				}
//			}
//		}
//		}
//
//		currentSnakes = hotSnakes;
//		return hotSnakes;
//	}
		
//	void ClearOldNodeValues()
//	{
//		foreach(ScriptModule hotMod in GetComponent<ScriptShipController>().shipModuleContainer.GetComponentsInChildren<ScriptModule>())
//		        {
//			Node hotModNode = GetNodeFromModule(hotMod);
//		
//			hotModNode.snakeIndex = -1;
//			hotModNode.activationIndex = -1;
//			hotModNode.isAdded = false;
//		}
//	}

//	List<Activation> ConvertSnakesToActivations(List<Snake> hotSnakes)
//	{
//		List<Activation> hotActivations = new List<Activation> ();
//		foreach (Snake snake in hotSnakes) {
//			//Debug.Log ("Node count >= min Nodes " + snake.constituentNodes.Count + ", " + minNodesForActivation);
//			if(snake.constituentNodes.Count >= minNodesForActivation)
//			{
//				hotActivations.Add (new Activation(snake.snakeID, snake.moduleType, snake.constituentNodes)); 
//				foreach(Node node in snake.constituentNodes)
//				{
//					node.activationIndex = hotActivations.Count - 1;
//				}
//			}
//				}
//				                    return hotActivations;
//	}


//	public void ArmActivations (List<Activation> hotActivations)
//	{
//
//		scriptShipController.ClearOldLasers();
//		//Debug.Log ("Arming");
//		foreach(Activation activation in hotActivations)
//		{
//
//				//Calculate activation stats
//				foreach(Node node in activation.constituentNodes)
//				{
//					switch(node.module.moduleType)
//					{
//					case ModuleType.Weapon:
//
//					switch(node.module.moduleSubtype)
//					{
//					case ModuleSubtype.Duration:
//						activation.gunDurationLevel++;
//						break;
//					//case ModuleSubtype.Homing:
//					//	activation.homingLevel++;
//					//	break;
//					case ModuleSubtype.Number:
//						activation.gunNumberLevel++;
//						break;
//					case ModuleSubtype.Power:
//						activation.gunPowerLevel++;
//						break;
//					case ModuleSubtype.Radius:
//						activation.gunRadiusLevel++;
//						break;
//					case ModuleSubtype.Speed:
//						activation.gunSpeedLevel++;
//						break;
//					default:
//						Debug.LogError("Invalid module subtype: " + node.module.moduleSubtype);
//						break;
//					}
//						break;
//					case ModuleType.Armor:
//						activation.armorPowerLevel++;
//						break;
//					case ModuleType.Laser:
//						activation.laserRadiusLevel++;
//						break;
//					default:
//						Debug.Log ("Invalid module type: " + node.module.moduleType);
//					break;
//					}
//
//					activation.DeriveRealStats(activation.moduleType);
//
//				if(activation.moduleType == ModuleType.Armor)
//				{
//					node.module.currentHP += activation.bonusHP;
//				} 
//				}
//			//For each activation
//			if(activation.moduleType == ModuleType.Laser)
//			{
//				scriptShipController.InitializeLaser(activation);
//			}
//			
//			
//
//		}
//
//		//return hotActivations;
//	}


//	public List<Activation> GetActivations()
//	{
//		return ConvertSnakesToActivations(currentSnakes);
//	}

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

	public void AddModuleToGrid(ScriptModule module, Vector2 schematicCoordinates)
	{
		Node centerNode = new Node(module);
		schematic[(int)schematicCoordinates.x, (int)schematicCoordinates.y] = centerNode;

		Vector2[] adjacentPoints = GetAdjacentPoints(schematicCoordinates);
		//I. Check adjacent nodes for like module types
		//Debug.Log ("Adjacent points: "+adjacentPoints);
		foreach(Vector2 adjacentNodeCoordinates in adjacentPoints)
		{
			Node adjacentNode = schematic[(int)adjacentNodeCoordinates.x, (int)adjacentNodeCoordinates.y];
			if(!adjacentNode.isEmpty)
			{
				if(adjacentNode.module.moduleType == centerNode.module.moduleType)
				{
					if(adjacentNode.snakeIndex == -1) //A. Adjacent unowned
					{
						if(centerNode.snakeIndex == -1) //1. Center unowned- Add center and adjacent to new snake
						{
							Snake hotSnake = new Snake(currentSnakes, centerNode.module.moduleType);
							hotSnake.AddNodeToSnake(centerNode);
							hotSnake.AddNodeToSnake(adjacentNode);
						} else if(centerNode.snakeIndex >= 0) //2. Center owned- Add adjacent to center's snake
						{
							currentSnakes[centerNode.snakeIndex].AddNodeToSnake(adjacentNode);
						} else {
							Debug.LogError("Invalid snake index on center: "+centerNode.module.moduleID);
						}
					} else if(adjacentNode.snakeIndex >= 0) //B. Adjacent owned
					{
						if(centerNode.snakeIndex == -1) //1. Center unowned- Add center to adjacent's snake 
						{
							currentSnakes[adjacentNode.snakeIndex].AddNodeToSnake(centerNode);
						} else if (centerNode.snakeIndex == adjacentNode.snakeIndex) //2. Center owned by same- Do nothing
						{
							//Do nothing
						} else if (centerNode.snakeIndex >= 0) //3. Center owned by different- Subsume adjacent snake into current snake
						{
							currentSnakes[adjacentNode.snakeIndex].isPruned = true; //Mark adjacent snake as dead
							//Assign node's snake id as root node snake
							List<Node> transferList = new List<Node>(currentSnakes[adjacentNode.snakeIndex].constituentNodes); //Cache adjacent snake's nodes
							currentSnakes[adjacentNode.snakeIndex].constituentNodes = new List<Node>(); //Clear adjacent snake's nodes
							foreach(Node transferNode in transferList)
							{
								//Debug.Log("12 - root snake, adjacent snake, module:" + hotModNode.snakeIndex + ", " + adjacentNode.snakeIndex + ", " + transferNode.module.name);
								currentSnakes[centerNode.snakeIndex].AddNodeToSnake(transferNode);
								//node.snakeIndex = hotModNode.snakeIndex;
							}
						} else {
							Debug.LogError("Invalid snake index on center: "+centerNode.module.moduleID);
						}
					} else {
						Debug.LogError ("Invalid snake index on adjacent: "+adjacentNode.module.moduleID);
					}
					//Debug.Log ("Center, adjacent: {" + centerNode.module.moduleID+", "+ adjacentNode.module.moduleID+"}");
				}
		}
		}

		if(centerNode.snakeIndex != -1)
		{
			if(currentSnakes[centerNode.snakeIndex].constituentNodes.Count >= minNodesForActivation)
			{
				currentSnakes[centerNode.snakeIndex].SetArmed(true);
			}
		}

	}

	public void RemoveModuleFromGrid(ScriptModule module)
	{
		//Node centerNode = new Node(module);
		//schematic[(int)schematicCoordinates.x, (int)schematicCoordinates.y] = centerNode;
		Node centerNode = GetNodeFromModule(module);
		Vector2 schematicCoordinates = GetGridNodeCoordinates(centerNode.module.moduleNodeCoordinates);
		schematic[(int)schematicCoordinates.x, (int)schematicCoordinates.y] = new Node();

		if(centerNode.snakeIndex >= 0)
		{

		Vector2[] adjacentPoints = GetAdjacentPoints(schematicCoordinates);

		foreach(Vector2 adjacentNodeCoordinates in adjacentPoints)
		{
				Node adjacentNode = schematic[(int)adjacentNodeCoordinates.x, (int)adjacentNodeCoordinates.y];
				if(!adjacentNode.isEmpty)
				{
					if(adjacentNode.module.moduleType == centerNode.module.moduleType)
					{
						//If exactly one adjacent node belongs to same snake, remove center node from snake
							//If current snake now has one module, remove snake
						//If more than one adjacent node belongs to the same snake
							//Depth-first search to see which adjacent nodes (if any) are connected
						//If node is unconnected, split into new snake

					}
				}
		}

		}
	}

}
