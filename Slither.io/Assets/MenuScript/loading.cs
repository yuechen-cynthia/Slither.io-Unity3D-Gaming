using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Timers;

public class loading : MonoBehaviour {
	public float time = 0.1f;
	// Use this for initialization
	void Start () {
		//set final score is 0 and nickname is null when loading game.
		PlayerPrefs.SetInt ("FinalScore",-1);
		PlayerPrefs.SetString ("nickname","");
		PlayerPrefs.SetInt ("removeAds", 0);	

	}

	void Update(){
		//Auto load menu scene.
		time -= Time.deltaTime;
		if(time < 0)
		{
			SceneManager.LoadScene ("Menu");
		}

	}
	
}
