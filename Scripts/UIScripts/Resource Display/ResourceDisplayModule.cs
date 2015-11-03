using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ResourceDisplayModule : MonoBehaviour {

	public GameObject sensingPointModulePrefab;
	public Text nameDisplay, locationDisplay;
	public Button iconDisplayButton;
	public Image icon;
	public ColorBlock tempcolorblock; // added to modify struct. 
	public ColorBlock tempcolorblockExpanded;// so button stays highlighted on click. 
	public Color32 tempcolor; // added to modify value type
	public Transform sensorListContent, sensorListOutter;
	public LayoutElement layoutElement;


	public Color32 lightNormal = new Color32(201,216,113,255);
	public Color32 lightHighlighted =new Color32(247,255,103,255);
	public Color32 waterNormal = new Color32(64,120,186,255);
	public Color32 waterHighlighted =new Color32 (116,182,238,255);
	public Color32 airNormal = new Color32(129,146,156,255);
	public Color32 airHighlighted = new Color32 (185,209,223,255);

	[HideInInspector] public SensorDisplayManager sensorDisplayManager;
	[HideInInspector] public FarmResource resource;
	[HideInInspector] public FarmObject farmObject;
	[HideInInspector] public List<FarmSensingPoint> sensingPoints = new List<FarmSensingPoint>();
	[HideInInspector] public string name, location;
	[HideInInspector] public bool expanded = true;

	private float baseHeight = 80.0f;
	private List<SensingPointModule> sensingPointModules = new List<SensingPointModule> ();


	void Start(){
		if (nameDisplay.text == "Light"){
			print ("passed light");
			gameObject.GetComponent<Image>().color = new Color32(141,141, 88,255);
			tempcolorblock = iconDisplayButton.colors;
			tempcolorblock.normalColor = lightNormal;
			tempcolorblock.highlightedColor=lightHighlighted;
			tempcolorblock.pressedColor= lightHighlighted;
			tempcolorblockExpanded = tempcolorblock;
			tempcolorblockExpanded.normalColor=lightHighlighted;
			print("accessed light icon");
		}
		if (nameDisplay.text == "Water"){
			print ("passed water");
			gameObject.GetComponent<Image>().color = new Color32(53,88, 128,255);
			tempcolorblock=iconDisplayButton.colors;
			tempcolorblock.normalColor= waterNormal;
			tempcolorblock.highlightedColor = waterHighlighted;
			tempcolorblock.pressedColor= waterHighlighted;
			tempcolorblockExpanded = tempcolorblock;

			tempcolorblockExpanded.normalColor=waterHighlighted;
			print("accessed water icon");

		}
		if (nameDisplay.text == "Air"){
			print ("passed air");
			gameObject.GetComponent<Image>().color = new Color32(68,83, 92,255);
			tempcolorblock=iconDisplayButton.colors;
			tempcolorblock.normalColor=airNormal;
			tempcolorblock.highlightedColor = airHighlighted;
			tempcolorblock.pressedColor= airHighlighted;
			tempcolorblockExpanded = tempcolorblock;
			tempcolorblockExpanded.normalColor=airHighlighted;
			print("accessed air icon");
		}
		iconDisplayButton.colors = tempcolorblockExpanded; // if switching to minimized UI, use tempcolorblock. 
	}
	public IEnumerator Initialize(FarmResource re)
	{

		resource = re;
		farmObject = resource.origin;
		// Set name and location
		name = resource.resourceType;
		location = farmObject.name;
		nameDisplay.text = name;
		locationDisplay.text = location;
		//print (resource.resourceType);
		icon.sprite = sensorDisplayManager.icons [resource.resourceType.ToLower()];
		// Create all sensing points
		foreach (FarmSensor sensor in resource.sensorsList) 
		{
			foreach (FarmSensingPoint point in sensor.farmSensingPoints)
			{
				yield return StartCoroutine("AddSensingPoint", point);
			}
		}
		transform.localScale = new Vector3 (1,1,1);

		yield return StartCoroutine ("UpdateSize");

		yield return null;
	}

	public IEnumerator AddSensingPoint(FarmSensingPoint point)
	{
		if (!point.activePoint) 
		{
			yield break;
		}

		GameObject module = Instantiate (sensingPointModulePrefab) as GameObject;
		// Set parent to GUICanvas item
		module.transform.SetParent (sensorListContent, false);

		// Call get values
		sensingPoints.Add (point);

		SensingPointModule script = module.GetComponent<SensingPointModule> ();
		script.sensingpoint = point;

		sensingPointModules.Add (script);

		script.StartCoroutine ("Initialize");

		yield return null;
	}

	public void RestartSensors()
	{
		foreach (SensingPointModule point in sensingPointModules) 
		{
			point.StopCoroutine("GetReadings");

			if(expanded)
			{
				point.StartCoroutine("GetReadings");
			}
		}
	}

	public IEnumerator UpdateSize()
	{


		float iconDim = iconDisplayButton.GetComponent<RectTransform> ().rect.width;
		float spContentHeight = sensorListContent.GetComponent<RectTransform>().rect.height;

		float newHeight, newWidth;
		if (expanded) 
		{
			newWidth = iconDim + 170.0f + 10f;
			newHeight = iconDim + spContentHeight + 10.0f;
		}
		else
		{
			newWidth = iconDim + 10.0f;
			newHeight = iconDim + 10.0f;
		}


		layoutElement.minWidth = newWidth;
		layoutElement.minHeight = newHeight;
		yield return null;
	}

	public void IconButtonPress()
	{
		expanded = !expanded;

		if (expanded) {
			iconDisplayButton.colors = tempcolorblockExpanded;
		}

		else{
			iconDisplayButton.colors=tempcolorblock;
		}
		sensorListOutter.gameObject.SetActive (expanded);
		nameDisplay.gameObject.SetActive (expanded);
		locationDisplay.gameObject.SetActive (expanded);

		RestartSensors ();

		StopCoroutine ("UpdateSize");
		StartCoroutine ("UpdateSize");


	}
}
