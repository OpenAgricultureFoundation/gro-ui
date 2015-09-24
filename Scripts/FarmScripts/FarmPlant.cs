using UnityEngine;
using System.Collections;
using SimpleJSON;

public class FarmPlant : MonoBehaviour {


	public float xOff, yOff, zOff, xScale, yScale, zScale;

	public string commonName, latinName;
	public JSONNode plantNode, plantTypeNode;

	[HideInInspector] public bool isEmpty;
	[HideInInspector] public FarmSite site;

	public Color selectColor = new Color(255f, 255f, 255f, 255f);

	public bool hovering = false, 
				hoverable = true,
				selectable = false,
				clickable = false;

	public IEnumerator Initialize()
	{
		transform.localPosition = new Vector3 (xOff, yOff, zOff);
		transform.localScale = new Vector3 (xScale, yScale, zScale);
		hoverable = (!isEmpty);
		clickable = hoverable;
		if (!isEmpty) 
		{
			commonName = plantTypeNode["common_name"].Value;
			latinName = plantTypeNode["latin_name"].Value;
		}

		yield return null;
	}

	public void OnMouseEnter()
	{
		hovering = true;
		if (selectable || clickable) 
		{
			Highlight(true);
		}
	}

	public void OnMouseExit()
	{
		hovering = false;
		if (selectable || clickable) 
		{
			Highlight(false);
		}
	}

	void OnMouseDown()
	{
		if ((!isEmpty) && clickable) 
		{
			PlantInfoBoxManager.plantInfoBox.ClearActivePlant();
			PlantInfoBoxManager.plantInfoBox.hasActivePlant = true;
			PlantInfoBoxManager.plantInfoBox.gameObject.SetActive(true);
			PlantInfoBoxManager.plantInfoBox.SetCommonName(commonName);
			PlantInfoBoxManager.plantInfoBox.AddInfo("Scientific Name:", latinName, 1);

			int sownTimestamp = site.plantNode["sow_event"]["timestamp"].AsInt;
			string sownDate = (new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(sownTimestamp)).ToShortDateString();

			PlantInfoBoxManager.plantInfoBox.AddInfo("Planted on:", sownDate);
		}
		if (selectable)
		{
			site.Selected = true;
		}
	}

	public void Highlight(bool val)
	{
		float sign = val ? 1f : -1f;
		Color newColor = new Color ();
		newColor += 0.4f * sign * selectColor;
		newColor += GetComponent<Renderer> ().material.color;

		GetComponent<Renderer> ().material.color = newColor;
	}



	public void OnGUI()
	{
		GUI.skin = DataManager.dataManager.GUISkin1;
		if (hoverable && hovering) 
		{
			float x = Event.current.mousePosition.x;
			float y = Event.current.mousePosition.y;
			
			Rect labelRect = GUILayoutUtility.GetRect(new GUIContent(commonName), "label");
			labelRect.x = x + 5f;// + labelRect.width/2.0f;
			labelRect.y = y - labelRect.height/1.5f;
			GUI.Label(labelRect, commonName);
			//float labelWidth = GUI.skin.GetStyle("label").CalcSize(GUIContent(name)).x;
			//GUI.Label(new Rect(x + labelWidth/2.0f, y - guiHeight/2.0f, guiWidth, guiHeight), name);
			//GUI.Label(new Rect(x, y - guiHeight/2.0f, guiWidth, guiHeight), name);
		}
	}
}
