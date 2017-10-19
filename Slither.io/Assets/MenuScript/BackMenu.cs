using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class BackMenu: MonoBehaviour {
	public void back(){
		if (PlayerPrefs.GetInt("removeAds",0)==0){
			ShowAd();
		}
		SceneManager.LoadScene ("Menu");
	}



	public void ShowAd()
	{
//		if (Advertisement.IsReady ()) {
//			Advertisement.Show ();
//		} else {
//			Advertisement.Initialize ("1165124");
//		}


		if (!Advertisement.IsReady())
		{
		Debug.Log("Ads not ready for default placement");
		return;
		}

		Advertisement.Show();

	}
}
