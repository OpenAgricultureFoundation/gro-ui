using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FarmObject: MonoBehaviour {

	public GameObject genericFarmObject;
	public GameObject trayFarmObject;

	// Required for setup
	[HideInInspector] public Transform parentTransform;
	public string myURL;
	public bool isUnityPrimitive;
	// Set during / after setup
	[HideInInspector] public List<Transform> childrenTransforms = new List<Transform>();
	[HideInInspector] public List<FarmResource> resourceList;
	[HideInInspector] public bool isTrayObject = false;

	private JSONNode node;
	private JSONNode node2;


	//OLD
	private void Setup ()
	{
		float x, y, z;
		float length, width, height;
		string name = "Unnamed";
		Vector3 scale, position;


		// Set object variables

		x = node ["x"].AsFloat;
		z = node ["y"].AsFloat;
		y = node ["z"].AsFloat;
		length = node ["length"].AsFloat;
		width = node ["width"].AsFloat;
		height = node ["height"].AsFloat;
		if (node["name"] != null)
		{
			name = node ["name"];
		}

		// Set object position, scale, parent, name

		// Set Name
		gameObject.name = name;
		// Set Scale
		scale = new Vector3 (length, height, width);
		transform.localScale = scale;
		// Set Parent gameObject (transform)
		transform.SetParent (parentTransform);
		// Set Position
		Vector3 plScale = transform.parent.lossyScale;
		position = new Vector3 (x / plScale.x, y / plScale.y, z / plScale.z);
		if (isUnityPrimitive) 
		{
			Vector3 primitiveOffset = new Vector3(transform.localScale.x*0.5f - 0.5f, transform.localScale.y*0.5f - 0.5f, transform.localScale.z*0.5f - 0.5f);
			position += primitiveOffset;
		}
		transform.localPosition = position;

		InitializeResources ();

		if(isTray(myURL))
		{
			isTrayObject = true;
			FarmTray script = GetComponent<FarmTray>();
			script.node = node;
			script.PopulateTray();
		}

		// Check for children, and create as necessary

		if (node["children"] == null ) 
		{
			return;
		}

		JSONArray children = node["children"].AsArray;
		foreach (JSONNode child in children) 
		{

			GameObject toCreate = genericFarmObject;
			// Check type for tray vs generic
			if(isTray (child))
			{
				toCreate = trayFarmObject;
			}

			// Instantiate correct object
			GameObject childObject = Instantiate(toCreate) as GameObject;
			// Set parent
			childObject.transform.GetComponent<FarmObject>().parentTransform = transform;
			childrenTransforms.Add(childObject.transform);
			// Call setup
			childObject.GetComponent<FarmObject>().Build(child);
		}


	}
	//OLD
	IEnumerator Query (string URL)
	{
		//print ("New query about to be made at: " + URL);
		WWW www = new WWW (URL);
		yield return www;
		node = JSON.Parse (www.text);
		// Check for Empty Node
		if (isEmpty (node)) {
			Debug.Log ("Query returned an empty object");
			yield return null;
		}
		myURL = URL;
		Setup ();
	}

	public IEnumerator Initialize(string url)
	{
		float x, y, z;
		float length, width, height;
		string name = "Unnamed";
		Vector3 scale, position;

		// Query
		if (!DataManager.dataManager.databaseMirror.ContainsKey (url)) 
		{
			yield return DataManager.dataManager.StartCoroutine("SingleQuery", url);
		}
		node2 = DataManager.dataManager.databaseMirror[url];
		myURL = url;

		// Setup
		// Get node contents
		
		x = node2 ["x"].AsFloat;
		z = node2 ["y"].AsFloat;
		y = node2 ["z"].AsFloat;
		length = node2 ["length"].AsFloat;
		width = node2 ["width"].AsFloat;
		height = node2 ["height"].AsFloat;
		if (node2["name"].Value != "null")
		{
			name = node2 ["name"].Value;
		}

		// Set object position, scale, parent, name
		// Set Name
		gameObject.name = name;
		// Set Scale
		scale = new Vector3 (length, height, width);
		transform.localScale = scale;
		// Set Parent gameObject (transform)
		transform.SetParent (parentTransform);
		// Set Position
		Vector3 plScale = transform.parent.lossyScale;
		position = new Vector3 (x / plScale.x, y / plScale.y, z / plScale.z);
		if (isUnityPrimitive) 
		{
			Vector3 primitiveOffset = new Vector3(transform.localScale.x*0.5f - 0.5f, transform.localScale.y*0.5f - 0.5f, transform.localScale.z*0.5f - 0.5f);
			position += primitiveOffset;
		}
		transform.localPosition = position;

		// Load Tray / Children, and Resources
		yield return StartCoroutine (LoadResources ());

		if (isTray (url)) 
		{
			isTrayObject = true;
			yield return StartCoroutine(LoadTray ());
		}

		JSONArray children = node2["children"].AsArray;
		foreach (JSONNode child in children) 
		{
			yield return StartCoroutine(LoadChild (child));
		}
		// Return
		yield return null;
	}

	public IEnumerator LoadChild(JSONNode child)
	{
		GameObject obj;

		// Create child
		if (isTray (child)) 
		{
			obj = Instantiate (trayFarmObject) as GameObject;
		} else 
		{
			obj = Instantiate (genericFarmObject) as GameObject;
		}
		childrenTransforms.Add (obj.transform);
		FarmObject script = obj.GetComponent<FarmObject> ();
		script.parentTransform = transform;

		yield return script.StartCoroutine ("Initialize", child.Value);

		yield return null;
	}

	public IEnumerator LoadTray()
	{
		// Access FarmTray script
		// yield return InitializeTray
		FarmTray script = GetComponent<FarmTray> ();
		script.node = node2;
		yield return script.StartCoroutine ("Initialize");
		yield return null;
	}

	public IEnumerator LoadResources()
	{
		// foreach FarmResource
			// Create Resource
			// yield return InitializeRescource
		FarmResourceManager manager = GameObject.FindGameObjectWithTag ("FarmResourceManager").GetComponent<FarmResourceManager> ();
		JSONArray resources = node2 ["resources"].AsArray;
		foreach ( JSONNode resource in resources)
		{
			manager.CreateResource(resource, this);
		}
		yield return null;
	}

	//OLD
	public void Build (string URL)
	{
		StartCoroutine (Query (URL));
	}


	//OLD
	void InitializeResources () 
	{
		FarmResourceManager manager = GameObject.FindGameObjectWithTag ("FarmResourceManager").GetComponent<FarmResourceManager> ();
		JSONArray resources = node2 ["resources"].AsArray;
		foreach ( JSONNode resource in resources)
		{
			manager.CreateResource(resource, this);
		}
	}


	public void activate()
	{
		// Set parent to parentTransform
		transform.SetParent (parentTransform);
		// Make gameObject Active
		transform.gameObject.SetActive (true);


		// Activate children, and turn their colliders off
		foreach(Transform child in childrenTransforms)
		{
			child.GetComponent<FarmObject>().activate();
			child.GetComponent<Collider>().enabled = false;
		}
	}


	public void deactivate()
	{
		// Set parent to parentTransform*
			// Unless parentTransform is FarmObjects, then send to inactive object folder
		if(parentTransform.gameObject.name == "FarmObjects")
		{
			transform.SetParent(GameObject.FindGameObjectWithTag("Inactive").transform);
		}
		else
		{
			transform.SetParent(parentTransform);
			parentTransform.GetComponent<FarmObject>().deactivate();
		}
		//transform.gameObject.SetActive (false);
		GetComponent<Collider>().enabled = false;
		// Disable gameobject

		// Deactivate parentTransform

	}

	public void selected()
	{
		GameObject.FindGameObjectWithTag ("FarmManager").GetComponent<FarmManager> ().SetActiveObject (transform);
	}

	public void OnMouseDown()
	{
		selected ();
	}

	// TODO: Implement
	bool isEmpty (JSONNode node)
	{
		// Do a check for emptiness

		return false;
	}

	public bool isTray (string URL)
	{
		string[] split = URL.Split ("/"[0]);
		foreach(string s in split)
		{
			if (string.Compare(s, "tray", true) == 0)
			{

				return true;
			}
		}
		return false;
	}
}
