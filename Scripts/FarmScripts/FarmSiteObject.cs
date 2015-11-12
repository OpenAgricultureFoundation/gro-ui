using UnityEngine;
using System.Collections;

public class FarmSiteObject : MonoBehaviour {

	[HideInInspector] public FarmSite mySite;
	[HideInInspector] public bool clickable = false;
	[HideInInspector] public bool hoverable = true;
	public string name;

	public Color originalColor;
	public Color highlightColor;
	public Color dehighlightColor;
	public Color selectedColor;
	private Color normalColor;
	private Color hoverColor;
	private bool clicked = false;
	private bool hovering = false;
	private float guiWidth = 150f;
	private float guiHeight = 20f;



	public void Start()
	{
		hoverColor = highlightColor;
	}

	public IEnumerator Initialize()
	{
		yield return null;
	}

	public void OnMouseEnter()
	{
		if (hoverable) 
		{
			//normalColor = GetComponent<Renderer> ().material.color;
			//GetComponent<Renderer> ().material.color = hoverColor;
		}
		hovering = true;
	}
	public void OnMouseExit()
	{
		if (hoverable) 
		{
			//GetComponent<Renderer> ().material.color = normalColor;
		}
		hovering = false;
	}
	public void OnMouseDown()
	{
		if(clickable)
		{
			clicked = !clicked;
			if(clicked)
			{
				normalColor = selectedColor;
				hoverColor = dehighlightColor;
			}
			else
			{
				normalColor = originalColor;
				hoverColor = highlightColor;
			}
			//GetComponent<Renderer>().material.color = normalColor;
			mySite.Selected = clicked;
		}
	}

	public void Deselect()
	{
		//GetComponent<Renderer> ().material.color = originalColor;
		hoverColor = highlightColor;
	}

	void OnGUI()
	{
		GUI.skin = DataManager.dataManager.GUISkin1;
		if (hoverable && hovering) 
		{
			float x = Event.current.mousePosition.x;
			float y = Event.current.mousePosition.y;

			Rect labelRect = GUILayoutUtility.GetRect(new GUIContent(name), "label");
			labelRect.x = x + labelRect.width/2.0f;
			labelRect.y = y - labelRect.height/2.0f;
			GUI.Label(labelRect, name);
			//float labelWidth = GUI.skin.GetStyle("label").CalcSize(GUIContent(name)).x;
			//GUI.Label(new Rect(x + labelWidth/2.0f, y - guiHeight/2.0f, guiWidth, guiHeight), name);
			//GUI.Label(new Rect(x, y - guiHeight/2.0f, guiWidth, guiHeight), name);
		}
	}
}






