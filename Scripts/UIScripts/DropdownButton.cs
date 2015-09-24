using UnityEngine;
using System.Collections;

public class DropdownButton : MonoBehaviour {

	[SerializeField]
	private RectTransform dropContent;

	private bool isDropped;

	private Vector3 up;
	private Vector3 down;
	//private float rotationTime = 0.25f;
	
	public RectTransform buttonImage;

	public void Start()
	{
		isDropped = dropContent.gameObject.activeSelf;
		up = new Vector3 (0, 0, 90);
		down = new Vector3 (0, 0, 0);
		toggleButtonState (isDropped);
	}

	public void ToggleDropState()
	{
		isDropped = (!isDropped);
		dropContent.gameObject.SetActive (isDropped);
		toggleButtonState (isDropped);
	}

	public void toggleButtonState(bool isDropped)
	{
		if (isDropped) {
			buttonImage.eulerAngles = down;
		} else {
			buttonImage.eulerAngles = up;
		}
	}
}
