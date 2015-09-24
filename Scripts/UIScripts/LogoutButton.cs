using UnityEngine;
using System.Collections;

public class LogoutButton : MonoBehaviour {

	public void LogoutButtonPress()
	{
		StartCoroutine("Logout");
	}
	
	public IEnumerator Logout()
	{
		WWWForm form = new WWWForm();
		string url = DataManager.dataManager.ipAddress + "/auth/logout/";
		
		object[] parms = new object[] {url, form};
		yield return DataManager.dataManager.StartCoroutine("PostRequest", parms);
		
		DataManager.dataManager.token = "";
		DataManager.dataManager.username = "";
		DataManager.dataManager.StartCoroutine("Save");
		
		Application.LoadLevel(0);
		
		yield return null;
	}
}
