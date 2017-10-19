using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingStatus : MonoBehaviour {

	public string nickname;
	public int skinID;
	public int controlMethodId;
	public InputField input;
	void Start(){

	}
	//Set the settings when related button is clicked.			
	public void Selected(string ButtonName){
		if (ButtonName == "Skin1") {
			skinID = 1;
			PlayerPrefs.SetInt ("skinID", skinID);		
		} else if (ButtonName == "Skin2") {
			skinID = 2;
			PlayerPrefs.SetInt ("skinID", skinID);
		} else if (ButtonName == "Skin3") {
			skinID = 3;
			PlayerPrefs.SetInt ("skinID", skinID);		
		} else if (ButtonName == "Control1") {
			controlMethodId = 1;
			PlayerPrefs.SetInt ("moveWayID", controlMethodId);
		} else if (ButtonName == "Control2") {
			controlMethodId = 2;
			PlayerPrefs.SetInt ("moveWayID", controlMethodId);		
		} else if (ButtonName == "Control3") {
			controlMethodId = 3;
			PlayerPrefs.SetInt ("moveWayID", controlMethodId);		
		} else if (ButtonName == "removeAds") {

			PlayerPrefs.SetInt ("removeAds", 1);		
		} else if (ButtonName == "showAds") {

			PlayerPrefs.SetInt ("removeAds", 0);		
		}
		//When button "PlayOnline" or "PlayWithAI" is clicked, set the nickname and load game scene.
		else if (ButtonName == "PlayOnline" || ButtonName == "PlayWithAI") {
			input = GameObject.Find ("Nickname").GetComponent<InputField> ();
			PlayerPrefs.SetString ("nickname",input.text);		
			SceneManager.LoadScene("Slither.io");

		
		}

	}
	}