using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class RecipeChoice : MonoBehaviour {

	public Text recipeNameDisplay, plantTypeDisplay;

	[HideInInspector] public JSONNode recipe;
	[HideInInspector] public ISelectionReceiver<RecipeChoice> selectionReceiver;
	[HideInInspector] public string url;


	public IEnumerator Initialize()
	{
		url = recipe ["url"].Value;
		recipeNameDisplay.text = recipe ["name"].Value;
		yield return null;
	}

	public void ToggleSet(bool val)
	{
		if (val)
		{
			selectionReceiver.MakeSelection(this);
		}
	}
}
