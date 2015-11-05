using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ErrorMessageController : MonoBehaviour {

	private string message, stack;
	
	public Text errorDisplayText, stackDisplayText;
	
	public void SetErrorMessage(string message) 
	{
		this.message = message;
		errorDisplayText.text = this.message;
	}
	
	public void SetStackMessage(string stack)
	{
		this.stack = stack;
		stackDisplayText.text = this.stack;
	}
	
	public void OkButtonPress() 
	{
		Destroy(this.gameObject);
	}
}
