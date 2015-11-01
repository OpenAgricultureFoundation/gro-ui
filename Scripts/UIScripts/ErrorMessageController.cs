using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ErrorMessageController : MonoBehaviour {

	private string message;
	
	public Text errorDisplayText;
	
	public void SetErrorMessage(string message) 
	{
		this.message = message;
		errorDisplayText.text = this.message;
	}
	
	public void OkButtonPress() 
	{
		Destroy(this.gameObject);
	}
}
