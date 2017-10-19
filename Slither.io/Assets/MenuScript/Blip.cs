using UnityEngine;
using System.Collections;

public class Blip : MonoBehaviour 
{
	Minimap map;
	RectTransform myRectTransform;

	void Start()
	{	
		//Get the minimap and positon.
		map = GetComponentInParent<Minimap> ();
		myRectTransform = GetComponent<RectTransform> ();
	}

	void LateUpdate()
	{
		//Show the position in minimap
		Vector2 newPosition = map.TrasformPosition (); //Target.transform.position
		myRectTransform.localPosition = newPosition;
	}
}
