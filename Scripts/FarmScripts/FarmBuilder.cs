using UnityEngine;
using System.Collections;
using SimpleJSON;

public class FarmBuilder : MonoBehaviour {

	public Transform PlantosObject;
	public Transform TrayObject;

	public void Start ()
	{
		string URL = "18.133.7.124:8082/tray/1/";
		Transform tray = Instantiate(TrayObject) as Transform;
		tray.GetComponent<FarmObject> ().Build (URL);
		//Debug.Log ("Query started");

	}

	// TODO: Finish InitialBuild
	/**
	 * This should be the first build call
	 * This function will check the current (first) object's type (Tray or not)
	 * 	then call the appropriate build function
	 * @Param: node, type is JSONClass
	 */ 
	void InitialBuild(JSONClass node)
	{
		// Check for empty node

		// Check URL for type
		string url = node["url"];
		string[] urlSplit = url.Split ("/" [0]);
		//string objType = urlSplit [urlSplit.Length - 2];

		foreach(string split in urlSplit)
		{
			if(string.Compare(split, "enclosure", true) == 0)
			{
				BuildEnclosure(node, transform);
			}
			else if(string.Compare(split, "tray", true) == 0 )
			{
				// Obj is a tray, call BuildTray
				BuildTray (node, transform);
			}
		}
		// Obj is not a tray, call Build
		Build (node, transform);
	}
















	// TODO: Finish Build
	/**
	 * Takes in a system 'node' and builds it
	 * If it has a child, calls Build on that node
	 * If the child is a Tray, calls BuildTray
	 * 
	 * @Param: node, type is JSONClass, the node / system object to build
	 * @Param: parent, type is Transform, the parent object for this node
	 */
	void Build( JSONClass node, Transform parent)
	{
		// Check for empty node
		if ( isEmpty(node) ) {
			// Send error message
			print ("Node is empty!");
			return;
		}

		// Get URL
		string url = node["url"];
		//string[] urlSplit = url.Split ("/" [0]);



		/* This should go in the recursion step
		foreach(string split in urlSplit)
		{
			if(string.Compare(split, "tray", true) == 0 )
			{
				// Child obj is a tray
			}
		}
		*/

	}
	// TODO: Finish Build Tray
	/*
	 * Takes a Tray Node and builds it
	 * @Param: tray, type JSONClass, the tray to build
	 * @Param: parent, type is Transform, the parent object for this tray
	 */
	void BuildTray(JSONClass node, Transform parent)
	{

	}

	void BuildEnclosure( JSONClass node, Transform parent)
	{
		//Instantiate empty game object with lossy scale of enclsoue dims
		// Recurse through 
	}

	// TODO: Finish isEmpty
	private bool isEmpty( JSONClass node ) 
	{
		bool emptiness = false;

		// Logic

		return emptiness;
	}
}
