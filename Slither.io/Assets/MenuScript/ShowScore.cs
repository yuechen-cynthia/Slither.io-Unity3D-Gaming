using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowScore : MonoBehaviour {
	Text FinalScoreText, BestScoreText;
	InputField nickname;
	Button removeAds, showAds;
	// Use this for initialization
	void Start () {
		string final, best;
		int finalScore,bestScore;

		final = "Y o u r   f i n a l   l e n g t h   w a s  ";
		best = "Y o u r   b e s t   l e n g t h   e v e r   w a s  ";
		//Get the final score and best score from game.
		finalScore = PlayerPrefs.GetInt ("FinalScore", -1);
		bestScore = PlayerPrefs.GetInt ("BestScore", -1);
		//Get the target UI
		FinalScoreText = GameObject.Find("final").GetComponent<Text>();
		BestScoreText = GameObject.Find("bst").GetComponent<Text>();
		nickname = GameObject.Find("Nickname").GetComponent<InputField>();
//		transform.Find ("Canvas/remove").gameObject.SetActive();//.GetComponent<Button>()
//		transform.Find ("Canvas/show").gameObject.SetActive();//.GetComponent<Button>()

		if (PlayerPrefs.GetInt ("removeAds", 0) == 0) {
			if (GameObject.FindWithTag ("show") != null) 
			{
				GameObject.FindWithTag ("show").SetActive (false);
			}

			//tansform.Find ("remove").gameObject.SetActive(true);//.GetComponent<Button>()
			//transform.Find ("show").gameObject.SetActive(false);//.GetComponent<Button>()

		} else {
			
			if (GameObject.FindWithTag ("remove") != null) 
			{
				GameObject.FindWithTag ("remove").SetActive (false);
			}
		}

		//Keep showing the nickname after game ends.
		nickname.text = PlayerPrefs.GetString ("nickname","");	
		//Do not show final score and best score when app is just opened.
		if (finalScore == -1) {
			FinalScoreText.enabled = false;
			BestScoreText.enabled = false;
		} else {
		//Update the best score if final score is greater than last best score.
			FinalScoreText.text = final + finalScore;
			BestScoreText.text = best + bestScore;
		}




	}
}
