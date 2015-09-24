using UnityEngine;
using System.Collections;

public class ColumnLayoutManager : MonoBehaviour 
{
	private float outterMarginPixels;
	private float innerSpacingPixels;
	private float columnWidthPixels;

	public float outterMarginPercent;
	public float innerSpacingPercent;
	public Camera camera;

	public void Start ()
	{
		calculateColumnValues ();
	}

	public void calculateColumnValues()
	{
		outterMarginPixels = camera.pixelWidth * outterMarginPercent;
		innerSpacingPixels = camera.pixelWidth * innerSpacingPercent;
		columnWidthPixels = (camera.pixelWidth - (2.0f * outterMarginPixels + 11.0f * innerSpacingPixels))/12.0f;
		/**
		Debug.Log (camera.pixelWidth);
		Debug.Log (outterMarginPixels);
		Debug.Log (innerSpacingPixels);
		Debug.Log (columnWidthPixels);
		*/
	}
}
