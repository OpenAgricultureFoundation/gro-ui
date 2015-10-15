using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ActionButtonManager : MonoBehaviour {

	public static ActionButtonManager actionButtonManager;

	public List<Button> actionButtons = new List<Button>();
	public Button collapseButton;
	public Image collapseArrow;

	public bool isGuest = false;

	private bool collapsed = false;
	private float panelWidth = 140;

	public void Awake(){
		// to test if isGuest deactivates. 
		//isGuest = true;

		if (actionButtonManager == null) {
			//DontDestroyOnLoad (gameObject);
			actionButtonManager = this;
		} else if (actionButtonManager != this) {
			Destroy (gameObject);
		}

		if (isGuest == true) {
			foreach (Button button in actionButtons) {
				button.interactable = false;
			}
		}
	}

	public void SetActionButtonsInteractable(bool val)
	{
		if (isGuest == false) {
			foreach (Button button in actionButtons) {
				button.interactable = val;
			}
		}
	}

	public void ModuleStart()
	{
		if (isGuest == false) {

			SetActionButtonsInteractable (false);
		}
	}

	public void ModuleEnd()
	{
		if (isGuest == false) {

			SetActionButtonsInteractable (true);
		}
	}

	public void CollapsePanel(bool val)
	{
		if (isGuest == false) {

			foreach (Button button in actionButtons) {
				button.gameObject.SetActive (!val);
			}

			if (val) {
				collapseArrow.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, -90f));
				transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (collapseButton.GetComponent<RectTransform> ().rect.width, transform.GetComponent<RectTransform> ().sizeDelta.y);
			} else {
				collapseArrow.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, 90f));
				transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (panelWidth, transform.GetComponent<RectTransform> ().sizeDelta.y);
			}
		}
	}

	public void CollapseButtonPress()
	{
		if (isGuest == false) {

			collapsed = !collapsed;

			CollapsePanel (collapsed);
		}
	}
}
