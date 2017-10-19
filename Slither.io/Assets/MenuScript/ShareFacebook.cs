using UnityEngine;
using System.Collections;
using Facebook.Unity;
using UnityEngine.UI;


public class ShareFacebook : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void Awake(){
	
		if (!FB.IsInitialized) {
		
			FB.Init();
		} else {
		
			FB.ActivateApp();
		
		}


	}

//	public void logIn(){
//		{
//			FB.LogInWithReadPermissions ();
//		}
//	}
//
//	private void OnLogIn(ILoginResult result){
//	
//		if (FB.IsLoggedIn) {
//		
//			AccessToken token = AccessToken.CurrentAccessToken;
//			userIdText.text = token.UserId;
//		} else {
//
//			Debug.Log ("Canceled Login");
//		
//		}
//	}

	public void Share(){

		FB.ShareLink (
			contentTitle:"Slither from Team CCMP",
			contentDescription:"Hey,join this game and play with me!",
			callback: OnShare);	
	}

	private void OnShare(IShareResult result){
				
		if (result.Cancelled || !string.IsNullOrEmpty (result.Error)) {
		
			Debug.Log ("Share score error" + result.Error);
		} else if (!string.IsNullOrEmpty(result.PostId)){
		
			Debug.Log (result.PostId);
		} else
			Debug.Log("Share succeed!");
	}

}
