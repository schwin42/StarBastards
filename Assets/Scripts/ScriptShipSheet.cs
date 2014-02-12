using UnityEngine;
using System.Collections;
//using System.Collections.Generic;





public class Node
{
	public ScriptModule module;
	public ModuleType moduleType;

	private bool isChecked;

	public Node()
	{
		module = null;
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




	static private int maxY = 1000;
	static private int maxX = 1000;

	public Node[,] schematic = new Node[maxX*2,maxY*2];
	
	// Use this for initialization
	void Start () {
		//foreach (GameObject module in transform) {

		//		}
	}
	
	// Update is called once per frame
	void Update () {
	


	}
}
