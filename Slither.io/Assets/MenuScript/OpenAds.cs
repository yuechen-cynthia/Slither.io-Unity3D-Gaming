using UnityEngine;
using UnityEngine.Advertisements;

public class OpenAds : MonoBehaviour
{
	public void ShowAd()
	{
//		if (Advertisement.IsReady())
//		{
//			Advertisement.Show();
//		}


		if (!Advertisement.IsReady())
		{
		Debug.Log("Ads not ready for default placement");
		return;
		}

		Advertisement.Show();

	}
}

